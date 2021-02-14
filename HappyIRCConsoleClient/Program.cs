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
        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-5.0

            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                //.WriteTo.Console()
                .CreateLogger();

            Log.Logger.Information("HappyIRCConsoleClient Starting");

            //var cts = new CancellationTokenSource();

            try
            {
                await Run(args);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An unhandeled exception occured.");
                Console.WriteLine("An unhandeled exception occured." );
            }
            finally
            {
                Log.CloseAndFlush();
                Environment.Exit(-1);
            }

        }

        private static async Task Run(string[] args)
        {
            Server server = new Server("irc.quakenet.org", 6667);
            User user = new User("HappyIRC", "The Happiest IRC");

            var serviceProvider = Bootstrap.Initialize(args, server, user);
            var ircClient = serviceProvider.GetRequiredService<IIrcClient>();

            if (ircClient != null)
            {
                await ircClient.Connect();

                Channel win95 = ircClient.GetChannel("#Windows95");
                await win95.Join();
                await win95.SendMessage("Hello IRC world!");

                await Task.Delay(35000);
                await win95.Part("Goodbye IRC world!");

                //while (true)
                //{
                //    await Task.Delay(120000);
                //}
                await Task.Delay(15000);
                await ircClient.Disconnect();
            }
            else
            {
                Console.WriteLine("ircClient is null!");
            }
        }

        private static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables();
        }
    }
}
