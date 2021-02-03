using System;
using System.Collections.Generic;
using System.Text;

namespace HappyIRCClientLibrary.Models
{
    public class User
    {
        public string NickName { get; set; }
        public string RealName { get; set; }

        public User(string nickName, string realName)
        {
            NickName = nickName;
            RealName = realName;
        }

        public void SendMessage(string message)
        {
            throw new NotImplementedException();
        }

        public void ReceiveMessage(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
