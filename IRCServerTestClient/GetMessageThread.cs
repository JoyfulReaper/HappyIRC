using HappyIRCClientLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRCServerTestClient
{
    public class GetMessageThread
    {
        public void MessageListener(Object ircClient)
        {
            IrcClient client = ircClient as IrcClient;
            if(client == null)
            {
                throw new ArgumentException("I wrote this error checking just for you! Now do it right!");
            }


        }
    }
}
