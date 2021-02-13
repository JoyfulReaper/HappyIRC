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

/* TODO Important notes:
 * 
 * Because of IRC's Scandinavian origin, the characters {}|^ are
 * considered to be the lower case equivalents of the characters []\~,
 * respectively. This is a critical issue when determining the
 * equivalence of two nicknames or channel names. 
 */

using HappyIRCClientLibrary.Services;
using Serilog;
using System;
using System.Text;

namespace HappyIRCClientLibrary.Models
{
    /// <summary>
    /// Represents a Channel
    /// See RFC 2812 3.2
    /// </summary>
    public class Channel : IChannel
    {
        #region Properties
        public string Name
        {
            get { return name; }
            set
            {
                if (NameIsValid(value))
                {
                    name = value;
                }
                else
                {
                    throw new ArgumentException("Channel name is not valid", nameof(value));
                }
            }
        }

        public string Key { get; set; }
        #endregion Properties

        #region Private Data
        private readonly IIrcClient client;
        private static readonly char[] validStartingChars = { '&', '#', '+', '!' };

        /// <summary>
        /// Name of the channel
        /// </summary>
        private string name;
        #endregion Private Data

        #region Constructors
        public Channel(IIrcClient client, string name, string key = "")
        {
            this.client = client;
            this.Name = name;
            this.Key = key;
        }
        #endregion Constructors

        #region Public Methods
        /// <summary>
        /// Send a message to this channel
        /// </summary>
        /// <param name="message">The message to send</param>
        public void SendMessage(string message)
        {
            Log.Debug("Sending {message} to {channel}", message, Name);
            client.SendMessageToServer($"PRIVMSG {Name} :{message}\r\n");
        }

        /// <summary>
        /// Called when a message is sent to this channel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ReceiveMessage(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Join the Channel
        /// RFC 2812 3.2.1 Join message
        /// http://www.geekshed.net/2012/03/using-channel-keys/ Channel Keys
        /// </summary>
        /// <returns></returns>
        public bool Join()
        {
            /*
           TODO: Error checking
           Possible Numeric Replies:
           ERR_NEEDMOREPARAMS ERR_BANNEDFROMCHAN
           ERR_INVITEONLYCHAN ERR_BADCHANNELKEY
           ERR_CHANNELISFULL ERR_BADCHANMASK
           ERR_NOSUCHCHANNEL ERR_TOOMANYCHANNELS
           ERR_TOOMANYTARGETS ERR_UNAVAILRESOURCE
           RPL_TOPIC
            */

            StringBuilder joinBuilder = new StringBuilder($"JOIN {Name}");
            if (!string.IsNullOrEmpty(Key))
            {
                joinBuilder.Append($" {Key}");
            }
            joinBuilder.Append("\r\n");

            client.SendMessageToServer(joinBuilder.ToString());

            return true;
        }

        /// <summary>
        /// Part the channel
        /// </summary>
        /// <returns></returns>
        public bool Part()
        {
            return Part(string.Empty);
        }

        /// <summary>
        /// Part the channel
        /// </summary>
        /// <param name="message">The parting message</param>
        /// <returns></returns>
        public bool Part(string message)
        {
            /* Possible replies
            TODO: Error checking
            ERR_NEEDMOREPARAMS              ERR_NOSUCHCHANNEL
            ERR_NOTONCHANNEL 
            */

            StringBuilder partBuilder = new StringBuilder($"PART {name}");
            if(!string.IsNullOrEmpty(message))
            {
                partBuilder.Append($" :{message}");
            }
            partBuilder.Append("\r\n");

            client.SendMessageToServer(partBuilder.ToString());

            return true;
        }

        /// <summary>
        /// Check to see if a channel name is valid per See RFC 2812 1.3
        /// </summary>
        /// <param name="name"></param>
        /// <returns>true if valid, false if not</returns>
        public static bool NameIsValid(string name)
        {
            //Must not be longer than 50 characters
            if (name.Length > 50)
            {
                return false;
            }

            //Must not contain spaces, commas, or control G (ASCII 7)
            if (name.Contains(' ') || name.Contains(',') || name.Contains('\a'))
            {
                return false;
            }

            // Must start with a specific charcter
            foreach (char c in validStartingChars)
            {
                if (name[0] == c)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion Public Methods
    }
}
