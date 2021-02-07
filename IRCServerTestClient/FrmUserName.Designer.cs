
namespace IRCServerTestClient
{
    partial class FrmUserName
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
            this.btnSetNick = new System.Windows.Forms.Button();
            this.txtNick = new System.Windows.Forms.TextBox();
            this.txtReal = new System.Windows.Forms.TextBox();
            this.lblNick = new System.Windows.Forms.Label();
            this.lblReal = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnSetNick
            // 
            this.btnSetNick.Location = new System.Drawing.Point(80, 121);
            this.btnSetNick.Name = "btnSetNick";
            this.btnSetNick.Size = new System.Drawing.Size(75, 33);
            this.btnSetNick.TabIndex = 0;
            this.btnSetNick.Text = "DO IT!";
            this.btnSetNick.UseVisualStyleBackColor = true;
            this.btnSetNick.Click += new System.EventHandler(this.btnSetNick_Click);
            // 
            // txtNick
            // 
            this.txtNick.Location = new System.Drawing.Point(80, 21);
            this.txtNick.Name = "txtNick";
            this.txtNick.Size = new System.Drawing.Size(134, 25);
            this.txtNick.TabIndex = 1;
            this.txtNick.Text = "HappyIRC";
            // 
            // txtReal
            // 
            this.txtReal.Location = new System.Drawing.Point(80, 71);
            this.txtReal.Name = "txtReal";
            this.txtReal.Size = new System.Drawing.Size(134, 25);
            this.txtReal.TabIndex = 2;
            this.txtReal.Text = "Mr. Happy";
            // 
            // lblNick
            // 
            this.lblNick.AutoSize = true;
            this.lblNick.Location = new System.Drawing.Point(19, 29);
            this.lblNick.Name = "lblNick";
            this.lblNick.Size = new System.Drawing.Size(33, 17);
            this.lblNick.TabIndex = 3;
            this.lblNick.Text = "Nick";
            // 
            // lblReal
            // 
            this.lblReal.AutoSize = true;
            this.lblReal.Location = new System.Drawing.Point(19, 79);
            this.lblReal.Name = "lblReal";
            this.lblReal.Size = new System.Drawing.Size(33, 17);
            this.lblReal.TabIndex = 4;
            this.lblReal.Text = "Real";
            // 
            // FrmUserName
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(245, 196);
            this.Controls.Add(this.lblReal);
            this.Controls.Add(this.lblNick);
            this.Controls.Add(this.txtReal);
            this.Controls.Add(this.txtNick);
            this.Controls.Add(this.btnSetNick);
            this.Name = "FrmUserName";
            this.Text = "Set Nick";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSetNick;
        private System.Windows.Forms.TextBox txtNick;
        private System.Windows.Forms.TextBox txtReal;
        private System.Windows.Forms.Label lblNick;
        private System.Windows.Forms.Label lblReal;
    }
}