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

using HappyIRCClientLibrary.Events;
using HappyIRCClientLibrary.Models;
using HappyIRCClientLibrary.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IRCServerTestClient
{
    public enum Mode { Raw, Simple };

    public partial class frmMain : Form
    {
        public string Nick { get; set; } // setter prob shouldn't be public, but this is just test code!
        public string Real { get; set; } // setter prob shouldn't be public, but this is just test code!
        public Mode Mode { get; set; }

        private readonly IIrcClient client;
        private bool inChannel = false;
        private Channel currentChannel = null;

        public frmMain(IIrcClient client)
        {
            InitializeComponent();
            this.client = client;
        }

        private async Task Setup()
        {
            var frmNick = new FrmUserName(this);
            frmNick.ShowDialog(this);

            User user = new User(Nick, Real);
            Server server = new Server("irc.quakenet.org", 6667);

            client.Initialize(server, user);

            frmConnecting frmCon = new frmConnecting();
            frmCon.Show(this);

            Application.DoEvents();

            client.ServerMessageReceived += MessageReceived;
            await client.Connect();
            frmCon.Close();
        }

        private async void frmMain_Load(object sender, EventArgs e)
        {
            await Setup();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (Mode == Mode.Raw)
            {
                client.SendMessageToServer(txtSendToServer.Text + "\r\n");
                txtServerMessages.AppendText($"YOU SENT: {txtSendToServer.Text}{Environment.NewLine}");
            }
            else
            {
                if(!client.Connected)
                {
                    MessageBox.Show("DUDE YOU ARE NOT CONNECTED!");
                }
                ProccessCommand(txtSendToServer.Text);
            }

            txtSendToServer.Text = string.Empty;
        }

        // Super dumb client with all the logic in the form...
        // Don't do it like this
        private void ProccessCommand(string command)
        {
            if(command.ToUpperInvariant().StartsWith("/PART"))
            {
                var message = string.Empty;
                if (command.Contains(" "))
                {
                    message = command.Substring(command.IndexOf(" "));
                }

                currentChannel.Part(message);

                inChannel = false;
                return;
            }

            if (command.ToUpperInvariant().StartsWith("/JOIN"))
            {
                var channel = new Channel(client, command.Substring(6));
                channel.Join();
                currentChannel = channel;

                return;
            }

            //TODO this doesn't disconnect, look into it after fixing logging
            if (command.ToUpperInvariant().StartsWith("/QUIT"))
            {
                var message = string.Empty;
                if(command.Contains(" "))
                {
                    message = command.Substring(command.IndexOf(" "));
                }

                client.SendMessageToServer($"QUIT {message}");
                return;
            }

            currentChannel.SendMessage(command);
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.Disconnect();
        }

        private void MessageReceived(object sender, ServerMessageReceivedEventArgs e)
        {
            var message = e.ServerMessage.Message;

            if (txtServerMessages.InvokeRequired)
            {
                txtServerMessages.Invoke(new MethodInvoker(delegate { txtServerMessages.AppendText(message + '\n'); }));
            }
            else
            {
                txtServerMessages.AppendText(message + '\n');
            }

            StringBuilder sbParsed = new StringBuilder();
            sbParsed.Append($"Type: {e.ServerMessage.Type} ");
            sbParsed.Append($"Prefix: {e.ServerMessage.Prefix} ");
            sbParsed.Append($"Channel: {e.ServerMessage.Channel} ");
            sbParsed.Append($"Nick: {e.ServerMessage.Nick} ");
            sbParsed.Append($"Response Code {e.ServerMessage.ResponseCode} ");
            sbParsed.Append($"Command: {e.ServerMessage.Command} ");
            foreach (var p in e.ServerMessage.Parameters)
            {
                sbParsed.Append($" Parameter: {p} ");
            }
            sbParsed.Append($"Trailing: {e.ServerMessage.Trailing} ");
            sbParsed.AppendLine();

            var parsed = sbParsed.ToString();

            if (txtParsedMessages.InvokeRequired)
            {
                txtParsedMessages.Invoke(new MethodInvoker(delegate { txtParsedMessages.AppendText(parsed); }));
            }
            else
            {
                txtParsedMessages.AppendText(parsed);
            }
        }
    }
}
