using HappyIRCClientLibrary.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace HappyIRCClientLibrary.Services
{
    public class ChannelService : IChannelService
    {
        public List<Channel> Channels { get; private set; } = new List<Channel>();

        private readonly ILogger logger;

        public ChannelService(ILogger<ChannelService> logger)
        {
            this.logger = logger;
        }

        public void AddChannel(Channel channel)
        {
            if (channel == null)
            {
                logger.LogError("Attemped to add a null channel");
                throw new ArgumentException("Channel cannot be null", nameof(channel));
            }

            Channels.Add(channel);
        }

        public void RemoveChannel(Channel channel)
        {
            if (channel == null)
            {
                logger.LogError("Attemped to remove a null channel");
                throw new ArgumentException("Channel cannot be null", nameof(channel));
            }

            Channels.Remove(channel);
        }
    }
}
