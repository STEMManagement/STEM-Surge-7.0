using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using STEM.Surge.Messages;
using STEM.Sys.IO;

namespace STEM.Surge.ControlPanel
{
    public partial class InstructionSetEditorForm : Form
    {
        public EventHandler onSaved;

        string _ProcessName = null;
        public string ProcessName { get { return _ProcessName; } }

        public InstructionSetEditorForm(List<FileDescription> instructionSets, Surge.InstructionSet instructionSet, Dictionary<string, string> macros, UIActor messageClient, string relPath, bool saveToManager)
        {
            InitializeComponent();

            _ProcessName = instructionSet.ProcessName;

            FormClosing += InstructionSetEditorForm_FormClosing;

            Text = System.IO.Path.Combine(relPath, instructionSet.ProcessName);

            instructionSetEditor1.onSaved += instructionSetEditor1_onSaved;

            instructionSetEditor1.Bind(instructionSets, instructionSet, macros, messageClient, false, relPath, saveToManager);
        }

        public InstructionSetEditorForm(List<FileDescription> instructionSets, STEM.Sys.IO.FileDescription instructionSet, Dictionary<string, string> macros, UIActor messageClient, string relPath, bool saveToManager)
        {
            InitializeComponent();

            _ProcessName = STEM.Sys.IO.Path.GetFileNameWithoutExtension(instructionSet.Filename);

            FormClosing += InstructionSetEditorForm_FormClosing;

            Text = System.IO.Path.Combine(relPath, _ProcessName);

            instructionSetEditor1.onSaved += instructionSetEditor1_onSaved;

            instructionSetEditor1.Bind(instructionSets, instructionSet, macros, messageClient, false, relPath, saveToManager);
        }

        private void instructionSetEditor1_onSaved(object sender, EventArgs e)
        {
            _ProcessName = instructionSetEditor1.ProcessName;
            
            if (onSaved != null)
                onSaved(sender, EventArgs.Empty);
        }

        void InstructionSetEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (instructionSetEditor1.IsDirty)
            {
                if (MessageBox.Show(this, "Cancel Changes?", "Unsaved", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
