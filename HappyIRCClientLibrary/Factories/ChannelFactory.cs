using HappyIRCClientLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HappyIRCClientLibrary.Factories
{
    public class ChannelFactory : IChannelFactory
    {
        private readonly IIrcClient ircClient;

        public ChannelFactory(IIrcClient ircClient)
        {
            this.ircClient = ircClient;
        }

        public Channel Channel(string name, string key)
        {
            //return new Channel(ircClient, name, key);
            return null;
        }
    }
}
