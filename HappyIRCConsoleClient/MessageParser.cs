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

using HappyIRCClientLibrary.Config;
using log4net;
using System.Collections.Generic;


namespace HappyIRCConsoleClient
{
    public class MessageParser
    {
        private readonly string nick; // User's nickname
        private readonly IConfig config;

        private ILog log;

        public MessageParser(string nick, IConfig config)
        {
            this.nick = nick;
            this.config = config;
            log = config.GetLogger("ParseMessage");
        }

        public void ParseMessage(string message)
        {
            string trailing;
            string prefix;
            string command;
            string nick;
            List<string> paramerters = new List<string>();
            int prefixEnd = -1;
            int trailingStart = message.IndexOf(" :");

            // Extract the nickname
            if (message.IndexOf("!") >= 0)
            {
                nick = message.Substring(1, message.IndexOf("!"));
            }

            // Extract the prefix
            if(message.StartsWith(":"))
            {
                prefixEnd = message.IndexOf(" ");
                prefix = message.Substring(1, prefixEnd);
            }

            // Exrtract trailing part of message
            if (trailingStart >=0)
            {
                trailing = message.Substring(trailingStart + 2);
            }
            else
            {
                trailingStart = message.Length;
            }

            // Extract command and parameters
            string[] commandAndParameters = message.Substring(prefixEnd + 1, trailingStart).Split(" ");
            command = commandAndParameters[0];

            if(commandAndParameters.Length > 1)
            {
                for(int i = 1; i < commandAndParameters.Length; i++)
                {
                    paramerters.Add(commandAndParameters[i]);
                }
            }

            MessageType type = GetType(command, paramerters);
            // Do something with the message
            
        }

        private MessageType GetType(string command, List<string> parameters)
        {
            MessageType type = MessageType.Unknown;

            if(command == "PRIVMSG")
            {
                if(parameters[0] == nick)
                {
                    type = MessageType.PrivateMessage;
                }
                else
                {
                    type = MessageType.ChannelMessage;
                }
            }

            if(int.TryParse(command, out int _))
            {
                type = MessageType.NumericResponse;
            }

            return type;
        }
    }
}
