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
    public partial class AdHoc : Form
    {
        UIActor _UIActor;
        Surge.InstructionSet _InstructionSet = null;
        bool _DIRTY = false;

        public AdHoc(UIActor uiActor)
        {
            InitializeComponent();

            _UIActor = uiActor;

            branchList.Items.AddRange(_UIActor.BranchEntries.Select(i => i.BranchIP + "\t" + i.BranchName).ToArray());

            instructionProperties.SelectedGridItemChanged += instructionProperties_SelectedGridItemChanged;
            instructionProperties.PropertyValueChanged += new PropertyValueChangedEventHandler(instructionProperties_PropertyValueChanged);

            savedAdHoc.Items.AddRange(_UIActor.DeploymentManagerConfiguration.AdHocInstructionSets.Where(i => i.Content != null).Select(i => Path.GetFileNameWithoutExtension(i.Filename)).OrderBy(i => i).ToArray());
            
            instructionSetEditPanel.Visible = false;
        }

        void Bind()
        {
            try
            {
                if (_InstructionSet == null)
                {
                    instructionSetEditPanel.Visible = false;
                    return;
                }

                _LastSelectedInstructionIndex = -1;

                instructionProperties.PropertyValueChanged -= new PropertyValueChangedEventHandler(instructionProperties_PropertyValueChanged);
                processName.TextChanged -= new EventHandler(processName_TextChanged);

                instructionList.Items.Clear();

                processName.Text = _InstructionSet.ProcessName;

                foreach (Surge.Instruction i in _InstructionSet.Instructions)
                    instructionList.Items.Add(i.VersionDescriptor.TypeName);

                instructionProperties.SelectedObject = null;

                if (_InstructionSet.Instructions.Count > 0)
                {
                    instructionProperties.SelectedObject = _InstructionSet.Instructions[0];
                    instructionList.SelectedIndex = 0;
                }

                instructionProperties.PropertyValueChanged += new PropertyValueChangedEventHandler(instructionProperties_PropertyValueChanged);
                processName.TextChanged += new EventHandler(processName_TextChanged);

                _DIRTY = false;
                save.Enabled = _DIRTY;

                instructionSetEditPanel.Visible = true;
            }
            catch (Exception ex)
            {
                ExceptionViewer ev = new ExceptionViewer(ex);
                ev.ShowDialog(this);

                instructionSetEditPanel.Visible = false;
            }
        }

        void instructionProperties_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            if (e.NewSelection.PropertyDescriptor == null)
                return;

            if (e.NewSelection.PropertyDescriptor.PropertyType == typeof(List<string>))
            {
                StringCollectionEditor strings = new StringCollectionEditor(e.NewSelection.PropertyDescriptor, instructionProperties.SelectedObject);
                strings.ShowDialog(this);

                if (strings.PropertyValueChanged)
                    instructionProperties_PropertyValueChanged(sender, null);

                return;
            }

            if (e.NewSelection.PropertyDescriptor.PropertyType.ToString().Contains("Dictionary"))
            {
                DictionaryEditorForm dict = new DictionaryEditorForm(e.NewSelection.PropertyDescriptor, instructionProperties.SelectedObject);
                dict.ShowDialog(this);
                
                if (dict.PropertyValueChanged)
                    instructionProperties_PropertyValueChanged(sender, null);

                return;
            }
        }
        
        void instructionProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            _DIRTY = true;
            save.Enabled = _DIRTY;
        }

        private void moveUp_Click(object sender, EventArgs e)
        {
            if (instructionList.SelectedIndex > 0)
            {
                _DIRTY = true;
                save.Enabled = _DIRTY;

                int index = instructionList.SelectedIndex;

                STEM.Surge.Instruction i = _InstructionSet.Instructions[index];
                _InstructionSet.Instructions.Remove(i);
                _InstructionSet.Instructions.Insert(index - 1, i);

                object o = instructionList.Items[index];

                instructionList.ClearSelected();

                instructionList.Items.RemoveAt(index);
                instructionList.Items.Insert(index - 1, o);

                _LastSelectedInstructionIndex = instructionList.SelectedIndex = index - 1;
            }
        }

        private void moveDown_Click(object sender, EventArgs e)
        {
            if (instructionList.SelectedIndex < instructionList.Items.Count - 1)
            {
                _DIRTY = true;
                save.Enabled = _DIRTY;

                int index = instructionList.SelectedIndex;

                STEM.Surge.Instruction i = _InstructionSet.Instructions[index];
                _InstructionSet.Instructions.Remove(i);
                _InstructionSet.Instructions.Insert(index + 1, i);

                object o = instructionList.Items[index];

                instructionList.ClearSelected();

                instructionList.Items.RemoveAt(index);
                instructionList.Items.Insert(index + 1, o);

                _LastSelectedInstructionIndex = instructionList.SelectedIndex = index + 1;
            }
        }

        private void deleteInstruction_Click(object sender, EventArgs e)
        {
            if (instructionList.SelectedIndex < 0)
                return;
            
            _DIRTY = true;
            save.Enabled = _DIRTY;

            int index = instructionList.SelectedIndex;

            instructionList.ClearSelected();

            _InstructionSet.Instructions.RemoveAt(index);
            instructionList.Items.RemoveAt(index);

            if (index > 0)
                instructionList.SelectedIndex = index - 1;
            else if (_InstructionSet.Instructions.Count > 0)
                instructionList.SelectedIndex = 0;
            else
                instructionList.SelectedIndex = -1;

            instructionList_SelectedIndexChanged(this, EventArgs.Empty);
        }

        static List<Assembly> _LoadedAssemblies = new List<Assembly>();
        static List<Type> _LoadedTypes = new List<Type>();
        private void addInstruction_Click(object sender, EventArgs e)
        {
            Assembly[] appAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in appAssemblies)
            {
                if (_LoadedAssemblies.Contains(a))
                    continue;

                _LoadedAssemblies.Add(a);

                try
                {
                    Module[] mod = a.GetModules();
                    foreach (Module m in mod)
                    {
                        try
                        {
                            Type[] types = m.GetTypes();
                            foreach (Type t in types)
                            {
                                if (_LoadedTypes.Contains(t))
                                    continue;

                                try
                                {
                                    if (!t.IsAbstract)
                                    {
                                        if (t.IsSubclassOf(typeof(STEM.Surge.Instruction)))
                                        {
                                            _LoadedTypes.Add(t);
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }
                }
                catch { }
            }

            InstructionSelect sel = new InstructionSelect(_LoadedTypes);

            sel.ShowDialog(this);

            if (sel.SelectedType != null)
            {
                _DIRTY = true;
                save.Enabled = _DIRTY;

                _InstructionSet.Instructions.Add((STEM.Surge.Instruction)Activator.CreateInstance(sel.SelectedType));
                instructionList.Items.Add(_InstructionSet.Instructions[_InstructionSet.Instructions.Count - 1].VersionDescriptor.TypeName);
                instructionList.SelectedIndex = instructionList.Items.Count - 1;
                instructionList_SelectedIndexChanged(this, EventArgs.Empty);
            }
        }

        int _LastSelectedInstructionIndex = -1;
        private void instructionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (_LastSelectedInstructionIndex == instructionList.SelectedIndex)
                    return;

                if (instructionList.SelectedIndex == -1)
                {
                    instructionProperties.SelectedObject = null;
                    _LastSelectedInstructionIndex = instructionList.SelectedIndex;
                }
                else if (_InstructionSet.Instructions.Count > instructionList.SelectedIndex)
                {
                    instructionProperties.SelectedObject = _InstructionSet.Instructions[instructionList.SelectedIndex];
                    _LastSelectedInstructionIndex = instructionList.SelectedIndex;
                }
            }
            catch { }
        }

        private void selectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < branchList.Items.Count; i++)
            {
                branchList.SetItemCheckState(i, CheckState.Checked);
            }
        }

        private void deselectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < branchList.Items.Count; i++)
            {
                branchList.SetItemCheckState(i, CheckState.Unchecked);
            }
        }

        private void deploy_Click(object sender, EventArgs e)
        {
            string iSetXml = _InstructionSet.Serialize();

            for (int i = 0; i < branchList.Items.Count; i++)
            {
                if (branchList.GetItemCheckState(i) == CheckState.Checked)
                {
                    DeployInstructionSet m = new DeployInstructionSet();
                    m.BranchIP = ((string)branchList.Items[i]).Split('\t')[0];
                    m.InstructionSetXml = iSetXml;

                    _UIActor.Send(m);
                }
            }
        }

        private void newFile_Click(object sender, EventArgs e)
        {
            if (_DIRTY)
            {
                if (MessageBox.Show(this, "Cancel Changes?", "Unsaved", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }
            
            savedAdHoc.SelectedIndices.Clear();
            savedAdHoc.SelectedItems.Clear();

            _LastFileObject = null;
            _InstructionSet = new Surge.InstructionSet();

            _InstructionSet.ProcessName = "New AdHoc";

            Bind();
        }

        private void deleteFile_Click(object sender, EventArgs e)
        {
            if (savedAdHoc.SelectedItem == null)
                return;

            string file = savedAdHoc.SelectedItem as string;

            if (MessageBox.Show(this, "Delete " + file + "?", "Delete?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                STEM.Sys.IO.FileDescription fd = _UIActor.DeploymentManagerConfiguration.AdHocInstructionSets.Where(i => i.Content != null).FirstOrDefault(i => i.Filename.Equals(file + ".is", StringComparison.InvariantCultureIgnoreCase) && i.Content != null);

                if (fd != null)
                {
                    fd.Content = null;
                    fd.LastWriteTimeUtc = DateTime.UtcNow;
                    _UIActor.SubmitConfigurationUpdate();

                    try
                    {
                        _UIActor.DeploymentManagerConfiguration.AdHocInstructionSets.Remove(fd);
                    }
                    catch { }
                }
            }
            else
            {
                return;
            }

            int index = savedAdHoc.SelectedIndex;

            savedAdHoc.SelectedIndices.Clear();
            savedAdHoc.SelectedItems.Clear();
            savedAdHoc.Items.RemoveAt(index);

            if (index > 0)
                savedAdHoc.SelectedItem = savedAdHoc.Items[index - 1];
            else if (savedAdHoc.Items.Count > 0)
                savedAdHoc.SelectedItem = savedAdHoc.Items[0];

            savedAdHoc_SelectedIndexChanged(sender, e);
        }

        private void clearFilter_Click(object sender, EventArgs e)
        {
            filterBox.Text = "";
            filterBox_TextChanged(sender, e);
        }

        private void filterBox_TextChanged(object sender, EventArgs e)
        {
            savedAdHoc.Items.Clear();

            if (filterBox.Text.Trim().Length > 0)
                savedAdHoc.Items.AddRange(_UIActor.DeploymentManagerConfiguration.AdHocInstructionSets.Where(i => i.Content != null).Select(i => STEM.Sys.IO.Path.GetFileName(i.Filename)).Where(i => i.ToUpper().Contains(filterBox.Text.Trim().ToUpper())).OrderBy(i => i).ToArray());
            else
                savedAdHoc.Items.AddRange(_UIActor.DeploymentManagerConfiguration.AdHocInstructionSets.Where(i => i.Content != null).Select(i => STEM.Sys.IO.Path.GetFileName(i.Filename)).OrderBy(i => i).ToArray());
        }

        private void save_Click(object sender, EventArgs e)
        {
            if (_InstructionSet == null)
                return;

            _InstructionSet.ProcessName = processName.Text.Trim();
            
            string fileName = _InstructionSet.ProcessName.Trim();

            if (fileName.Length < 1)
            {
                MessageBox.Show(this, "You must set a name for this InstructionSet in the 'Process Name' box.", "Name required.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            while (fileName.ToUpper().EndsWith(".IS"))
                fileName = fileName.Remove(fileName.Length - 4);

            STEM.Sys.IO.FileDescription fd = _UIActor.DeploymentManagerConfiguration.AdHocInstructionSets.Where(i => i.Content != null).FirstOrDefault(i => i.Filename.Equals(fileName + ".is", StringComparison.InvariantCultureIgnoreCase));

            if (fd != null && fd.Content != null)
            {
                if (MessageBox.Show(this, "An InstructionSet named " + fileName + " already exists in this space. Overwrite?", "Overwrite?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;

                fd.StringContent = _InstructionSet.Serialize();
                fd.LastWriteTimeUtc = DateTime.UtcNow;
            }
            else if (fd != null)
            {
                fd.CreationTimeUtc = DateTime.UtcNow;
                fd.StringContent = _InstructionSet.Serialize();
                fd.LastWriteTimeUtc = DateTime.UtcNow;
            }
            else
            {
                fd = new STEM.Sys.IO.FileDescription();
                fd.CreationTimeUtc = DateTime.UtcNow;
                fd.Filename = fileName + ".is";

                fd.StringContent = _InstructionSet.Serialize();
                fd.LastWriteTimeUtc = DateTime.UtcNow;

                _UIActor.DeploymentManagerConfiguration.AdHocInstructionSets.Add(fd);
            }

            _UIActor.SubmitConfigurationUpdate();

            save.Enabled = _DIRTY = false;

            savedAdHoc.Items.Clear();
            savedAdHoc.Items.AddRange(_UIActor.DeploymentManagerConfiguration.AdHocInstructionSets.Where(i => i.Content != null).Select(i => Path.GetFileNameWithoutExtension(i.Filename)).OrderBy(i => i).ToArray());

            _LastFileObject = fileName;
            savedAdHoc.SelectedItem = _LastFileObject;

            MessageBox.Show(this, "Save Complete!", "", MessageBoxButtons.OK);
        }

        object _LastFileObject = null;

        private void savedAdHoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_UIActor.AssemblyInitializationComplete)
            {
                MessageBox.Show("Assembly Initialization is still in progress. Please retry later.", "Initialization In Progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                if (_DIRTY)
                {
                    if (MessageBox.Show(this, "Cancel Changes?", "Unsaved", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                    {
                        savedAdHoc.SelectedItem = _LastFileObject;
                        return;
                    }
                }

                _LastFileObject = savedAdHoc.SelectedItem;

                _InstructionSet = null;

                if (savedAdHoc.SelectedItem != null)
                {
                    STEM.Sys.IO.FileDescription fd = _UIActor.DeploymentManagerConfiguration.AdHocInstructionSets.Where(i => i.Content != null).FirstOrDefault(i => i.Filename.Equals(savedAdHoc.SelectedItem as string + ".is", StringComparison.InvariantCultureIgnoreCase) && i.Content != null);

                    if (fd != null)
                        _InstructionSet = Surge.InstructionSet.Deserialize(fd.StringContent) as Surge.InstructionSet;
                }
            }
            catch (Exception ex)
            {
                ExceptionViewer ev = new ExceptionViewer(ex);
                ev.ShowDialog(this);

                _InstructionSet = null;
            }

            Bind();
        }
        
        private void processName_TextChanged(object sender, System.EventArgs e)
        {
            _DIRTY = true;
            save.Enabled = _DIRTY;
        }
    }
}
