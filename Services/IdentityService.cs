using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Poppin.Configuration;
using Poppin.Interfaces;
using Poppin.Models.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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
								private readonly UserManager<IdentityUser> _userManager;

								public IdentityService(UserManager<IdentityUser> userMgr, JwtSettings jwtSettings)
								{
												_jwtSettings = jwtSettings;
												_userManager = userMgr;
								}

								public async Task<AuthenticationResult> RegisterAsync(string email, string password, string password2)
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

												var newUser = new IdentityUser
												{
																Email = email,
																UserName = email
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

												return GenerateAuthenticationResultForUser(newUser);
								}

								public async Task<AuthenticationResult> LoginAsync(string email, string password)
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

												return GenerateAuthenticationResultForUser(user);
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
												var list = new List<IdentityUser>();
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

								private AuthenticationResult GenerateAuthenticationResultForUser(User user)
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
												return new AuthenticationResult
												{
																Success = true,
																Token = tokenHandler.WriteToken(token)
												};
								}

								private AuthenticationResult GenerateAuthenticationResultForUser(IdentityUser newUser)
								{
												var tokenHandler = new JwtSecurityTokenHandler();
												var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
												var tokenDescriptor = new SecurityTokenDescriptor
												{
																Subject = new ClaimsIdentity(new[]
																{
																				new Claim(JwtRegisteredClaimNames.Sub, newUser.Email),
																				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
																				new Claim(JwtRegisteredClaimNames.Email, newUser.Email),
																				new Claim("Id", newUser.Id)
																}),
																Expires = DateTime.UtcNow.AddHours(2),
																SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
												};

												var token = tokenHandler.CreateToken(tokenDescriptor);
												return new AuthenticationResult
												{
																Success = true,
																Token = tokenHandler.WriteToken(token)
												};
								}
				}
}
