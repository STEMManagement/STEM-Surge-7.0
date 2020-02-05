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
    public partial class Finished : UserControl
    {
        public EventHandler onComplete;

        public bool Advance { get; set; }

        public Finished()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Advance = false;
            onComplete(this, EventArgs.Empty);
        }
    }
}
