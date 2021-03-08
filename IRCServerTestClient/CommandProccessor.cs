using HappyIRCClientLibrary.Models;
using HappyIRCClientLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyIrcTestClient
{
    public class CommandProccessor
    {
        private readonly IIrcClient client;
        private readonly List<Channel> channels;

        public CommandProccessor(IIrcClient client, List<Channel> channels)
        {
            this.client = client;
            this.channels = channels;
        }

        public async Task ProccessCommand(string command)
        {
            if(command.ToUpperInvariant().StartsWith("/JOIN"))
            {
                await ProccessJoin(command.Substring(6));
                return;
            }
            if (command.ToUpperInvariant().StartsWith("/PART"))
            {
                await ProccessPart(command.Substring(6));
                return;
            }
        }

        private async Task ProccessPart(string chan)
        {
            var channel = channels.Where(x => x.Name == chan).FirstOrDefault();
            if(channel != null)
            {
                await channel.Part();
            }
        }

        private async Task<Channel> ProccessJoin(string chan)
        {
            Channel channel;

            var split = chan.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (split.Length == 2)
            {
                channel = client.GetChannel(chan, split[1]);
            }
            else
            {
                channel = client.GetChannel(chan);
            }

            channels.Add(channel);

            await channel.Join();
            return channel;
        }
    }
}
