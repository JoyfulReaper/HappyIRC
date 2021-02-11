using HappyIRCClientLibrary.Parsers;
using HappyIRCClientLibrary.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Threading.Tasks;

namespace HappyIRCConsoleClient
{
    static class Bootstrap
    {
        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>IHostBuilder/returns>
        public static IHostBuilder Initialize(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services
                    .AddTransient<ITcpClient, TcpClient>()
                    .AddTransient<IMessageParser, MessageParser>()
                    .AddTransient<IIrcClient, IrcClient>();
                })
                .UseSerilog();
        }

        //public static async Task<ServiceProvider> Initialize(string[] args)
        //{
        //    var serviceCollection = new ServiceCollection();
        //    var serviceProvider = serviceCollection
        //            .AddLogging(configure => configure.AddSerilog())
        //            .AddOptions()
        //            .AddTransient<ITcpClient, TcpClient>()
        //            .AddTransient<IMessageParser, MessageParser>()
        //            .AddTransient<IIrcClient, IrcClient>()
        //            .BuildServiceProvider();

        //    return serviceProvider;
        //}
    }
}
