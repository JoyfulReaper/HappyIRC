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
using System.Collections.Generic;

namespace HappyIRCClientLibrary.Models
{
    /// <summary>
    /// Represents a Message from the IRC server
    /// </summary>
    public class ServerMessage : IServerMessage
    {
        public CommandType Type { get; private set; }
        public List<string> Parameters { get; private set; }
        public string Message { get; private set; }
        public string Nick { get; private set; }
        public string Command { get; private set; }
        public string Channel
        {
            get
            {
                if (Parameters.Count < 1)
                {
                    return string.Empty;
                }

                return Parameters[0];
            }
        }
        public string Trailing { get; private set; }
        public string Prefix { get; private set; }
        public NumericResponse ResponseCode { get; private set; }

        // TODO Maybe re-arange the parameters so the order makes more sense
        public ServerMessage(CommandType type, string command, List<string> parameters, string trailing, string message, NumericResponse response, string nick, string prefix)
        {
            Prefix = prefix;
            Trailing = trailing;
            Type = type;
            Command = command;
            Nick = nick;
            Parameters = parameters;
            Message = message;
            ResponseCode = response;
        }
    }
}
