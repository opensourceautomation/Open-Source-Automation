using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OSAInstallCustomActions
{
    public partial class ServerDetails : Form
    {
        Session session;
        public ServerDetails(Session s)
        {
            session = s;
            InitializeComponent();
        }

        private void ServerDetails_Load(object sender, EventArgs e)
        {

        }

        private void q1TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void q2TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void q3TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void q4TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void AllDataRequired()
        {
            if (localhostRadioButton.Checked)
            {
                okButton.Enabled = true;
            }
            else
            {
                if (string.IsNullOrEmpty(q1TextBox.Text) ||
                    string.IsNullOrEmpty(q2TextBox.Text) ||
                    string.IsNullOrEmpty(q3TextBox.Text) ||
                    string.IsNullOrEmpty(q4TextBox.Text))
                {
                    okButton.Enabled = false;
                }
                else
                {
                    okButton.Enabled = true;
                }
            }
        }

        public string ServerIP()
        {
            if (ipRadioButton.Checked)
            {
                return q1TextBox.Text + "." + q2TextBox.Text + "." + q3TextBox.Text + "." + q4TextBox.Text;
            }
            else
            {
                return "localhost";
            }
        }

        private void IPTextChanged(object sender, EventArgs e)
        {
            AllDataRequired();
        }

        private void okButton_Click(object sender, EventArgs e)
        {

        }

        private void localhostRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            AllDataRequired();
        }
    }
}
