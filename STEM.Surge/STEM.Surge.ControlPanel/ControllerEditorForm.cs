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
        public string ControllerFilename { get; private set; }

        public ControllerEditorForm(UIActor messageClient, string controllerFilename)
        {
            InitializeComponent();

            FormClosing += ControllerEditorForm_FormClosing;

            ControllerFilename = controllerFilename;

            controllerEditor1.Bind(controllerFilename, messageClient, true);
        }

        void ControllerEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (controllerEditor1.IsDirty)
            {
                if (MessageBox.Show(this, "Cancel Changes?", "Unsaved", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            ControllerFilename = controllerEditor1.ControllerFilename;
        }
    }
}
