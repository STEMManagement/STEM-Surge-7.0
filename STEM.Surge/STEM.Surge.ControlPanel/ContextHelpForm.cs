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
    public partial class ContextHelpForm : Form
    {
        public ContextHelpForm(string rtf)
        {
            InitializeComponent();

            richTextBox1.Rtf = rtf;
        }
    }
}
