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
        RPL_USERHOST = 302,
        RPL_ISON = 303,
        RPL_AWAY = 301,
        RPL_UNAWAY = 305,
        RPL_NOWAWAY = 305,
        
        ERR_NICKNAMEINUSE = 433 // "<nick> :Nickname is already in use"
    };
}
