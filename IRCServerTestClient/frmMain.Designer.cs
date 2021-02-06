
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
            this.SuspendLayout();
            // 
            // txtSendToServer
            // 
            this.txtSendToServer.Location = new System.Drawing.Point(2, 751);
            this.txtSendToServer.Name = "txtSendToServer";
            this.txtSendToServer.Size = new System.Drawing.Size(1267, 25);
            this.txtSendToServer.TabIndex = 0;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(1275, 751);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(121, 25);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "SEND!!!!";
            this.btnSend.UseVisualStyleBackColor = true;
            // 
            // txtServerMessages
            // 
            this.txtServerMessages.Location = new System.Drawing.Point(12, 12);
            this.txtServerMessages.Multiline = true;
            this.txtServerMessages.Name = "txtServerMessages";
            this.txtServerMessages.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtServerMessages.Size = new System.Drawing.Size(1384, 733);
            this.txtServerMessages.TabIndex = 2;
            this.txtServerMessages.WordWrap = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1408, 788);
            this.Controls.Add(this.txtServerMessages);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtSendToServer);
            this.Name = "frmMain";
            this.Text = "TEST ALL THE THINGS!";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSendToServer;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtServerMessages;
    }
}

