using HappyIRCClientLibrary.Models;

namespace IRCServerTestClient
{
    public class MessageReceivedEventArgs : System.EventArgs
    {
        public readonly ServerMessage ServerMessage;

        public MessageReceivedEventArgs(ServerMessage serverMessage)
        {
            ServerMessage = serverMessage;
        }
    }
}
