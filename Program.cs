using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using Poppin.Data;
using System;

namespace Poppin
{
				public class Program
				{
								public static void Main(string[] args)
								{
												var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
												try
												{
																logger.Debug("init main");
																var host = CreateHostBuilder(args).Build();

																using (var scope = host.Services.CreateScope())
																{
																				var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
																				context.Database.Migrate();
																}

																host.Run();
												}
												catch (Exception exception)
												{
																//NLog: catch setup errors
																logger.Error(exception, "Stopped program because of exception");
																throw;
												}
												finally
												{
																// Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
																NLog.LogManager.Shutdown();
												}
								}

								public static IHostBuilder CreateHostBuilder(string[] args) =>
												Host.CreateDefaultBuilder(args)
																.ConfigureWebHostDefaults(webBuilder =>
																{
																				webBuilder.UseStartup<Startup>();
																}).ConfigureLogging(logging =>
																{
																				logging.ClearProviders();
																				logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
																}).UseNLog();  // NLog: Setup NLog for Dependency injection;
				}
}
