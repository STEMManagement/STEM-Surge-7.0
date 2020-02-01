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
        public ConnectionNotification()
        {
            InitializeComponent();
        }

        public void CloseForm()
        {
            lock (this)
            {
                if (_IsOpen)
                {
                    _IsOpen = false;
                    Close();
                }
            }
        }

        bool _IsOpen = false;

        public void Update(IWin32Window owner, string address)
        {
            lock (this)
            {
                label3.Text = "(" + address + ")";
                label2.Text = DateTime.UtcNow.ToString("U");

                if (!_IsOpen)
                {
                    _IsOpen = true;
                    this.ShowDialog(owner);
                }
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            CloseForm();
        }
    }
}
