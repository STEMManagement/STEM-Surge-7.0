using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using STEM.Surge.Messages;
using STEM.Sys.IO;

namespace STEM.Surge.ControlPanel
{
    public partial class StaticsListEditor : UserControl
    {
        UIActor _UIActor;

        public bool IsDirty
        {
            get
            {
                return instructionSetListEditor1.IsDirty;
            }
        }
        
        public StaticsListEditor(UIActor messageClient)
        {
            InitializeComponent();

            if (messageClient.DeploymentManagerConfiguration.MessageConnection == null)
            {
                MessageBox.Show(this, "No active connection.", "Connect", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            _UIActor = messageClient; 
            
            instructionSetListEditor1.Visible = false;

            foreach (string sub in _UIActor.DeploymentManagerConfiguration.InstructionSetStatics.Keys)
            {
                fileList.Items.Add(sub);
            }

            if (!fileList.Items.Contains("All"))
                fileList.Items.Add("All");

            if (!fileList.Items.Contains("Manager"))
                fileList.Items.Add("Manager");

            fileList_SelectedIndexChanged(this, EventArgs.Empty);
        }

        private void newFile_Click(object sender, EventArgs e)
        {
            if (IsDirty)
            {
                if (MessageBox.Show(this, "Cancel Changes?", "Unsaved", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }

            NewStaticGroup nsg = new NewStaticGroup();
            nsg.ShowDialog(this);

            if (nsg.GroupName != null)
            {
                fileList.Items.Add(nsg.GroupName);
                fileList.SelectedItem = nsg.GroupName;

                fileList_SelectedIndexChanged(this, EventArgs.Empty);
            }
        }

        private void deleteFile_Click(object sender, EventArgs e)
        {
            if (IsDirty)
            {
                if (MessageBox.Show(this, "Cancel Changes?", "Unsaved", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }
        }

        private void fileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_UIActor.AssemblyInitializationComplete)
            {
                MessageBox.Show("Assembly Initialization is still in progress. Please retry later.", "Initialization In Progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (fileList.SelectedItem == null)
                return;
            
            if (IsDirty)
            {
                if (MessageBox.Show(this, "Cancel Changes?", "Unsaved", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }

            List<FileDescription> instructionSets = new List<FileDescription>();

            if (!_UIActor.DeploymentManagerConfiguration.InstructionSetStatics.ContainsKey(fileList.SelectedItem as string))
                _UIActor.DeploymentManagerConfiguration.InstructionSetStatics[fileList.SelectedItem as string] = new List<FileDescription>();

            instructionSets = _UIActor.DeploymentManagerConfiguration.InstructionSetStatics[fileList.SelectedItem as string];

            instructionSetListEditor1.Initialize(instructionSets, _UIActor, System.IO.Path.Combine("Statics", fileList.SelectedItem as string));
            instructionSetListEditor1.Visible = true;
        }
    }
}
