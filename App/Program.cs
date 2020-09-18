using System;
using System.IO;
using System.Threading.Tasks;
using Lib.Configuration;
using Lib.Extensions;
using Lib.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace App
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();
            var settings = host.Services.GetService<IOptions<Settings>>().Value;
            var webjobInfoProvider = host.Services.GetService<IWebJobInfoProvider>();

            foreach (var webjob in settings.WebJobs)
            {
                ConsoleColor.Yellow.WriteLine($"Checking infos for webjob '{webjob}'");
                var webjobInfo = await webjobInfoProvider.GetWebJobInfoAsync(webjob);
                ConsoleColor.Green.WriteLine(webjobInfo);
            }

            ConsoleColor.Gray.WriteLine("Press any key to exit !");
            Console.ReadKey();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(config =>
                {
                    config.AddCommandLine(args);
                    config.AddEnvironmentVariables();
                    config.SetBasePath(Directory.GetCurrentDirectory());
                })
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddCommandLine(args);
                    config.AddEnvironmentVariables();
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    var environment = Environment.GetEnvironmentVariable("ENVIRONMENT");
                    config.AddJsonFile("appsettings.json", false, true);
                    config.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
                })
                .ConfigureLogging((context, builder) =>
                {
                    builder.AddNonGenericLogger();
                    builder.AddConsole(options =>
                    {
                        options.DisableColors = false;
                        options.TimestampFormat = "[HH:mm:ss:fff] ";
                    });
                    builder.AddFilter("System.Net.Http", LogLevel.Warning);
                    builder.AddFilter("Microsoft.Extensions.Http", LogLevel.Warning);
                    builder.AddConfiguration(context.Configuration.GetSection("Logging"));
                })
                .ConfigureServices((context, services) =>
                {
                    services.Configure<Settings>(context.Configuration.GetSection(nameof(Settings)));
                    services.AddTransient<IWebJobInfoProvider, WebJobInfoProvider>();
                    services.AddHttpClient();
                });

        private static void AddNonGenericLogger(this ILoggingBuilder loggingBuilder)
        {
            var services = loggingBuilder.Services;
            services.AddSingleton(serviceProvider =>
            {
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                return loggerFactory.CreateLogger("WebJobsCheckerDemo");
            });
        }
    }
}
