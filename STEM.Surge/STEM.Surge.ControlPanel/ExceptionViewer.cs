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
    public partial class ExceptionViewer : Form
    {
        public ExceptionViewer(Exception ex)
        {
            InitializeComponent();
            richTextBox1.Text = "Exception:\r\n" + ex.Message + "\r\n\r\n" + ex.ToString();

            Exception e2 = ex.InnerException;
            while (e2 != null)
            {
                richTextBox1.Text += "\r\nInnerException:\r\n" + e2.Message + "\r\n\r\n" + e2.ToString();
                e2 = e2.InnerException;
            }
        }
    }
}
