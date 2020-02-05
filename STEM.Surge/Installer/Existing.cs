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
    public partial class Existing : UserControl
    {
        public EventHandler onComplete;
        public Operation SelectedAction { get; set; }

        public enum Operation { Exit, Uninstall, Update }

        public Existing()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (updateRB.Checked)
            {
                SelectedAction = Operation.Update;
                onComplete(this, EventArgs.Empty);
            }
            else if (uninstallRB.Checked)
            {
                SelectedAction = Operation.Uninstall;
                onComplete(this, EventArgs.Empty);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SelectedAction = Operation.Exit;
            onComplete(this, EventArgs.Empty);
        }
    }
}
