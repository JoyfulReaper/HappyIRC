using HappyIRCClientLibrary.Models;
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
    public partial class frmUser : Form
    {
        private readonly frmMain mainFrm;

        public frmUser(frmMain mainFrm)
        {
            InitializeComponent();
            this.mainFrm = mainFrm;
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            mainFrm.User = new User(textBoxNick.Text, textBoxReal.Text);
            this.Close();
        }
    }
}
