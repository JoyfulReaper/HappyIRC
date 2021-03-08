
namespace HappyIrcTestClient
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.richTextBoxChat = new System.Windows.Forms.RichTextBox();
            this.textBoxInput = new System.Windows.Forms.TextBox();
            this.textBoxParsed = new System.Windows.Forms.TextBox();
            this.btnCommand = new System.Windows.Forms.Button();
            this.btnRaw = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBoxChat
            // 
            this.richTextBoxChat.Location = new System.Drawing.Point(12, 12);
            this.richTextBoxChat.Name = "richTextBoxChat";
            this.richTextBoxChat.Size = new System.Drawing.Size(1589, 514);
            this.richTextBoxChat.TabIndex = 0;
            this.richTextBoxChat.Text = "";
            // 
            // textBoxInput
            // 
            this.textBoxInput.Location = new System.Drawing.Point(12, 800);
            this.textBoxInput.Name = "textBoxInput";
            this.textBoxInput.Size = new System.Drawing.Size(1359, 25);
            this.textBoxInput.TabIndex = 0;
            // 
            // textBoxParsed
            // 
            this.textBoxParsed.Location = new System.Drawing.Point(12, 526);
            this.textBoxParsed.Multiline = true;
            this.textBoxParsed.Name = "textBoxParsed";
            this.textBoxParsed.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxParsed.Size = new System.Drawing.Size(1589, 268);
            this.textBoxParsed.TabIndex = 2;
            // 
            // btnCommand
            // 
            this.btnCommand.Location = new System.Drawing.Point(1377, 800);
            this.btnCommand.Name = "btnCommand";
            this.btnCommand.Size = new System.Drawing.Size(111, 25);
            this.btnCommand.TabIndex = 1;
            this.btnCommand.Text = "Send Command";
            this.btnCommand.UseVisualStyleBackColor = true;
            this.btnCommand.Click += new System.EventHandler(this.btnCommand_Click);
            // 
            // btnRaw
            // 
            this.btnRaw.Location = new System.Drawing.Point(1494, 800);
            this.btnRaw.Name = "btnRaw";
            this.btnRaw.Size = new System.Drawing.Size(107, 25);
            this.btnRaw.TabIndex = 2;
            this.btnRaw.Text = "Send Raw";
            this.btnRaw.UseVisualStyleBackColor = true;
            this.btnRaw.Click += new System.EventHandler(this.btnRaw_Click);
            // 
            // frmMain
            // 
            this.AcceptButton = this.btnCommand;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1613, 837);
            this.Controls.Add(this.btnRaw);
            this.Controls.Add(this.btnCommand);
            this.Controls.Add(this.textBoxParsed);
            this.Controls.Add(this.textBoxInput);
            this.Controls.Add(this.richTextBoxChat);
            this.Name = "frmMain";
            this.Text = "HappyIRC Test Client";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBoxChat;
        private System.Windows.Forms.TextBox textBoxInput;
        private System.Windows.Forms.TextBox textBoxParsed;
        private System.Windows.Forms.Button btnCommand;
        private System.Windows.Forms.Button btnRaw;
    }
}