using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IRCServerTestClient
{
    public partial class FrmUserName : Form
    {
        private frmMain caller;

        public FrmUserName(frmMain frmMain)
        {
            InitializeComponent();
            this.caller = frmMain;
        }

        private void btnSetNick_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(txtNick.Text) || string.IsNullOrEmpty(txtReal.Text))
            {
                MessageBox.Show("Dude you have to enter some values first...");
                return;
            }

            caller.Nick = txtNick.Text;
            caller.Real = txtReal.Text;

            this.Close();
        }
    }
}
