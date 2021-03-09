using HappyIRCClientLibrary;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;

namespace HappyIRCConsoleClient
{
    static class Bootstrap
    {
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables();
            IConfiguration config = configBuilder.Build();

            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging(configure => configure.AddSerilog())
                    .AddSingleton<IConfiguration>(config)
                    .AddHappyIrcClient()
                    .AddSingleton<IApplication, Application>();
                });
        }


        /// <summary>
        /// Initialize Configuration and Dependency Injection
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns></returns>
        //public static ServiceProvider Initialize(string[] args)
        //{
        //    IConfigurationBuilder configBuilder = new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        //        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        //        .AddEnvironmentVariables();
        //    IConfiguration config = configBuilder.Build();

        //    var serviceCollection = new ServiceCollection();
        //    var serviceProvider = serviceCollection
        //            .AddLogging(configure => configure.AddSerilog())
        //            .AddSingleton<IConfiguration>(config)
        //            .AddHappyIrcClient()
        //            .BuildServiceProvider();

        //    return serviceProvider;
        //}
    }
}
