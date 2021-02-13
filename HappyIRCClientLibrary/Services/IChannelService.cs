using HappyIRCClientLibrary.Models;
using System.Collections.Generic;

namespace HappyIRCClientLibrary.Services
{
    public interface IChannelService
    {
        List<Channel> Channels { get; }

        void AddChannel(Channel channel);
        void RemoveChannel(Channel channel);
    }
}