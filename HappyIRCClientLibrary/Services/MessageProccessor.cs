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

using HappyIRCClientLibrary.Enums;
using HappyIRCClientLibrary.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HappyIRCClientLibrary.Services
{
    public class MessageProccessor : IMessageProccessor
    {
        private readonly IUser user;
        private readonly IServer server;
        private readonly ILogger logger;
        private readonly IChannelService channelService;

        public MessageProccessor(IUser user,
            IServer server,
            ILogger<MessageProccessor> logger,
            IChannelService channelService)
        {
            this.user = user;
            this.server = server;
            this.logger = logger;
            this.channelService = channelService;
        }

        public void ProccessMessage(ServerMessage message)
        {
            if (message.Type == CommandType.Message)
            {
                ProccessPrivmsg(message);
            }
        }

        private void ProccessPrivmsg(ServerMessage message)
        {
            if (message.Nick == user.NickName)
            {
                // Private message to us
            }
            else
            {
                var channel = channelService.Channels.Where(x => x.Name == message.Channel).FirstOrDefault();

                if (channel == null)
                {
                    throw new ArgumentException("Channel cannot be null.", nameof(channel));
                }

                channel.ReceiveMessage(message);
            }
        }
    }
}
