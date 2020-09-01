using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Poppin.Configuration;
using Poppin.Data;
using Poppin.Interfaces;
using Poppin.Models.Identity;
using Poppin.Services;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Poppin
{
				public class Startup
				{
								public Startup(IConfiguration configuration)
								{
												Configuration = configuration;
								}

								public IConfiguration Configuration { get; }

								// This method gets called by the runtime. Use this method to add services to the container.
								public void ConfigureServices(IServiceCollection services)
								{
												services.AddDbContext<ApplicationDbContext>(options =>
																options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
												services.AddIdentityCore<User>(options => options.SignIn.RequireConfirmedAccount = true)
																.AddEntityFrameworkStores<ApplicationDbContext>();
												//services.AddIdentityServer()
												//				.AddApiAuthorization<ApplicationUser, ApplicationDbContext>();
												//services.AddAuthentication()
												//				.AddIdentityServerJwt();

												// JWT
												var jwtSettings = new JwtSettings();
												Configuration.Bind(nameof(jwtSettings), jwtSettings);
												services.AddSingleton(jwtSettings);
												services.AddScoped<IIdentityService, IdentityService>();
												services.AddAuthentication(a =>
												{
																a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
																a.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
																a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
												}).AddJwtBearer(b =>
												{
																b.SaveToken = true;
																b.TokenValidationParameters = new TokenValidationParameters
																{
																				ValidateIssuerSigningKey = true,
																				IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
																				ValidateIssuer = false,
																				ValidateAudience = false,
																				RequireExpirationTime = false, // needs change eventually
																				ValidateLifetime = true
																};
												});


												// requires using Microsoft.Extensions.Options
												services.Configure<MongoDBSettings>(
																Configuration.GetSection(nameof(MongoDBSettings)));

												services.AddSingleton<IMongoDBSettings, MongoDBSettings>(sp =>
																sp.GetRequiredService<IOptions<MongoDBSettings>>().Value);

												var segmentSettings = new SegmentSettings();
												Configuration.Bind(nameof(SegmentSettings), segmentSettings);
												Segment.Analytics.Initialize(segmentSettings.Key);

												services.AddSingleton<ILocationService, LocationService>();
												services.AddSingleton<IVendorService, VendorService>();
												services.AddSingleton<IUserService, UserService>();
												services.AddSingleton<ILogActionService, LogActionService>(); 
												services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
												services.AddHttpClient<IYelpService, YelpService>();

												services.AddControllersWithViews();
												services.AddRazorPages();

												services.AddSwaggerGen(s =>
												{
																s.SwaggerDoc("v1", new OpenApiInfo() { Title = "Poppin API", Version = "v1" });

																var scheme = new OpenApiSecurityScheme
																{
																				Description = "JWT Authorization header with bearer scheme",
																				Name = "Authorization",
																				In = ParameterLocation.Header,
																				Type = SecuritySchemeType.Http,
																				Scheme = "Bearer"
																};
																s.AddSecurityDefinition("Bearer", scheme);

																var reference = new OpenApiReference
																{
																				Type = ReferenceType.SecurityScheme,
																				Id = "Bearer"
																};
																s.AddSecurityRequirement(new OpenApiSecurityRequirement
																{
																				{
																								new OpenApiSecurityScheme { Reference = reference },
																								new List<string>()
																				}
																});
												});
								}

								// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
								public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
								{
												if (env.IsDevelopment())
												{
																app.UseDeveloperExceptionPage();
																app.UseDatabaseErrorPage();
												}
												else
												{
																app.UseExceptionHandler("/Home/Error");
																// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
																app.UseHsts();
												}

												// Swagger
												var swaggerOptions = new SwaggerOptions();
												Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);
												app.UseSwagger(o => o.RouteTemplate = swaggerOptions.JsonRoute);
												app.UseSwaggerUI(o => o.SwaggerEndpoint(swaggerOptions.UIEndpoint, swaggerOptions.Description));

												// rewrite client-side routes to return index.html
												var options = new RewriteOptions()
																.AddRewrite("^admin", "index.html", skipRemainingRules: true)
																.AddRewrite("^admin/.*", "index.html", skipRemainingRules: true)
																.AddRewrite("^account", "index.html", skipRemainingRules: true)
																.AddRewrite("^account/.*", "index.html", skipRemainingRules: true)
																.AddRewrite("^search", "index.html", skipRemainingRules: true)
																.AddRewrite("^search/.*", "index.html", skipRemainingRules: true);
												app.UseRewriter(options);


												app.UseHttpsRedirection();
												//app.UseDefaultFiles();
												// index.html is the default if a file isn't asked for
												app.UseDefaultFiles(new DefaultFilesOptions()
												{
																DefaultFileNames = new List<string>() { "index.html" },
																FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
																RequestPath = new PathString("")
												});
												app.UseStaticFiles();

												app.UseRouting();

												app.UseAuthentication();
												//app.UseIdentityServer();
												app.UseAuthorization();

												app.UseCors(builder =>
												{
																builder.WithOrigins("http://localhost:4200")
																.AllowAnyHeader()
																.AllowAnyMethod()
																.AllowCredentials();
												});


												app.UseEndpoints(endpoints =>
												{
																endpoints.MapControllerRoute(
																				name: "default",
																				pattern: "{controller=Home}/{action=Index}/{id?}");
																endpoints.MapRazorPages();
												});
								}
				}
}
