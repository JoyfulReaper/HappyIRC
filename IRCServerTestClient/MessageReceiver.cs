using HappyIRCClientLibrary;
using HappyIRCClientLibrary.Models;
using System;
using System.Threading;

namespace IRCServerTestClient
{
    public class MessageReceiver
    {
        private IrcClient client;
        public event EventHandler<MessageReceivedEventArgs> messageReceived;
        private bool quitting = false;

        public MessageReceiver(Object ircClient)
        {
            client = ircClient as IrcClient;
            if (client == null)
            {
                throw new ArgumentException("must be ircClient object", nameof(ircClient));
            }
        }

        public void Close()
        {
            quitting = true;
        }

        public void MessageListener()
        {
            while (!quitting)
            {
                while(client.DeQueueMessage(out ServerMessage serverMessage))
                {
                    OnMessageReceived(new MessageReceivedEventArgs(serverMessage));
                }
                Thread.Sleep(50);
            }
        }

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            messageReceived?.Invoke(this, e);
        }

    }
}

