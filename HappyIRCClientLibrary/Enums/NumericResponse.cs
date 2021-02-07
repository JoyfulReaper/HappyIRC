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
        RPL_BOUNCE = 5, // "Try server <server name>, port <port number>"

        RPL_AWAY = 301,
        RPL_USERHOST = 302, // ":*1<reply> *( " " <reply> )"
        RPL_ISON = 303, // ":*1<nick> *( " " <nick> )"
        RPL_UNAWAY = 305, // ":You are no longer marked as being away"
        RPL_NOWAWAY = 306, //":You have been marked as being away"
        RPL_WHOISUSER = 311, // "<nick> <user> <host> * :<real name>"
        RPL_WHOISSERVER = 312, // "<nick> <server> :<server info>"
        RPL_WHOISOPERATOR = 313, // "<nick> :is an IRC operator"
        RPL_WHOWASUSER = 314, // "<nick> <user> <host> * :<real name>"
        RPL_ENDOFWHO = 315, // "<name> :End of WHO list"
        RPL_WHOISIDLE = 317, // "<nick> <integer> :seconds idle"
        RPL_ENDOFWHOIS = 318, // "<nick> :End of WHOIS list"
        RPL_WHOISCHANNELS = 319, // "<nick> :*( ( "@" / "+" ) <channel> " " )"
        RPL_LISTSTART = 321, // Obsolete. Not used
        RPL_LIST = 322, // "<channel> <# visible> :<topic>"
        RPL_LISTEND = 323, // ":End of LIST"
        RPL_CHANNELMODEIS = 324, // "<channel> <mode> <mode params>"
        RPL_UNIQOPIS = 325, // "<channel> <nickname>"
        RPL_NOTOPIC = 331, // "<channel> :No topic is set"
        RPL_TOPIC = 332, // "<channel> :<topic>"
        RPL_INVITING = 341, // "<channel> <nick>"
        RPL_SUMMONING = 342, // "<user> :Summoning user to IRC"
        RPL_INVITELIST = 346, // "<channel> <invitemask>"
        RPL_ENDOFINVITELIST = 347, // "<channel> :End of channel invite list"
        RPL_EXCEPTLIST = 348, // "<channel> <exceptionmask>"
        RPL_ENDOFEXCEPTLIST = 349, // "<channel> :End of channel exception list"
        RPL_VERSION = 351, // "<version>.<debuglevel> <server> :<comments>"
        RPL_WHOREPLY = 352, //  "<channel> <user> <host> <server> <nick> \n ( "H" / "G" > ["*"] [ ( "@" / "+" ) ] \n :<hopcount> <real name>"
        RPL_NAMREPLY = 353, // "( "=" / "*" / "@" ) <channel> \n :[ "@" / "+" ] <nick> *( " " [ "@" / "+" ] <nick> ) 
        RPL_LINKS = 364, // "<mask> <server> :<hopcount> <server info>"
        RPL_ENDOFLINKS = 365, // "<mask> :End of LINKS list"
        RPL_BANLIST = 367, // "<channel> <banmask>"
        RPL_ENDOFBANLIST = 368, // "<channel> :End of channel ban list"
        RPL_ENDOFNAMES = 366, // "<channel> :End of NAMES list"
        RPL_ENDOFWHOWAS = 369, // "<nick> :End of WHOWAS"
        RPL_INFO = 371, // ":<string>"
        RPL_MOTD = 372, // ":- <text>"
        RPL_ENDOFINFO = 374, // ":End of INFO list"
        RPL_MOTDSTART = 375, // ":- <server> Message of the day - "
        RPL_ENDOFMOTD = 376, // ":End of MOTD command"

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
