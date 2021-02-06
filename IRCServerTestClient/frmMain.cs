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
        private Thread messageReceiverThread; // Just using an event would be better, too lazy to relearn now...
        private MessageReceiver messageReceiver;

        public frmMain(IrcClient client)
        {
            InitializeComponent();
            this.client = client;
        }

        private async Task Setup()
        {
            var frm = new FrmUserName(this);
            frm.ShowDialog(this);

            User user = new User(Nick, Real);
            Server server = new Server("irc.quakenet.org", 6667);

            client.Initialize(server, user);
            await client.Connect();

            messageReceiver = new MessageReceiver(client);

            messageReceiver.messageReceived += message_Received;

            messageReceiverThread = new Thread(new ThreadStart(messageReceiver.MessageListener));
            messageReceiverThread.Start();
        }

        private async void frmMain_Load(object sender, EventArgs e)
        {
            await Setup();
        }

        private void message_Received(Object sender, MessageReceivedEventArgs e)
        {
            var message = e.ServerMessage.Message;
          
            if(txtServerMessages.InvokeRequired)
            {
                txtServerMessages.Invoke(new MethodInvoker(delegate { txtServerMessages.AppendText(message + '\n'); } ));
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            client.SendMessageToServer(txtSendToServer.Text + "\r\n");
            txtSendToServer.Text = string.Empty;
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            messageReceiver.Close();
            client.Disconnect();
            messageReceiverThread.Join();
        }
    }
}
