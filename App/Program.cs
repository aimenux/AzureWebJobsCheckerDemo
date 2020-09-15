using System;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Lib.Configuration;
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

            var logger = host.Services.GetService<ILogger>();
            var settings = host.Services.GetService<IOptions<Settings>>().Value;
            var webjobInfoProvider = host.Services.GetService<IWebJobInfoProvider>();
            var webjobStatus = await webjobInfoProvider.GetWebJobStatusAsync(settings);

            logger.LogInformation("Webjob '{name}' has status '{status}'", settings.Name, webjobStatus);

            Console.WriteLine("Press any key to exit !");
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
                    builder.AddConfiguration(context.Configuration.GetSection("Logging"));
                })
                .ConfigureServices((context, services) =>
                {
                    services.Configure<Settings>(context.Configuration.GetSection(nameof(Settings)));
                    services.AddHttpClient<IWebJobInfoProvider, WebJobInfoProvider>((provider, client) =>
                    {
                        var settings = provider.GetService<IOptions<Settings>>().Value;
                        client.BaseAddress = new Uri(settings.Url);
                        client.DefaultRequestHeaders.Authorization = GetAuthenticationHeader(settings);
                    });
                });

        private static AuthenticationHeaderValue GetAuthenticationHeader(Settings settings)
        {
            var bytes = Encoding.ASCII.GetBytes($"{settings.UserName}:{settings.Password}");
            var base64 = Convert.ToBase64String(bytes);
            return new AuthenticationHeaderValue("Basic", base64);
        }

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
