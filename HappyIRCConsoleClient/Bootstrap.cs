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
                    .AddOptions()
                    .AddTransient<ITcpClient, TcpClient>()
                    .AddTransient<IMessageParser, MessageParser>()
                    .AddTransient<IIrcClient, IrcClient>()
                    .BuildServiceProvider();

            return serviceProvider;
        }
    }
}
