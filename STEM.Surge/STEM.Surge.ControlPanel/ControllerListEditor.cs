using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using STEM.Surge.Messages;

namespace STEM.Surge.ControlPanel
{
    public partial class ControllerListEditor : UserControl
    {
        UIActor _UIActor;
        List<string> _LastList = new List<string>();
        object _LastFileObject = null;

        public ControllerListEditor(UIActor messageClient)
        {
            InitializeComponent();

            _UIActor = messageClient;

            if (messageClient.DeploymentManagerConfiguration.MessageConnection == null)
            {
                MessageBox.Show(this, "No active connection.", "Connect", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            fileList.SelectedIndexChanged += fileList_SelectedIndexChanged;

            filterBox.TextChanged += filterBox_TextChanged;

            _LastList = _UIActor.DeploymentManagerConfiguration.DeploymentControllers.Where(i => i.Content != null).Select(i => STEM.Sys.IO.Path.GetFileNameWithoutExtension(i.Filename)).ToList();
            filterBox_TextChanged(this, EventArgs.Empty);
            fileList_SelectedIndexChanged(this, EventArgs.Empty);

            detailsPanel.Visible = false;

            controllerEditor1.onSaved += controllerEditor1_onSaved;
        }

        private void controllerEditor1_onSaved(object sender, EventArgs e)
        {
            _LastList = _UIActor.DeploymentManagerConfiguration.DeploymentControllers.Where(i => i.Content != null).Select(i => STEM.Sys.IO.Path.GetFileNameWithoutExtension(i.Filename)).ToList();

            if (!_LastList.Contains(STEM.Sys.IO.Path.GetFileNameWithoutExtension(controllerEditor1.ActiveControllerFile)))
                _LastList.Add(STEM.Sys.IO.Path.GetFileNameWithoutExtension(controllerEditor1.ActiveControllerFile));

            filterBox_TextChanged(this, EventArgs.Empty);
            fileList.SelectedItem = STEM.Sys.IO.Path.GetFileNameWithoutExtension(controllerEditor1.ActiveControllerFile);
            fileList_SelectedIndexChanged(this, EventArgs.Empty);
        }

        public bool IsDirty
        {
            get
            {
                return detailsPanel.Visible && controllerEditor1.IsDirty;
            }
        }
        
        private void clearFilter_Click(object sender, EventArgs e)
        {
            filterBox.Text = "";
            filterBox_TextChanged(sender, e);
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

            controllerEditor1.Bind(_UIActor, null);

            detailsPanel.Visible = true;
        }

        private void deleteFile_Click(object sender, EventArgs e)
        {
            if (fileList.SelectedItem == null)
                return;

            string file = fileList.SelectedItem as string;
            STEM.Sys.IO.FileDescription fd = _UIActor.DeploymentManagerConfiguration.DeploymentControllers.FirstOrDefault(i => i.Filename.Equals(file + ".dc", StringComparison.InvariantCultureIgnoreCase) && i.Content != null);

            if (fd != null)
            {
                if (MessageBox.Show(this, "Delete " + file + "?", "Delete?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    fd.Content = null;
                    fd.LastWriteTimeUtc = DateTime.UtcNow;
                    _UIActor.SubmitConfigurationUpdate();
                }
                else
                {
                    return;
                }
            }

            _LastList = _UIActor.DeploymentManagerConfiguration.DeploymentControllers.Where(i => i.Content != null).Select(i => STEM.Sys.IO.Path.GetFileNameWithoutExtension(i.Filename)).ToList();

            int index = fileList.SelectedIndex;

            fileList.SelectedIndices.Clear();
            fileList.SelectedItems.Clear();
            fileList.Items.RemoveAt(index);

            if (index > 0)
                fileList.SelectedItem = fileList.Items[index - 1];
            else if (fileList.Items.Count > 0)
                fileList.SelectedItem = fileList.Items[0];

            fileList_SelectedIndexChanged(sender, e);
        }
        
        private void filterBox_TextChanged(object sender, EventArgs e)
        {
            fileList.Items.Clear();

            if (filterBox.Text.Trim().Length > 0)
                fileList.Items.AddRange(_LastList.Select(i => STEM.Sys.IO.Path.GetFileName(i)).Where(i => i.ToUpper().Contains(filterBox.Text.Trim().ToUpper())).ToArray());
            else
                fileList.Items.AddRange(_LastList.Select(i => STEM.Sys.IO.Path.GetFileName(i)).ToArray());
        }
        
        private void fileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_UIActor.AssemblyInitializationComplete)
            {
                MessageBox.Show("Assembly Initialization is still in progress. Please retry later.", "Initialization In Progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                if (fileList.SelectedItem == _LastFileObject && _LastFileObject != null)
                    return;
                
                if (IsDirty)
                {
                    if (MessageBox.Show(this, "Cancel Changes?", "Unsaved", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                    {
                        fileList.SelectedItem = _LastFileObject;
                        return;
                    }
                }

                if (fileList.SelectedItem == null)
                {
                    detailsPanel.Visible = false;
                    return;
                }

                detailsPanel.Visible = true;

                _LastFileObject = fileList.SelectedItem;
                
                controllerEditor1.Bind(_UIActor, fileList.SelectedItem as string + ".dc");                
            }
            catch (Exception ex)
            {
                ExceptionViewer ev = new ExceptionViewer(ex);
                ev.ShowDialog(this);

                fileList.ClearSelected();
                detailsPanel.Visible = false;
            }
        }
        
        private void archiveUnusedControllers_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Archive Unused Controllers?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _UIActor.Send(new ArchiveUnusedControllers());

                _LastList = _UIActor.DeploymentManagerConfiguration.DeploymentControllers.Where(i => i.Content != null).Select(i => STEM.Sys.IO.Path.GetFileNameWithoutExtension(i.Filename)).ToList();

                filterBox_TextChanged(this, EventArgs.Empty);

                MessageBox.Show(this, "The task has been started. Please return to this page later to see the cleaned list.", "Time is needed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
