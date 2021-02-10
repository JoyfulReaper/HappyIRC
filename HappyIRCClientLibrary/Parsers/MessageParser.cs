﻿/*
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

// TODO This will need some cleanup or probably a re-write.
// See https://tools.ietf.org/html/rfc2812#section-2.3.1

using HappyIRCClientLibrary.Enums;
using HappyIRCClientLibrary.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace HappyIRCClientLibrary.Parsers
{
    /// <summary>
    /// Parse the IRC server'c reply
    /// </summary>
    public class MessageParser : IMessageParser
    {
        // TODO clean up/re-write/improve this class

        private readonly string clientNick; // User's nickname
        private readonly ILogger<MessageParser> log;

        public MessageParser(IIrcClient ircClient,
            ILogger<MessageParser> log)
        {
            this.clientNick = ircClient.User.NickName;
            this.log = log;
        }

        public ServerMessage ParseMessage(string message)
        {
            // Example:
            // :userNick!~userNick@userhost PRIVMSG #Channel :message

            string trailing = string.Empty; // Trailing text (ex message sent to a channel)
            string prefix = string.Empty; // The prefix (nick and hostname)
            string command = string.Empty; // The command
            string nick = string.Empty; // Nick associated with command

            List<string> parameters = new List<string>();
            int prefixEnd = message.Length; // ending index of the prefix
            int trailingStart = message.IndexOf(" :"); // Index where the trailing message begins

            // Extract the nick
            if (message.IndexOf('!') > 1)
            {
                int nickEnd = message.IndexOf('!');
                nick = message.Substring(1, nickEnd - 1);
            }

            // Extract the prefix
            if (message.StartsWith(':'))
            {
                if (message.Contains(' '))
                {
                    prefixEnd = message.IndexOf(' ');
                }

                prefix = message.Substring(1, prefixEnd - 1);
            }

            // Exrtract trailing part of message
            if (trailingStart >= 0)
            {
                trailing = message.Substring(trailingStart + 2);
            }
            else
            {
                trailingStart = message.Length;
            }

            // Extract command and parameters
            if (message.Length != 0)
            {
                string[] commandAndParameters = message.Substring(prefixEnd).Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (commandAndParameters.Length > 1)
                {
                    command = commandAndParameters[0];
                }

                if (commandAndParameters.Length > 1)
                {
                    for (int i = 1; i < commandAndParameters.Length; i++)
                    {
                        if (commandAndParameters[i].StartsWith(":"))
                        {
                            break;
                        }

                        parameters.Add(commandAndParameters[i]);
                    }
                }
            }

            ServerMessage serverMessage = CreateServerMessage(command, nick, parameters, trailing, message, prefix);

            // Build the debug message
            StringBuilder sb = new StringBuilder();
            sb.Append($"Nick: {serverMessage.Nick} Type: {serverMessage.Type} Prefix: {prefix} Command: {command}");
            foreach (var p in parameters)
            {
                sb.Append($" Parameter: {p} ");
            }
            sb.Append($" trailing: {trailing}");
            var debugMessage = sb.ToString().Replace("\r", "").Replace("\n", "");
            log.LogDebug("{message}", debugMessage);

            return serverMessage;
        }

        //TODO I think Factory Pattern can improve this, look into that
        private ServerMessage CreateServerMessage(string command, string nick, List<string> parameters, string trailing, string message, string prefix)
        {
            CommandType type = CommandType.Unknown;
            NumericResponse numericReply = NumericResponse.INVALID;

            if (command == "PRIVMSG")
            {
                if (parameters[0] == clientNick)
                {
                    type = CommandType.PrivateMessage;
                }
                else
                {
                    type = CommandType.ChannelMessage;
                }
            }

            if (int.TryParse(command, out int reply))
            {
                type = CommandType.NumericReply;
                log.LogDebug("Found numeric reply: {reply}", reply);


                if (Enum.IsDefined(typeof(NumericResponse), reply))
                {
                    numericReply = (NumericResponse)reply;
                    log.LogDebug("I know the numeric reply as: {numericReply}", numericReply);
                }
            }

            ServerMessage serverMessage = new ServerMessage(type, command, parameters, trailing, message, numericReply, nick, prefix);
            return serverMessage;
        }
    }
}
