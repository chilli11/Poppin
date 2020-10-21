﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Poppin.Configuration;
using Poppin.Interfaces;
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
								private readonly JwtSettings _jwtSettings;
								private readonly UserManager<User> _userManager;
								private readonly ISmtpService _smtpService;

								public IdentityService(UserManager<User> userMgr, JwtSettings jwtSettings, ISmtpService smtpService)
								{
												_jwtSettings = jwtSettings;
												_userManager = userMgr;
												_smtpService = smtpService;
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

												var newUser = new User
												{
																Id = Guid.NewGuid().ToString(),
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

												// Segment.io Analytics
												Analytics.Client.Identify(newUser.Id, new Traits
												{
																{ "email", newUser.Email },
																{ "username", newUser.UserName },
																{ "category", SegmentIOKeys.Categories.Identity },
																{ "action", SegmentIOKeys.Actions.Register },
																{ "createdAt", DateTime.UtcNow }
												});

												var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
												Analytics.Client.Track(newUser.Id, SegmentIOKeys.Actions.Register, new Properties()
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
												Analytics.Client.Track(user.Id, SegmentIOKeys.Actions.ConfirmEmail);

												return await _userManager.ConfirmEmailAsync(user, token);
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
												Analytics.Client.Track(user.Id, SegmentIOKeys.Actions.Login);

												return GenerateAuthenticationResultForUser(user, ipAddress);
								}

								public async Task<AuthenticationResult> OAuthLoginAsync(string email, string ipAddress)
								{
												var user = await _userManager.FindByEmailAsync(email);
												if (user == null)
												{
																user = new User
																{
																				Id = Guid.NewGuid().ToString(),
																				Email = email,
																				UserName = email,
																				Role = RoleTypes.User
																};
																var createdUser = await _userManager.CreateAsync(user);

																if (!createdUser.Succeeded)
																{
																				return new AuthenticationResult
																				{
																								Success = false,
																								Errors = createdUser.Errors.Select(e => e.Description)
																				};
																}

																// Segment.io Analytics
																Analytics.Client.Identify(user.Id, new Traits
																{
																				{ "email", user.Email },
																				{ "username", user.UserName },
																				{ "category", SegmentIOKeys.Categories.Identity },
																				{ "action", SegmentIOKeys.Actions.Register },
																				{ "createdAt", DateTime.UtcNow }
																});

																Analytics.Client.Track(user.Id, SegmentIOKeys.Actions.Register, new Properties()
																{
																				{ "emailConfirmToken", "OAuth-register" }
																});
												}
												else
												{
																// Segment.io Analytics
																Identify(user, SegmentIOKeys.Categories.Identity, SegmentIOKeys.Actions.Login);
																Analytics.Client.Track(user.Id, SegmentIOKeys.Actions.Login);
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
												Analytics.Client.Track(user.Id, SegmentIOKeys.Actions.StartPasswordReset, new Properties()
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
								public void Identify(IdentityUser user, string category, string action)
								{
												Analytics.Client.Identify(user.Id, new Traits
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

												var tokenDescriptor = new SecurityTokenDescriptor
												{
																Subject = new ClaimsIdentity(new[]
																{
																				new Claim(JwtRegisteredClaimNames.Sub, user.Email),
																				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
																				new Claim(JwtRegisteredClaimNames.Email, user.Email),
																				new Claim("Role", user.Role),
																				new Claim("Id", user.Id)
																}),
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
