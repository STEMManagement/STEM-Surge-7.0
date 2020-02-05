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
    public partial class Agreement : UserControl
    {
        public EventHandler onComplete;

        public bool Advance { get; set; }

        public Agreement()
        {
            InitializeComponent();

            button1.Enabled = false;

            richTextBox1.Rtf = global::Installer.Properties.Resources.Agreement;
            richTextBox1.ReadOnly = true;

            Advance = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            button1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Advance = true;
            onComplete(this, EventArgs.Empty);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Advance = false;
            onComplete(this, EventArgs.Empty);
        }
    }
}
