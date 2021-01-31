using System;
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

            Console.WriteLine("Hello World!");
            var logger = config.GetLogger("test");

            logger.Error("test");
        }
    }
}
