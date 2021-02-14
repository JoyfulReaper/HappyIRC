
namespace HappyIrcTestClient
{
    partial class frmUser
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
            this.textBoxNick = new System.Windows.Forms.TextBox();
            this.textBoxReal = new System.Windows.Forms.TextBox();
            this.lblNick = new System.Windows.Forms.Label();
            this.lblReal = new System.Windows.Forms.Label();
            this.btnSet = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxNick
            // 
            this.textBoxNick.Location = new System.Drawing.Point(132, 52);
            this.textBoxNick.Name = "textBoxNick";
            this.textBoxNick.Size = new System.Drawing.Size(160, 25);
            this.textBoxNick.TabIndex = 0;
            this.textBoxNick.Text = "HappyIRC";
            // 
            // textBoxReal
            // 
            this.textBoxReal.Location = new System.Drawing.Point(132, 93);
            this.textBoxReal.Name = "textBoxReal";
            this.textBoxReal.Size = new System.Drawing.Size(160, 25);
            this.textBoxReal.TabIndex = 1;
            this.textBoxReal.Text = "Mr. Happy";
            // 
            // lblNick
            // 
            this.lblNick.AutoSize = true;
            this.lblNick.Location = new System.Drawing.Point(26, 55);
            this.lblNick.Name = "lblNick";
            this.lblNick.Size = new System.Drawing.Size(75, 17);
            this.lblNick.TabIndex = 2;
            this.lblNick.Text = "Nick Name:";
            // 
            // lblReal
            // 
            this.lblReal.AutoSize = true;
            this.lblReal.Location = new System.Drawing.Point(26, 96);
            this.lblReal.Name = "lblReal";
            this.lblReal.Size = new System.Drawing.Size(75, 17);
            this.lblReal.TabIndex = 3;
            this.lblReal.Text = "Real Name:";
            // 
            // btnSet
            // 
            this.btnSet.Location = new System.Drawing.Point(122, 135);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(101, 38);
            this.btnSet.TabIndex = 2;
            this.btnSet.Text = "Set Name";
            this.btnSet.UseVisualStyleBackColor = true;
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // frmUser
            // 
            this.AcceptButton = this.btnSet;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 185);
            this.Controls.Add(this.btnSet);
            this.Controls.Add(this.lblReal);
            this.Controls.Add(this.lblNick);
            this.Controls.Add(this.textBoxReal);
            this.Controls.Add(this.textBoxNick);
            this.Name = "frmUser";
            this.Text = "Set User Info";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxNick;
        private System.Windows.Forms.TextBox textBoxReal;
        private System.Windows.Forms.Label lblNick;
        private System.Windows.Forms.Label lblReal;
        private System.Windows.Forms.Button btnSet;
    }
}