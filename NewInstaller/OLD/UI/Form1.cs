using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OSA.Bootstrapper
{
    public partial class Form1 : Form
    {
        OSABA bs;

        public Form1(OSABA boot)
        {
            InitializeComponent();
            bs = boot;
            bs.ExecuteMsiMessage += this.ExecuteMsiMessage;
        }

        private string message = "Starting";

        public string Message
        {
            get
            {
                return this.message;
            }

            set
            {
                if (this.message != value)
                {
                    this.message = value;
                    //RaisePropertyChanged("Message");
                }
            }
        }

        private void ExecuteMsiMessage(object sender, ExecuteMsiMessageEventArgs e)
        {
            lock (this)
            {
                textBox1.Text = e.Message;
                //e.Result = this.root.Canceled ? Result.Cancel : Result.Ok;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bs.Engine.Plan(LaunchAction.Install);
        }
    }
}
