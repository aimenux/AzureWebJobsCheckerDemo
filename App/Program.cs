using System;
using System.Threading.Tasks;
using Lib.Configuration;
using Lib.Extensions;
using Lib.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace App;

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

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile();
                config.AddUserSecrets();
                config.AddEnvironmentVariables();
                config.AddCommandLine(args);
            })
            .ConfigureLogging((context, builder) =>
            {
                builder.AddNonGenericLogger();
                builder.AddSimpleConsole(options =>
                {
                    options.TimestampFormat = "[HH:mm:ss:fff] ";
                    options.ColorBehavior = LoggerColorBehavior.Disabled;
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
}