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
using System;
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
			services.AddDbContextPool<ApplicationDbContext>(options =>
				options.UseMySql(Configuration.GetConnectionString("MySQLConnection"),
					mySqlOptions => {
						mySqlOptions.EnableRetryOnFailure(
							maxRetryCount: 10,
							maxRetryDelay: TimeSpan.FromSeconds(10),
							errorNumbersToAdd: null
						);
					})
					.EnableSensitiveDataLogging()
			).AddDbContextPool<TokenDbContext>(options =>
				options.UseMySql(Configuration.GetConnectionString("MySQLConnection"),
					mySqlOptions => {
						mySqlOptions.EnableRetryOnFailure(
							maxRetryCount: 10,
							maxRetryDelay: TimeSpan.FromSeconds(10),
							errorNumbersToAdd: null
						);
					})
					.EnableSensitiveDataLogging()
			);

			services.AddIdentityCore<User>(options => options.SignIn.RequireConfirmedAccount = true)
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			services.AddLogging();
			ConfigNLog();

			// JWT
			var jwtSettings = new JwtSettings();
			Configuration.Bind(nameof(jwtSettings), jwtSettings);

			services.Configure<JwtSettings>(
				Configuration.GetSection(nameof(JwtSettings)));
			services.AddTransient<IJwtSettings, JwtSettings>(sp =>
				sp.GetRequiredService<IOptions<JwtSettings>>().Value);

			services.Configure<OAuthSettings>(
				Configuration.GetSection(nameof(OAuthSettings)));
			services.AddTransient<IOAuthSettings, OAuthSettings>(sp =>
				sp.GetRequiredService<IOptions<OAuthSettings>>().Value);

			services.Configure<HERESettings>(
				Configuration.GetSection(nameof(HERESettings)));
			services.AddTransient<IHERESettings, HERESettings>(sp =>
				sp.GetRequiredService<IOptions<HERESettings>>().Value);

			services.Configure<BestTimeSettings>(
				Configuration.GetSection(nameof(BestTimeSettings)));
			services.AddTransient<IBestTimeSettings, BestTimeSettings>(sp =>
				sp.GetRequiredService<IOptions<BestTimeSettings>>().Value);

			services.Configure<BigDataCloudSettings>(
				Configuration.GetSection(nameof(BigDataCloudSettings)));
			services.AddTransient<IBigDataCloudSettings, BigDataCloudSettings>(sp =>
				sp.GetRequiredService<IOptions<BigDataCloudSettings>>().Value);

			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			services.AddHttpClient<FBAuthService>();
			services.AddHttpClient<GoogleAuthService>();
			services.AddHttpClient<IHEREGeocoder, HEREGeocoder>();
			services.AddHttpClient<IBestTimeService, BestTimeService>();
			services.AddHttpClient<IBigDataCloudService, BigDataCloudService>();
			services.AddTransient<IOAuthHandler, OAuthHandler>();
			services.AddTransient<IIdentityService, IdentityService>();
			services.AddTransient<ITokenService, TokenService>();

			services.AddAuthentication(a =>
			{
				a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				a.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
				a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(b =>
			{
				b.SaveToken = true;
				b.TokenValidationParameters = new TokenService(jwtSettings).TokenValidationParameters;
			});

			services.AddAuthorization(opts =>
			{
				opts.AddPolicy("Admin", policy => policy.RequireClaim("Role", RoleTypes.Admin));
			});


			// requires using Microsoft.Extensions.Options
			services.Configure<MongoDBSettings>(
				Configuration.GetSection(nameof(MongoDBSettings)));
			services.AddTransient<IMongoDBSettings, MongoDBSettings>(sp =>
				sp.GetRequiredService<IOptions<MongoDBSettings>>().Value);
			services.AddHostedService<ConfigureMongoDbIndexesService>();

			var segmentSettings = new SegmentSettings();
			Configuration.Bind(nameof(SegmentSettings), segmentSettings);
			Segment.Analytics.Initialize(segmentSettings.Key);

			services.Configure<Office365Settings>(Configuration.GetSection(nameof(Office365Settings)));
			services.AddTransient<ISmtpService, SmtpService>();

			services.AddTransient<ILocationService, LocationService>();
			services.AddTransient<IVendorService, VendorService>();
			services.AddTransient<IUserService, UserService>();
			services.AddTransient<ILogActionService, LogActionService>(); 
			services.AddHttpClient<IYelpService, YelpService>();

			services.AddControllersWithViews();
			services.AddRazorPages();

			services.AddSwaggerGen(s =>
			{
				s.SwaggerDoc("v1", new OpenApiInfo() { Title = "Poppin API", Version = "v1" });
				s.OperationFilter<SwaggerOperationFilter>();

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
				app.UseDeveloperExceptionPage()
					.UseDatabaseErrorPage();
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


			//app.UseHttpsRedirection();
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

			//handle client side routes
			app.Run(async (context) =>
			{
				context.Response.ContentType = "text/html";
				await context.Response.SendFileAsync(Path.Combine(env.WebRootPath, "index.html"));
			});
		}

		public void ConfigNLog()
		{
			IConfigurationRoot config = new ConfigurationBuilder()
				.AddJsonFile(path: "appsettings.json").Build();
			NLog.Extensions.Logging.ConfigSettingLayoutRenderer.DefaultConfiguration = config;
		}
	}
}
