using HappyIRCClientLibrary;
using HappyIRCClientLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IRCServerTestClient
{
    // this code blows

    public partial class frmMain : Form
    {
        public string Nick { get; set; } // setter prob shouldn't be public, but this is just test code!
        public string Real { get; set; } // setter prob shouldn't be public, but this is just test code!

        private IrcClient client;
        private Thread getMessages; // Just using an event would be better, too lazy to relearn now...

        public frmMain()
        {
            InitializeComponent();
            
        }

        private async Task Setup()
        {
            var frm = new FrmUserName(this);
            frm.ShowDialog(this);

            User user = new User(Nick, Real);
            Server server = new Server("irc.quakenet.com", 6667);

            client.Initialize(server, user);
            await client.Connect();
        }

        private async void frmMain_Load(object sender, EventArgs e)
        {
            await Setup();
        }
    }
}
