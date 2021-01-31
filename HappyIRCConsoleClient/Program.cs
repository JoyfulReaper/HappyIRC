using System;
using HappyIRCClientLibrary;
using HappyIRCClientLibrary.Config;
using Microsoft.Extensions.DependencyInjection;

namespace HappyIRCConsoleClient
{
    class Program
    {
        private static readonly IServiceProvider Container = new ContainerBuilder().Build();

        static void Main(string[] args)
        {
            IConfig config = new Config();
            var logger = config.GetLogger("IRCClient");

            logger.Error("test");

            IRCClient client = new IRCClient("irc.quakenet.org", 6667, "HappyIRC", "HappyIRC", config);
            client.Connect();
        }
    }
}
