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

using System;

namespace HappyIRCClientLibrary.Models
{
    /// <summary>
    /// Represents a Channel
    /// See RFC 2812 1.3: https://tools.ietf.org/html/rfc2812#section-1.3
    /// </summary>
    public class Channel
    {
        private readonly IIrcClient client;

        private static readonly char[] validStartingChars = { '&', '#', '+', '!' };

        public Channel(IIrcClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Name of the channel
        /// </summary>
        private string Name;

        public string MyProperty
        {
            get { return Name; }
            set
            {
                
            }
        }


        /// <summary>
        /// Send a message to this channel
        /// </summary>
        /// <param name="message">The message to send</param>
        public void SendMessage(string message)
        {
            client.SendMessageToServer("\r\n");
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
        /// Check to see if a channel name is valid per See RFC 2812 1.3
        /// </summary>
        /// <param name="name"></param>
        /// <returns>true if valid, false if not</returns>
        public static bool NameIsValid(string name)
        {
            // Must start with a specific charcter
            foreach(char c in validStartingChars)
            {
                if (name[0] == c)
                {
                    return true;
                }
            }

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

            return true;
        }
    }
}
