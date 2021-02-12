
namespace IRCServerTestClient
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtSendToServer = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtServerMessages = new System.Windows.Forms.TextBox();
            this.txtParsedMessages = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtSendToServer
            // 
            this.txtSendToServer.Location = new System.Drawing.Point(12, 865);
            this.txtSendToServer.Name = "txtSendToServer";
            this.txtSendToServer.Size = new System.Drawing.Size(1267, 25);
            this.txtSendToServer.TabIndex = 0;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(1285, 865);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(121, 25);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "SEND!!!!";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // txtServerMessages
            // 
            this.txtServerMessages.Location = new System.Drawing.Point(12, 12);
            this.txtServerMessages.Multiline = true;
            this.txtServerMessages.Name = "txtServerMessages";
            this.txtServerMessages.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtServerMessages.Size = new System.Drawing.Size(1389, 510);
            this.txtServerMessages.TabIndex = 2;
            this.txtServerMessages.WordWrap = false;
            // 
            // txtParsedMessages
            // 
            this.txtParsedMessages.Location = new System.Drawing.Point(12, 528);
            this.txtParsedMessages.Multiline = true;
            this.txtParsedMessages.Name = "txtParsedMessages";
            this.txtParsedMessages.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtParsedMessages.Size = new System.Drawing.Size(1394, 331);
            this.txtParsedMessages.TabIndex = 3;
            this.txtParsedMessages.WordWrap = false;
            // 
            // frmMain
            // 
            this.AcceptButton = this.btnSend;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1416, 898);
            this.Controls.Add(this.txtParsedMessages);
            this.Controls.Add(this.txtServerMessages);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtSendToServer);
            this.Name = "frmMain";
            this.Text = "Testing IRC Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSendToServer;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtServerMessages;
        private System.Windows.Forms.TextBox txtParsedMessages;
    }
}

