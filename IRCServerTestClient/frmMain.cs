using HappyIRCClientLibrary.Enums;
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

namespace HappyIrcTestClient
{
    public partial class frmMain : Form
    {
        public Server Server { get; set; }
        public User User { get; set; }

        private bool showAllMessages = true;
        private readonly IIrcClient client;
        private readonly CommandProccessor commandProccessor;
        private readonly List<Channel> channels = new List<Channel>();

        public frmMain(IIrcClient client)
        {
            InitializeComponent();
            this.client = client;
            commandProccessor = new CommandProccessor(client, channels);
        }

        private async Task Setup()
        {
            frmUser frm = new frmUser(this);

            frm.ShowDialog(this);

            client.Initialize(new Server("irc.quakenet.org", 6667), User);

            client.ReceivedChannelMessage += OnChannelMessage;
            client.ReceivedRawMessage += OnRawMessage;
            client.ReceivedPrivateMessage += OnPrivateMessage;
            await client.Connect();
        }

        private Task OnPrivateMessage(ServerMessage arg)
        {
            throw new NotImplementedException();
        }

        private Task OnRawMessage(ServerMessage message)
        {
            StringBuilder sbParsed = new StringBuilder();
            sbParsed.Append($"Type: {message.Type} ");
            sbParsed.Append($"Prefix: {message.Prefix} ");
            sbParsed.Append($"Channel: {message.Channel} ");
            sbParsed.Append($"Nick: {message.Nick} ");
            sbParsed.Append($"Response Code {message.ResponseCode} ");
            sbParsed.Append($"Command: {message.Command} ");
            foreach (var p in message.Parameters)
            {
                sbParsed.Append($" Parameter: {p} ");
            }
            sbParsed.Append($"Trailing: {message.Trailing} ");
            sbParsed.AppendLine();

            var parsed = sbParsed.ToString();
            if(message.ResponseCode == NumericResponse.RPL_UMODEIS)
            {
                showAllMessages = false;
            }

            if (textBoxParsed.InvokeRequired)
            {
                textBoxParsed.Invoke(new MethodInvoker(delegate { textBoxParsed.AppendText($"Raw: {message.Message}\n"); }));
                textBoxParsed.Invoke(new MethodInvoker(delegate { textBoxParsed.AppendText(parsed); }));
                if(showAllMessages)
                {
                    richTextBoxChat.Invoke(new MethodInvoker(delegate { richTextBoxChat.AppendText(message.Message); }));
                    richTextBoxChat.Invoke(new MethodInvoker(delegate { richTextBoxChat.ScrollToCaret(); }));
                }
            }
            else
            {
                textBoxParsed.AppendText(parsed);
                if (showAllMessages)
                {
                    richTextBoxChat.AppendText(message.Message);
                    richTextBoxChat.ScrollToCaret();
                }
            }

            return Task.CompletedTask;
        }

        private Task OnChannelMessage(ServerMessage message)
        {
            if (richTextBoxChat.InvokeRequired)
            {
                richTextBoxChat.Invoke(new MethodInvoker(delegate { richTextBoxChat.AppendText($"{message.Nick}: {message.Trailing}\n"); }));
                richTextBoxChat.Invoke(new MethodInvoker(delegate { richTextBoxChat.ScrollToCaret(); }));
            }
            else
            {
                richTextBoxChat.AppendText(message.Message);
                richTextBoxChat.ScrollToCaret();
            }

            return Task.CompletedTask;
        }

        private async void frmMain_Load(object sender, EventArgs e)
        {
            await Setup();
        }

        private void btnRaw_Click(object sender, EventArgs e)
        {
            client.SendMessageToServer(textBoxInput.Text + "\r\n");
            richTextBoxChat.AppendText($"Raw message sent: {textBoxInput.Text}\n");
            richTextBoxChat.ScrollToCaret();

            textBoxInput.Text = string.Empty;
        }

        private async void btnCommand_Click(object sender, EventArgs e)
        {
            if (textBoxInput.Text[0] == '/')
            {
                await commandProccessor.ProccessCommand(textBoxInput.Text);
            }
            else
            {
                await channels[0].SendMessage(textBoxInput.Text);
                richTextBoxChat.AppendText($"{User.NickName}: {textBoxInput.Text}\n");
                richTextBoxChat.ScrollToCaret();
            }

            textBoxInput.Text = string.Empty;
        }
    }
}
