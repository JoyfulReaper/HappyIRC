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

namespace HappyIRCClientLibrary.Enums
{
    public enum NumericResponse
    {
        // 001 to 004 indicates the connection was successful
        INVALID = 0,

        RPL_WELCOME = 1, // "Welcome to the Internet Relay Network <nick>!<user>@<host>"
        RPL_YOURHOST = 2, // "Your host is <servername>, running version <ver>"
        RPL_CREATED = 3, // "This server was created <date>"
        RPL_MYINFO = 4, // "<servername> <version> <available user modes> < available channel modes>"
        RPL_BOUNCE = 5,
        RPL_AWAY = 301,
        RPL_USERHOST = 302,
        RPL_ISON = 303,
        RPL_UNAWAY = 305,
        RPL_NOWAWAY = 306,
        RPL_TOPIC = 332, // "<channel> :<topic>"

        ERR_NOSUCHNICK = 401, // "<nickname> :No such nick/channel"
        ERR_NOSUCHCHANNEL = 403, // "<channel name> :No such channel"
        ERR_CANNOTSENDTOCHAN = 404, // "<channel name> :Cannot send to channel"
        ERR_TOOMANYCHANNELS = 405, // "<channel name> :You have joined too many channels"
        ERR_TOOMANYTARGETS = 407, // "<target> :<error code> recipients. <abort message>"
        ERR_NORECIPIENT = 411, // ":No recipient given (<command>)"
        ERR_NOTEXTTOSEND = 412, // ":No text to send"
        ERR_NOTOPLEVEL = 413, // "<mask> :No toplevel domain specified"
        ERR_WILDTOPLEVEL = 414, // "<mask> :Wildcard in toplevel domain"
        ERR_NICKNAMEINUSE = 433, // "<nick> :Nickname is already in use"
        ERR_UNAVAILRESOURCE = 437, // "<nick/channel> :Nick/channel is temporarily unavailable"
        ERR_NOTONCHANNEL = 442, // "<channel> :You're not on that channel"
        ERR_NEEDMOREPARAMS = 461, // "<command> :Not enough parameters"
        ERR_BANNEDFROMCHAN = 474, // "<channel> :Cannot join channel (+b)"
        ERR_INVITEONLYCHAN = 473, // "<channel> :Cannot join channel (+i)"
        ERR_BADCHANNELKEY = 475,  // "<channel> :Cannot join channel (+k)"
        ERR_CHANNELISFULL = 471, // "<channel> :Cannot join channel (+l)"
        ERR_BADCHANMASK = 476,   // "<channel> :Bad Channel Mask"
    };
}
