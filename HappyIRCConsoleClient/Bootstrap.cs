using HappyIRCClientLibrary.Parsers;
using HappyIRCClientLibrary.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.IO;

namespace HappyIRCConsoleClient
{
    static class Bootstrap
    {
        /// <summary>
        /// Initialize Configuration and Dependency Injection
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns></returns>
        public static ServiceProvider Initialize(string[] args)
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables();
            IConfiguration config = configBuilder.Build();

            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection
                    .AddLogging(configure => configure.AddSerilog())
                    .AddSingleton<IConfiguration>(config)
                    //.AddOptions() https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-5.0
                    .AddTransient<ITcpClient, TcpClient>()
                    .AddTransient<IMessageParser, MessageParser>()
                    .AddTransient<IIrcClient, IrcClient>()
                    .BuildServiceProvider();

            return serviceProvider;
        }
    }
}
