using HappyIRCClientLibrary;
using HappyIRCClientLibrary.Parsers;
using HappyIRCClientLibrary.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Threading.Tasks;

namespace HappyIRCConsoleClient
{
    static class Bootstrap
    {
        public static async Task<ServiceProvider> Initialize(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection
                    .AddLogging(configure => configure.AddSerilog())
                    .AddTransient<IIrcClient, IrcClient>()
                    .AddTransient<IMessageParser, MessageParser>()
                    .AddTransient<ITcpService, TcpService>()
                    .BuildServiceProvider();

            return serviceProvider;
        }
    }
}
