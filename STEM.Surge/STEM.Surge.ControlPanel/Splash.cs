using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace STEM.Surge.ControlPanel
{
    public partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();
        }

        public void Start(IWin32Window owner)
        {
            this.Show(owner);
        }

        public void Stop()
        {
            Close();
        }
    }
}
