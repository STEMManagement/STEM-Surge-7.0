using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STEM.Surge.ControlPanel
{
    public partial class NewStaticGroup : Form
    {
        public string GroupName { get; set; }

        public NewStaticGroup()
        {
            InitializeComponent();

            GroupName = null;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            GroupName = staticName.Text.Trim();
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            GroupName = null;
            Close();
        }
    }
}
