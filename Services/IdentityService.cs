using DnsClient.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Poppin.Configuration;
using Poppin.Contracts.Requests;
using Poppin.Data;
using Poppin.Interfaces;
using Poppin.Models.BusinessEntities;
using Poppin.Models.Identity;
using Poppin.Models.Identity.OAuth;
using Poppin.Models.Tracking;
using Segment;
using Segment.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Poppin.Services
{
	/// <summary>
	/// Identity Service based on tutorial by Nick Chapsas
	/// https://www.youtube.com/channel/UCrkPsvLGln62OMZRO6K-llg
	/// </summary>
	public class IdentityService : IIdentityService
	{
		private readonly IJwtSettings _jwtSettings;
		private readonly UserManager<User> _userManager;
		private readonly ISmtpService _smtpService;
		private readonly IUserService _userService;
		private readonly ILogActionService _logActionService;
		private readonly ITokenService _tokenService;
		private readonly TokenDbContext _tokenContext;
		private readonly JwtSecurityTokenHandler _tokenHandler;

		public IdentityService(
			UserManager<User> userMgr,
			IJwtSettings jwtSettings,
			ISmtpService smtpService,
			IUserService userService,
			ILogActionService las,
			ITokenService ts,
			TokenDbContext tokenContext)
		{
			_jwtSettings = jwtSettings;
			_userManager = userMgr;
			_smtpService = smtpService;
			_userService = userService;
			_logActionService = las;
			_tokenService = ts;
			_tokenContext = tokenContext;
			_tokenHandler = new JwtSecurityTokenHandler();
		}

		public async Task<AuthenticationResult> RegisterAsync(string email, string password, string password2, string ipAddress)
		{
			var existingUser = await _userManager.FindByEmailAsync(email);

			if (existingUser != null)
			{
				return new AuthenticationResult
				{
					Success = false,
					Errors = new[] { "User with this email address already exists." }
				};
			}

			if (password != password2)
			{
				return new AuthenticationResult
				{
					Success = false,
					Errors = new[] { "Passwords do not match." }
				};
			}

			if (!IsValidPassword(password))
			{
				return new AuthenticationResult
				{
					Success = false,
					Errors = new[] { "Password does not meet requirements." }
				};
			}

			var user = new User
			{
				Email = email,
				UserName = email,
				Role = RoleTypes.User
			};
			var createdUser = await _userManager.CreateAsync(user, password);

			if (!createdUser.Succeeded)
			{
				return new AuthenticationResult
				{
					Success = false,
					Errors = createdUser.Errors.Select(e => e.Description)
				};
			}

			var newUser = await _userManager.FindByEmailAsync(user.Email);

			// Segment.io Analytics
			Analytics.Client.Identify(newUser.Id.ToString(), new Traits
			{
				{ "email", newUser.Email },
				{ "username", newUser.UserName },
				{ "category", SegmentIOKeys.Categories.Identity },
				{ "action", SegmentIOKeys.Actions.Register },
				{ "createdAt", DateTime.UtcNow }
			});

			var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
			Analytics.Client.Track(newUser.Id.ToString(), SegmentIOKeys.Actions.Register, new Properties()
			{
				{ "emailConfirmToken", confirmationToken }
			});

			_smtpService.SendConfirmationEmail(newUser, confirmationToken);
			return GenerateAuthenticationResultForUser(newUser, ipAddress);
		}

		public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
		{
			// Segment.io Analytics
			Identify(user, SegmentIOKeys.Categories.Identity, SegmentIOKeys.Actions.ConfirmEmail);
			Analytics.Client.Track(user.Id.ToString(), SegmentIOKeys.Actions.ConfirmEmail);

			return await _userManager.ConfirmEmailAsync(user, token);
		}

		public async Task ResendConfirmationAsync(ForgotPasswordRequest request)
		{
			User existingUser;

			if (request.UserId != null) existingUser = await _userManager.FindByIdAsync(request.UserId);
			else existingUser = await _userManager.FindByEmailAsync(request.Email);

			if (existingUser == null)
			{
				throw new NullReferenceException("User doesn't exist");
			}
			var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(existingUser);
			Analytics.Client.Track(existingUser.Id.ToString(), SegmentIOKeys.Actions.Register, new Properties()
			{
				{ "emailConfirmToken", confirmationToken }
			});

			_smtpService.SendConfirmationEmail(existingUser, confirmationToken);
		}

		public async Task<AuthenticationResult> LoginAsync(string email, string password, string ipAddress)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				return new AuthenticationResult
				{
					Success = false,
					Errors = new[] { "User does not exist." }
				};
			}

			var passwordValid = await _userManager.CheckPasswordAsync(user, password);
			if (!passwordValid)
			{
				return new AuthenticationResult
				{
					Success = false,
					Errors = new[] { "Invalid credentials" }
				};
			}

			// Segment.io Analytics
			Identify(user, SegmentIOKeys.Categories.Identity, SegmentIOKeys.Actions.Login);
			Analytics.Client.Track(user.Id.ToString(), SegmentIOKeys.Actions.Login);

			return GenerateAuthenticationResultForUser(user, ipAddress);
		}

		public async Task<AuthenticationResult> OAuthLoginAsync(IUserInfoResult userInfo, string ipAddress)
		{
			var user = await _userManager.FindByEmailAsync(userInfo.Email);
			if (user == null)
			{
				var newUser = new User
				{
					Id = Guid.NewGuid(),
					Email = userInfo.Email,
					UserName = userInfo.Email,
					Role = RoleTypes.User
				};
				var createdUser = await _userManager.CreateAsync(newUser);

				if (!createdUser.Succeeded)
				{
					return new AuthenticationResult
					{
						Success = false,
						Errors = createdUser.Errors.Select(e => e.Description)
					};
				}

				user = await _userManager.FindByEmailAsync(newUser.Email);

				try
				{

					var profile = new PoppinUser(user)
					{
						ProfilePhoto = userInfo.PictureUrl,
						FirstName = userInfo.FirstName,
						LastName = userInfo.LastName
					};
					await _userService.AddUser(profile);
				}
				catch (Exception ex)
				{

				}

				// Segment.io Analytics
				Analytics.Client.Identify(user.Id.ToString(), new Traits
				{
					{ "email", user.Email },
					{ "username", user.UserName },
					{ "category", SegmentIOKeys.Categories.Identity },
					{ "action", SegmentIOKeys.Actions.Register },
					{ "createdAt", DateTime.UtcNow }
				});

				Analytics.Client.Track(user.Id.ToString(), SegmentIOKeys.Actions.Register, new Properties()
				{
					{ "emailConfirmToken", "OAuth-register" }
				});
			}
			else
			{
				// Segment.io Analytics
				Identify(user, SegmentIOKeys.Categories.Identity, SegmentIOKeys.Actions.Login);
				Analytics.Client.Track(user.Id.ToString(), SegmentIOKeys.Actions.Login);
			}

			return GenerateAuthenticationResultForUser(user, ipAddress);
		}

		public async Task<AuthenticationResult> StartPasswordResetAsync(string email, string ipAddress)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				return new AuthenticationResult
				{
					Success = false,
					Errors = new[] { "User does not exist." }
				};
			}

			var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
			// Segment.io Analytics
			Identify(user, SegmentIOKeys.Categories.Identity, SegmentIOKeys.Actions.StartPasswordReset);
			Analytics.Client.Track(user.Id.ToString(), SegmentIOKeys.Actions.StartPasswordReset, new Properties()
			{
				{ "passwordResetToken", resetToken }
			});

			_smtpService.SendPasswordResetEmail(user, resetToken);
			return new AuthenticationResult
			{
				Success = true
			};
		}

		public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password)
		{
			return await _userManager.ResetPasswordAsync(user, token, password);
		}

		public async Task<UserDataResult> GetUserById(string identifier)
		{
			var user = await _userManager.FindByIdAsync(identifier);
			if (user == null)
			{
				return new UserDataResult
				{
					Success = false,
					Errors = new[] { "User does not exist." }
				};
			}
			return new UserDataResult
			{
				User = user,
				Success = true
			};
		}

		public async Task<UserDataResult> GetUserByEmail(string email)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				return new UserDataResult
				{
					Success = false,
					Errors = new[] { "User does not exist." }
				};
			}
			return new UserDataResult
			{
				User = user,
				Success = true
			};
		}

		public async Task<UserListResult> GetUsersById(IEnumerable<string> ids)
		{
			var list = new List<User>();
			foreach (var id in ids)
			{
				var user = await _userManager.FindByIdAsync(id);
				if (user == null)
				{
					return new UserListResult
					{
						Success = false,
						Errors = new[] { "User does not exist." }
					};
				}
				list.Add(user);
			}

			return new UserListResult
			{
				Users = list,
				Success = true
			};
		}

		public async Task<AuthenticationResult> RefreshToken(string token, string refreshToken, string ipAddress)
        {
			var validatedToken = GetPrincipalFromToken(token);
			if (validatedToken == null)
            {
				return new AuthenticationResult
				{
					Errors = new[] { "Invalid Token" }
				};
			}

			var expiresUnix = long.Parse(validatedToken.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Exp).Value);
            if (new DateTime(0, DateTimeKind.Utc).AddSeconds(expiresUnix) > DateTime.UtcNow)
            {
                return new AuthenticationResult
                {
					Success = true,
                    Token = token,
					RefreshToken = refreshToken
                };
            }

            var jti = validatedToken.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
			var storedRefreshToken = await _tokenContext.RefreshTokens.SingleOrDefaultAsync(rt => rt.Token == refreshToken);
			if (storedRefreshToken == null)
			{
				return new AuthenticationResult
				{
					Errors = new[] { "Refresh token not found." }
				};
			}

			if (DateTime.UtcNow > storedRefreshToken.Expires)
			{
				return new AuthenticationResult
				{
					Errors = new[] { "Refresh token expired." }
				};
			}

			if (storedRefreshToken.Revoked != null)
			{
				return new AuthenticationResult
				{
					Errors = new[] { "Refresh token was revoked." }
				};
			}

			if (storedRefreshToken.JwtId != jti)
			{
				return new AuthenticationResult
				{
					Errors = new[] { "Refresh token doesn't match auth token." }
				};
			}

			storedRefreshToken.Revoked = DateTime.UtcNow;
			_tokenContext.RefreshTokens.Update(storedRefreshToken);
			await _tokenContext.SaveChangesAsync();

			var userResult = await GetUserById(storedRefreshToken.UserId.ToString());
			return GenerateAuthenticationResultForUser(userResult.User, ipAddress);
		}

		public void Identify(User user, string category, string action)
		{
			Analytics.Client.Identify(user.Id.ToString(), new Traits
			{
				{ "email", user.Email },
				{ "username", user.UserName },
				{ "category", category },
				{ "action", action }
			});
		}

		public void Identify(string userId, string category, string action)
		{
			var user = _userManager.FindByIdAsync(userId).Result;
			if (user != null)
			{
				Identify(user, category, action);
			}
		}

		public async Task<bool> RevokeToken(string token, string ipAddress)
		{
			var storedToken = await _tokenContext.RefreshTokens.SingleOrDefaultAsync(rt => rt.Token == token);

			// return false if no user found with token
			if (storedToken == null) return false;

			// return false if token is not active
			if (!storedToken.IsActive) return false;

			// revoke token and save
			storedToken.Revoked = DateTime.UtcNow;
			_tokenContext.RefreshTokens.Update(storedToken);
			return true;
		}

		private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
			var handler = new JwtSecurityTokenHandler();
            try
            {
				var principal = handler.ValidateToken(token, _tokenService.TokenValidationParameters, out var securityToken);
				if (!IsValidJWT(securityToken))
                {
					return null;
                }
				return principal;
            }
			catch
            {
				return null;
            }
        }

		private bool IsValidJWT(SecurityToken securityToken)
        {
			return (securityToken is JwtSecurityToken jwtSecurityToken) &&
				jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
					StringComparison.InvariantCultureIgnoreCase);
        }

		private AuthenticationResult GenerateAuthenticationResultForUser(User user, string ipAddress)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
					new Claim(JwtRegisteredClaimNames.Sub, user.Email),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
					new Claim(JwtRegisteredClaimNames.Email, user.Email),
					new Claim("Role", user.Role),
					new Claim("Id", user.Id.ToString())
				}),
				Expires = DateTime.UtcNow.AddDays(1),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);

			return new AuthenticationResult
			{
				Success = true,
				Token = tokenHandler.WriteToken(token),
				RefreshToken = GenerateRefreshToken(user.Id, token, ipAddress).Token
			};
		}

		private RefreshToken GenerateRefreshToken(Guid userId, SecurityToken jwt, string ipAddress)
		{
			using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
			{
				var randomBytes = new byte[64];
				rngCryptoServiceProvider.GetBytes(randomBytes);
				var rt = new RefreshToken
				{
					UserId = userId,
					JwtId = jwt.Id,
					Token = Convert.ToBase64String(randomBytes),
					Expires = DateTime.UtcNow.AddDays(90),
					Created = DateTime.UtcNow,
					CreatedByIp = ipAddress
				};

				_tokenContext.Add(rt);
				_tokenContext.SaveChanges();

				return rt;
			}
		}

		public bool IsValidPassword(string password)
		{
			bool hasUpper = new Regex(@"[A-Z]+").IsMatch(password);
			bool hasLower = new Regex(@"[a-z]+").IsMatch(password);
			bool hasNumber = new Regex(@"[0-9]+").IsMatch(password);
			bool hasSpecial = new Regex(@"[_!@#$%^&*]+").IsMatch(password);
			bool isOnlyAllowed = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[_!@#$%^&*])(?=.{8,24})").IsMatch(password);
			return hasUpper && hasLower && hasNumber && hasSpecial && isOnlyAllowed;
		}
	}
}
