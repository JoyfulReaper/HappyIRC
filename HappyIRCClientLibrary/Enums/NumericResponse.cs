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

        RPL_TRACELINK = 200,
        RPL_TRACECONNECTION = 201,
        RPL_TRACEHANDSHAKE = 202,
        RPL_TRACEUNKNOWN = 203,
        RPL_TRACEOPERATOR = 204,
        RPL_TRACEUSER = 205,
        RPL_TRACESERVER = 206,
        RPL_TRACESERVICE = 207,
        RPL_TRACENEWTYPE = 208,
        RPL_TRACECLASS = 209,
        RPL_TRACECONNECT = 210,
        RPL_STATSLINKINFO = 211,
        RPL_STATSCOMMANDS = 212,
        RPL_ENDOFSTATS = 219,
        RPL_UMODEIS = 221,
        RPL_SERVLIST = 234,
        RPL_SERVLISTEND = 235,
        RPL_STATSUPTIME = 242,
        RPL_STATUSOLINE = 243,
        RPL_LUSERCLIENT = 251,
        RPL_LUSEROP = 252,
        RPL_LUSERUNKNOWN = 253,
        RPL_LUSERCHANNELS = 254,
        RPL_LUSERME = 255,
        RPL_ADMINME = 256,
        RPL_ADMINLOC1 = 257,
        RPL_ADMINLOC2 = 258,
        RPL_ADMINEMAIL = 259,
        RPL_TRACELOG = 261,
        RPL_TRACEEND = 262,
        RPL_TRYAGAIN = 263, //"<command> :Please wait a while and try again."

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
        RPL_YOUREOPER = 381, // ":You are now an IRC operator"
        RPL_REHASHING = 382, // "<config file> :Rehashing"
        RPL_YOURESERVICE = 383, // "You are service <servicename>"
        RPL_TIME = 391, // "<server> :<string showing server's local time>"
        RPL_USERSTART = 392, // :UserID   Terminal  Host"
        RPL_USERS = 393, // ":<username> <ttyline> <hostname>"
        RPL_ENDOFUSERS = 394, // ":End of users"
        RPL_NOUSERS = 395, // ":Nobody logged in"

        ERR_NOSUCHNICK = 401, // "<nickname> :No such nick/channel"
        ERR_NOSUCHSERVER = 402,
        ERR_NOSUCHCHANNEL = 403, // "<channel name> :No such channel"
        ERR_CANNOTSENDTOCHAN = 404, // "<channel name> :Cannot send to channel"
        ERR_TOOMANYCHANNELS = 405, // "<channel name> :You have joined too many channels"
        ERR_WASNOSUCHNICK = 406,
        ERR_TOOMANYTARGETS = 407, // "<target> :<error code> recipients. <abort message>"
        ERR_NOSUCHSERVICE = 408,
        ERR_NOORGIN = 409,
        ERR_NORECIPIENT = 411, // ":No recipient given (<command>)"
        ERR_NOTEXTTOSEND = 412, // ":No text to send"
        ERR_NOTOPLEVEL = 413, // "<mask> :No toplevel domain specified"
        ERR_WILDTOPLEVEL = 414, // "<mask> :Wildcard in toplevel domain"
        ERR_BADMASK = 415,
        ERR_UNKNOWNCOMMAND = 421,
        ERR_NOMOTD = 422,
        ERR_NOADMININFO = 423,
        ERR_FILEERROR = 424,
        ERR_NONICKNAMEGIVEN = 431,
        ERR_ERRONEUSNICKNAME = 432,
        ERR_NICKNAMEINUSE = 433, // "<nick> :Nickname is already in use"
        ERR_NICKCOLLISION = 436,
        ERR_UNAVAILRESOURCE = 437, // "<nick/channel> :Nick/channel is temporarily unavailable"
        ERR_USERNOTINCHANNEL = 441,
        ERR_NOTONCHANNEL = 442, // "<channel> :You're not on that channel" 
        ERR_USERONCHANNEL = 443,
        ERR_NOLOGIN = 444,
        ERR_SUMMONDISABLED = 445,
        ERR_USERDISABLED = 446,
        ERR_NOTREGISTERED = 451,
        ERR_NEEDMOREPARAMS = 461, // "<command> :Not enough parameters"
        ERR_ALREADYREGISTERED = 462,
        ERR_NOPERMFORHOST = 463,
        ERR_PASSWDMISMATCH = 464,
        ERR_YOUREBANNEDCREEP = 465, // ":You are banned from this server" Returned after an attempt to connect and register yourself with a server which has been setup to explicitly deny connections to you.
        ERR_YOUWILLBEBANNED = 466, // Sent by a server to a user to inform that access to the server will soon be denied.
        ERR_KEYSET = 467,
        ERR_CHANNELISFULL = 471, // "<channel> :Cannot join channel (+l)"
        ERR_UNKNOWNMODE = 472,
        ERR_INVITEONLYCHAN = 473, // "<channel> :Cannot join channel (+i)"
        ERR_BANNEDFROMCHAN = 474, // "<channel> :Cannot join channel (+b)"
        ERR_BADCHANNELKEY = 475,  // "<channel> :Cannot join channel (+k)"
        ERR_BADCHANMASK = 476,   // "<channel> :Bad Channel Mask"
        ERR_NOCHANMODES = 477,
        ERR_BANLISTFULL = 478,
        ERR_NOPRIVILEGES = 481,
        ERR_CHANOPPRIVSNEEDED = 482,
        ERR_CANTKILLSERVER = 483,
        ERR_RESTRICTED = 484,
        ERR_UNIQOPPRIVSNEEDED = 485,
        ERR_NOOPERHOST = 491,

        ERR_UMODEUNKNOWNFLAG = 501,
        ERR_USERSDONTMATCH = 502,

        // Reserved Numerics
        RPL_SERVICEINFO = 231,
        RPL_ENDOFSERVICES = 232,
        RPL_SERVICE = 233,
        RPL_NONE = 300,
        RPL_WHOISCHANOP = 316,
        RPL_KILLDONE = 361,
        RPL_CLOSING = 362,
        RPL_CLOSEEND = 363,
        RPL_INFOSTART = 373,
        RPL_MYPORTIS = 384,
        RPL_STATSCLINE = 213,
        RPL_STATSILINE = 215,
        RPL_STATSQLINE = 217,
        RPL_STATSVLINE = 240,
        RPL_STATSHLINE = 244,
        RPL_STATSPING = 246,
        RPL_STATSDLINE = 250,
        RPL_STATSNLINE = 214,
        RPL_STATSKLINE = 216,
        RPL_STATSYLINE = 218,
        RPL_STATSLLINE = 241,
        RPL_STATSSLINE = 244,
        RPL_STATSBLINE = 247,
        ERR_NOSERVICEHOST = 492
    };
}
