using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace App;

public static class Extensions
{
    public static void AddNonGenericLogger(this ILoggingBuilder loggingBuilder)
    {
        var services = loggingBuilder.Services;
        services.AddSingleton(serviceProvider =>
        {
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            return loggerFactory.CreateLogger("WebJobsCheckerDemo");
        });
    }
    
    public static void AddJsonFile(this IConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.SetBasePath(GetDirectoryPath());
        var environment = Environment.GetEnvironmentVariable("ENVIRONMENT");
        configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        configurationBuilder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
    }

    public static void AddUserSecrets(this IConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddUserSecrets(typeof(Program).Assembly);
    }
    
    private static string GetDirectoryPath()
    {
        try
        {
            return Path.GetDirectoryName(GetAssemblyLocation());
        }
        catch
        {
            return Directory.GetCurrentDirectory();
        }
    }
    
    private static string GetAssemblyLocation() => Assembly.GetExecutingAssembly().Location;
}