using HappyIRCClientLibrary.Parsers;
using HappyIRCClientLibrary.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HappyIRCClientLibrary
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add required services to DI Container.
        /// NOTE: I originally planned to allow the ability to connect to multiple servers
        /// However this lead to some issues with the lifetime of objects.
        /// TODO: Revisit allowing connections to multiple servers.
        /// </summary>
        /// <param name="services">The service collection container</param>
        /// <param name="server">The server to connect to</param>
        /// <param name="user">The user to connect as</param>
        /// <returns></returns>
        public static IServiceCollection AddHappyIrcClient(this IServiceCollection services, IServer server, IUser user)
        {
            services.AddTransient(x => server)
           .AddTransient(x => user)
           //.AddOptions() https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-5.0
           .AddTransient<ITcpClient, TcpClient>()
           .AddSingleton<IMessageParser, MessageParser>()
           .AddTransient<IIrcClient, IrcClient>();
            return services;
        }
    }
}