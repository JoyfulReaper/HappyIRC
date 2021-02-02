using System;
using System.Collections.Generic;
using System.Text;

namespace HappyIRCClientLibrary
{
    public enum NumericReply
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

        ERR_NOSUCHCHANNEL = 403, // "<channel name> :No such channel"
        ERR_TOOMANYCHANNELS = 405, // "<channel name> :You have joined too many channels"
        ERR_TOOMANYTARGETS = 407, // "<target> :<error code> recipients. <abort message>"
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
