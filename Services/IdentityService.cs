using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Poppin.Configuration;
using Poppin.Interfaces;
using Poppin.Models.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Poppin.Services
{
				/// <summary>
				/// Identity Service based on tutorial by Nick Chapsas
				/// https://www.youtube.com/channel/UCrkPsvLGln62OMZRO6K-llg
				/// </summary>
				public class IdentityService : IIdentityService
				{
								private readonly JwtSettings _jwtSettings;
								private readonly UserManager<User> _userManager;

								public IdentityService(UserManager<User> userMgr, JwtSettings jwtSettings)
								{
												_jwtSettings = jwtSettings;
												_userManager = userMgr;
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

												var newUser = new User
												{
																Email = email,
																UserName = email,
																Role = RoleTypes.User
												};
												var createdUser = await _userManager.CreateAsync(newUser, password);

												if (!createdUser.Succeeded)
												{
																return new AuthenticationResult
																{
																				Success = false,
																				Errors = createdUser.Errors.Select(e => e.Description)
																};
												}

												return GenerateAuthenticationResultForUser(newUser, ipAddress);
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

												return GenerateAuthenticationResultForUser(user, ipAddress);
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
								public async Task<AuthenticationResult> RefreshToken(string token, string ipAddress)
								{
												var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

												// return null if no user found with token
												if (user == null) return null;

												var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

												// return null if token is no longer active
												if (!refreshToken.IsActive) return null;

												// replace old refresh token with a new one and save
												var newRefreshToken = GenerateRefreshToken(ipAddress);

												// update user in database with token audit trail
												//refreshToken.Revoked = DateTime.UtcNow;
												//refreshToken.RevokedByIp = ipAddress;
												//refreshToken.ReplacedByToken = newRefreshToken.Token;
												user.RefreshTokens.Add(newRefreshToken);
												user.RefreshTokens.Remove(refreshToken);
												_userManager.UpdateAsync(user);

												return GenerateAuthenticationResultForUser(user, ipAddress);
								}

								public async Task<bool> RevokeToken(string token, string ipAddress)
								{
												var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

												// return false if no user found with token
												if (user == null) return false;

												var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

												// return false if token is not active
												if (!refreshToken.IsActive) return false;

												// revoke token and save
												user.RefreshTokens.Remove(refreshToken);
												_userManager.UpdateAsync(user);

												return true;
								}


								private AuthenticationResult GenerateAuthenticationResultForUser(User user, string ipAddress)
								{											
												return new AuthenticationResult
												{
																Success = true,
																Token = GenerateJWT(user),
																RefreshToken = GenerateRefreshToken(ipAddress).Token
												};
								}

								private string GenerateJWT(User user)
								{
												var tokenHandler = new JwtSecurityTokenHandler();
												var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
												var claims = new Claim[5];
												claims.Append(new Claim(JwtRegisteredClaimNames.Sub, user.Email));
												claims.Append(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
												claims.Append(new Claim(JwtRegisteredClaimNames.Email, user.Email));
												claims.Append(new Claim("Role", user.Role));
												claims.Append(new Claim("Id", user.Id));

												var tokenDescriptor = new SecurityTokenDescriptor
												{
																Subject = new ClaimsIdentity(claims),
																Expires = DateTime.UtcNow.AddHours(2),
																SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
												};

												var token = tokenHandler.CreateToken(tokenDescriptor);
												return tokenHandler.WriteToken(token);
								}

								private RefreshToken GenerateRefreshToken(string ipAddress)
								{
												using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
												{
																var randomBytes = new byte[64];
																rngCryptoServiceProvider.GetBytes(randomBytes);
																return new RefreshToken
																{
																				Token = Convert.ToBase64String(randomBytes),
																				Expires = DateTime.UtcNow.AddHours(8),
																				Created = DateTime.UtcNow,
																				CreatedByIp = ipAddress
																};
												}
								}
				}
}
