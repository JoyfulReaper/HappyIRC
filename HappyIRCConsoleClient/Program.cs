/*
MIT License

Copyright(c) 2021 Kyle Givler
https://github.com/JoyfulReaper

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.IO;
using System.Threading.Tasks;
using HappyIRCClientLibrary.Models;
using HappyIRCClientLibrary.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace HappyIRCConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // TODO look into configuration/appsettings.json more
            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.iconfiguration?view=dotnet-plat-ext-5.0

            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger.Information("Application Starting");

            var serviceProvider = await Bootstrap.Initialize(args);
            var ircClient = serviceProvider.GetRequiredService<IIrcClient>();

            //var cts = new CancellationTokenSource();

            if (ircClient != null)
            {
                Server server = new Server("irc.quakenet.org", 6667);
                User user = new User("HappyIRC", "The Happiest IRC");

                ircClient.Initialize(server, user);
                await ircClient.Connect();

                Channel win95 = new Channel(ircClient, "#Windows95");
                win95.SendMessage("Hello IRC world!");

                await Task.Delay(5000);
                ircClient.Disconnect();
            }
            else
            {
                Console.WriteLine("ircClient is null!");
            }
        }

        //private static IHostBuilder CreateHostBuilder(string [] args)
        //{
        //    return Host.CreateDefaultBuilder()
        //        .ConfigureServices((context, services) =>
        //        {
        //            services
        //            .AddTransient<IApplicationService, ApplicationService>()
        //            .AddTransient<IIrcClient, IrcClient>()
        //            .AddTransient<IMessageParser, MessageParser>()
        //            .AddHostedService<TcpService>();
        //        })
        //        .UseSerilog();
        //}

        private static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables();
        }
    }
}
