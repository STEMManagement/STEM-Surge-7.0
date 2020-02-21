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
    public partial class ConnectionNotification : Form
    {
        public ConnectionNotification(string address)
        {
            InitializeComponent();

            label3.Text = "(" + address + ")";
            label2.Text = DateTime.UtcNow.ToString("U");
        }

        private void label5_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
