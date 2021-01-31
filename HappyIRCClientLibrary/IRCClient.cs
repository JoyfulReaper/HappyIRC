using System;
using System.Collections.Generic;
using System.Text;

namespace HappyIRCClientLibrary
{
    public class IRCClient
    {
        public string Server { get; private set; }
        public int Port { get; private set; }
        public string NickName { get; private set; }
        public string RealName { get; private set; }

        public IRCClient (string server, int port, string nickname, string realname)
        {
            Server = server;
            Port = port;
            NickName = nickname;
            RealName = realname;
        }

        public void Connect()
        {

        }

        private void ListenThread()
        {

        }
    }
}
