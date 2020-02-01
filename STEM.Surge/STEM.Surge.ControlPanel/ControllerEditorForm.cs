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
    public partial class ControllerEditorForm : Form
    {
        public ControllerEditorForm(UIActor messageClient, string controllerFilename)
        {
            InitializeComponent();

            FormClosing += ControllerEditorForm_FormClosing;

            controllerEditor1.Bind(messageClient, controllerFilename);
        }

        void ControllerEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (controllerEditor1.IsDirty)
            {
                if (MessageBox.Show(this, "Cancel Changes?", "Unsaved", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
