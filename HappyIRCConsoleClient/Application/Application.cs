using HappyIRCClientLibrary.Models;
using HappyIRCClientLibrary.Services;
using System;
using System.Threading.Tasks;

namespace HappyIRCConsoleClient
{
    public class Application : IApplication
    {
        private readonly IIrcClient ircClient;

        public Application(IIrcClient ircClient)
        {
            this.ircClient = ircClient;
        }

        public async Task Run()
        {
            Server server = new Server("irc.quakenet.org", 6667);
            User user = new User("HappyIRC", "The Happiest IRC");

            ircClient.Initialize(server, user);

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
    }
}