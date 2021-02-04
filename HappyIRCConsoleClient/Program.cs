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

using System.Threading.Tasks;
using HappyIRCClientLibrary;
using HappyIRCClientLibrary.Models;
using Microsoft.Extensions.DependencyInjection;

namespace HappyIRCConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = ContainerBuilder.BuildContainer();

            Server server = new Server("irc.quakenet.org", 6667);
            User user = new User("HappyIRC", "Happy IRC!");

            IIrcClient client = serviceProvider.GetRequiredService<IIrcClient>();
            client.Initialize(server, user);
            client.Connect();

            //Thread.Sleep(15000); // wait for it to connect... we should use an event later
            //client.Join("#windows95");

            //Thread.Sleep(1000);
            //client.SendMessage("#windows95", "I swear I'm not a bot!");

            //Thread.Sleep(40000);
            //client.Part("#windows95");
            //Thread.Sleep(1000);
            //client.Disconnect();
        }
    }
}
