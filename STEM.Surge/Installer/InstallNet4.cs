using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Installer
{
    public partial class InstallNet4 : UserControl
    {
        public EventHandler onComplete;
        public bool Advance { get; set; }

        public InstallNet4()
        {
            InitializeComponent();

            Advance = true;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkLabel1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Advance = false;
            onComplete(this, EventArgs.Empty);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Advance = true;
            onComplete(this, EventArgs.Empty);
        }
    }
}
