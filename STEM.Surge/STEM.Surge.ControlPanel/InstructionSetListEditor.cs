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

namespace STEM.Surge.ControlPanel
{
    public partial class InstructionSetListEditor : UserControl
    {
        UIActor _UIActor;

        List<STEM.Sys.IO.FileDescription> _InstructionSets;
        string _RelPath = "";

        public bool IsDirty
        {
            get
            {
                return instructionSetEditor1.Visible && instructionSetEditor1.IsDirty;
            }
        }

        bool CanBeContinuous { get; set; }

        public InstructionSetListEditor(bool canBeContinuous)
        {
            InitializeComponent();

            _InstructionSets = new List<Sys.IO.FileDescription>();

            CanBeContinuous = canBeContinuous;
            filterBox_TextChanged(this, EventArgs.Empty);
            instructionSetEditor1.Visible = false;

            instructionSetEditor1.onSaved += instructionSetEditor1_onSaved;
        }

        private void instructionSetEditor1_onSaved(object sender, EventArgs e)
        {
            string fileName = instructionSetEditor1.ProcessName;
            
            filterBox_TextChanged(this, EventArgs.Empty);
            fileList.SelectedItem = fileName;
            fileList_SelectedIndexChanged(this, EventArgs.Empty);
        }

        public void Initialize(List<STEM.Sys.IO.FileDescription> instructionSets, UIActor messageClient, string relPath)
        {
            _UIActor = messageClient;
            _RelPath = relPath;
            
            filterBox_TextChanged(this, EventArgs.Empty);

            _InstructionSets = instructionSets;
            
            filterBox_TextChanged(this, EventArgs.Empty);
            fileList_SelectedIndexChanged(this, EventArgs.Empty);
        }

        object _LastFileObject = null;
        Surge.InstructionSet _ActiveInstructionSet = null;
        private void newFile_Click(object sender, EventArgs e)
        {
            if (IsDirty)
            {
                if (MessageBox.Show(this, "Cancel Changes?", "Unsaved", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }

            fileList.SelectedIndices.Clear();
            fileList.SelectedItems.Clear();

            _LastFileObject = null;
            _ActiveInstructionSet = new Surge.InstructionSet();

            instructionSetEditor1.Bind(_InstructionSets, _ActiveInstructionSet, new Dictionary<string, string>(), _UIActor, CanBeContinuous, _RelPath, true);
            instructionSetEditor1.Visible = true;
        }

        private void deleteFile_Click(object sender, EventArgs e)
        {
            if (fileList.SelectedItem == null)
                return;

            string file = fileList.SelectedItem as string;

            if (MessageBox.Show(this, "Delete " + file + "?", "Delete?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                STEM.Sys.IO.FileDescription fd = _InstructionSets.FirstOrDefault(i => i.Filename.Equals(file + ".is", StringComparison.InvariantCultureIgnoreCase) && i.Content != null);
                
                if (fd != null)
                {
                    fd.Content = null;
                    fd.LastWriteTimeUtc = DateTime.UtcNow;
                    _UIActor.SubmitConfigurationUpdate();
                }
            }
            else
            {
                return;
            }
            
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
         
        private void fileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_UIActor.AssemblyInitializationComplete)
            {
                MessageBox.Show("Assembly Initialization is still in progress. Please retry later.", "Initialization In Progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                if (IsDirty)
                {
                    if (MessageBox.Show(this, "Cancel Changes?", "Unsaved", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                    {
                        fileList.SelectedItem = _LastFileObject;
                        return;
                    }
                }

                _LastFileObject = fileList.SelectedItem;

                if (fileList.SelectedItem == null)
                {
                    _ActiveInstructionSet = null;
                    instructionSetEditor1.Visible = false;
                    return;
                }

                STEM.Sys.IO.FileDescription fd = _InstructionSets.FirstOrDefault(i => i.Filename.Equals(fileList.SelectedItem as string + ".is", StringComparison.InvariantCultureIgnoreCase) && i.Content != null);

                if (fd != null)
                    _ActiveInstructionSet = Surge.InstructionSet.Deserialize(fd.StringContent) as Surge.InstructionSet;

                instructionSetEditor1.Bind(_InstructionSets, _ActiveInstructionSet, new Dictionary<string, string>(), _UIActor, CanBeContinuous, _RelPath, true);
                instructionSetEditor1.Visible = true;
            }
            catch (Exception ex)
            {
                ExceptionViewer ev = new ExceptionViewer(ex);
                ev.ShowDialog(this);
                
                _ActiveInstructionSet = null;
                instructionSetEditor1.Visible = false;
            }
        }
        
        private void filterBox_TextChanged(object sender, EventArgs e)
        {
            fileList.Items.Clear();

            List<string> list = _InstructionSets.Where(i => i.Content != null).Select(i => STEM.Sys.IO.Path.GetFileNameWithoutExtension(i.Filename)).ToList();

            if (filterBox.Text.Trim().Length > 0)
                fileList.Items.AddRange(list.Select(i => STEM.Sys.IO.Path.GetFileName(i)).Where(i => i.ToUpper().Contains(filterBox.Text.Trim().ToUpper())).ToArray());
            else
                fileList.Items.AddRange(list.Select(i => STEM.Sys.IO.Path.GetFileName(i)).ToArray());
        }

        private void clearFilter_Click(object sender, EventArgs e)
        {
            filterBox.Text = "";
            filterBox_TextChanged(sender, e);
        }
        
        private void archiveUnusedTemplates_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Archive Unused Templates?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _UIActor.Send(new ArchiveUnusedTemplates());
                MessageBox.Show(this, "The task has been started. Please return to this page later to see the cleaned list.", "Time is needed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
