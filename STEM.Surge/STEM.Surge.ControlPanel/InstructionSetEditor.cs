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
    public partial class InstructionSetEditor : UserControl
    {
        UIActor _UIActor;
        Surge.InstructionSet _InstructionSet;
        List<STEM.Sys.IO.FileDescription> _InstructionSets;

        string _RelPath = "";

        public EventHandler onSaved;

        public string ProcessName { get { return _InstructionSet.ProcessName; } }

        public bool IsDirty
        {
            get
            {
                return _DIRTY;
            }
        }

        public InstructionSetEditor()
        {
            InitializeComponent();

            instructionProperties.SelectedGridItemChanged += instructionProperties_SelectedGridItemChanged;
            instructionProperties.PropertyValueChanged += new PropertyValueChangedEventHandler(instructionProperties_PropertyValueChanged);
            continuousExecution.CheckedChanged += new EventHandler(continuousExecution_CheckedChanged);
            runInSandboxes.CheckedChanged += new EventHandler(runInSandboxes_CheckedChanged);
            cachePostMortem.CheckedChanged += new EventHandler(cachePostMortem_CheckedChanged);
            processName.TextChanged += new EventHandler(processName_TextChanged);
            macroPlaceholderGrid.CellClick += macroPlaceholderGrid_CellClick;
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

            SetAutocomplete(instructionProperties);
        }

        void macroPlaceholderGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                if (instructionProperties.SelectedGridItem.PropertyDescriptor.PropertyType == typeof(string))
                {
                    string v;
                    string x = v = instructionProperties.SelectedGridItem.PropertyDescriptor.GetValue(instructionProperties.SelectedObject) as string;
                    v += macroPlaceholderGrid[1, e.RowIndex].Value;

                    instructionProperties.SelectedGridItem.PropertyDescriptor.SetValue(instructionProperties.SelectedObject, v);

                    instructionProperties_PropertyValueChanged(instructionProperties.SelectedObject, new PropertyValueChangedEventArgs(instructionProperties.SelectedGridItem, x));

                    instructionProperties.SelectedGridItem.Select();
                }
            }
        }

        Dictionary<string, string> _Macros = new Dictionary<string, string>();

        void SetAutocomplete(System.Windows.Forms.Control control)
        {
            if (control is TextBox)
            {
                TextBox t = control as TextBox;

                t.TextChanged -= TextBox_TextChanged;
                t.TextChanged += TextBox_TextChanged;
            }
            else
            {
                foreach (System.Windows.Forms.Control c in control.Controls)
                {
                    SetAutocomplete(c);
                }
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox t = sender as TextBox;

            if (t.Text.EndsWith("["))
            {
                t.AutoCompleteMode = AutoCompleteMode.Suggest;
                t.AutoCompleteSource = AutoCompleteSource.CustomSource;
                t.AutoCompleteCustomSource = new AutoCompleteStringCollection();
                foreach (DataGridViewRow r in macroPlaceholderGrid.Rows)
                {
                    if (r.Cells[1].Value == null)
                        break;

                    t.AutoCompleteCustomSource.Add(t.Text.Substring(0, t.Text.Length - 1) + r.Cells[1].Value.ToString());
                }
            }
        }

        public void Bind(List<STEM.Sys.IO.FileDescription> instructionSets, Surge.InstructionSet instructionSet, Dictionary<string, string> macros, UIActor messageClient, bool canRepeat, string relPath)
        {
            _UIActor = messageClient;
            _RelPath = relPath;
            _Macros = macros;

            _LastSelectedInstructionIndex = -1;

            instructionProperties.PropertyValueChanged -= new PropertyValueChangedEventHandler(instructionProperties_PropertyValueChanged);
            continuousExecution.CheckedChanged -= new EventHandler(continuousExecution_CheckedChanged);
            runInSandboxes.CheckedChanged -= new EventHandler(runInSandboxes_CheckedChanged);
            cachePostMortem.CheckedChanged -= new EventHandler(cachePostMortem_CheckedChanged);
            processName.TextChanged -= new EventHandler(processName_TextChanged);

            _InstructionSet = instructionSet;
            _InstructionSets = instructionSets;

            instructionList.Items.Clear();

            processName.Text = _InstructionSet.ProcessName;

            cachePostMortem.Checked = _InstructionSet.CachePostMortem;

            if (!canRepeat)
            {
                continuousExecution.Visible = false;
                runInSandboxes.Visible = false;
                _InstructionSet.ContinuousExecution = continuousExecution.Checked = false;
                repeatInterval.Visible = false;
                label1.Visible = false;
            }

            continuousExecution.Checked = _InstructionSet.ContinuousExecution;
            runInSandboxes.Checked = _InstructionSet.ExecuteStaticInSandboxes;
            cachePostMortem.Checked = _InstructionSet.CachePostMortem;

            repeatInterval.Enabled = continuousExecution.Checked;
            repeatInterval.Text = _InstructionSet.ContinuousExecutionInterval.ToString();

            foreach (Surge.Instruction i in _InstructionSet.Instructions)
                instructionList.Items.Add(i.VersionDescriptor.TypeName);

            instructionProperties.SelectedObject = null;

            if (_InstructionSet.Instructions.Count > 0)
            {
                instructionProperties.SelectedObject = _InstructionSet.Instructions[0];
                instructionList.SelectedIndex = 0;
            }

            instructionProperties.PropertyValueChanged += new PropertyValueChangedEventHandler(instructionProperties_PropertyValueChanged);
            continuousExecution.CheckedChanged += new EventHandler(continuousExecution_CheckedChanged);
            runInSandboxes.CheckedChanged += new EventHandler(runInSandboxes_CheckedChanged);
            cachePostMortem.CheckedChanged += new EventHandler(cachePostMortem_CheckedChanged);
            processName.TextChanged += new EventHandler(processName_TextChanged);

            macroPlaceholderGrid.Rows.Clear();

            foreach (string k in macros.Keys)
                macroPlaceholderGrid.Rows.Add(null, k, macros[k]);

            _DIRTY = false;
            save.Enabled = _DIRTY;
        }

        void processName_TextChanged(object sender, EventArgs e)
        {
            _DIRTY = true;
            save.Enabled = _DIRTY;
        }
        
        void continuousExecution_CheckedChanged(object sender, EventArgs e)
        {
            _DIRTY = true;
            save.Enabled = _DIRTY;

            repeatInterval.Enabled = continuousExecution.Checked;
        }

        private void runInSandboxes_CheckedChanged(object sender, EventArgs e)
        {
            _DIRTY = true;
            save.Enabled = _DIRTY;
        }

        private void save_Click(object sender, EventArgs e)
        {
            if (_InstructionSet == null)
                return;

            _InstructionSet.ProcessName = processName.Text.Trim();
            _InstructionSet.ContinuousExecution = continuousExecution.Checked;
            _InstructionSet.ExecuteStaticInSandboxes = runInSandboxes.Checked;
            _InstructionSet.CachePostMortem = cachePostMortem.Checked;

            try
            {
                _InstructionSet.ContinuousExecutionInterval = Int32.Parse(repeatInterval.Text.Trim());
            }
            catch
            {
                MessageBox.Show(this, "The repeat interval must be an interger value", "Integer required.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string fileName = _InstructionSet.ProcessName.Trim();

            if (fileName.Length < 1)
            {
                MessageBox.Show(this, "You must set a name for this InstructionSet in the 'Process Name' box.", "Name required.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            while (fileName.ToUpper().EndsWith(".IS"))
                fileName = fileName.Remove(fileName.Length - 4);

            STEM.Sys.IO.FileDescription fd = _InstructionSets.FirstOrDefault(i => i.Filename.Equals(fileName + ".is", StringComparison.InvariantCultureIgnoreCase));

            if (fd != null && fd.Content != null)
            {
                string usedBy = String.Join("\r\n\t", _UIActor.DeploymentManagerConfiguration.DeploymentControllers.Where(i => i.Content != null && i.StringContent.ToUpper().Contains((">" + _InstructionSet.ProcessName + "<").ToUpper())).Select(i => STEM.Sys.IO.Path.GetFileNameWithoutExtension(i.Filename)).ToList());

                if (usedBy == "")
                    usedBy = "Not in use by any Deployment Controller.";
                else
                    usedBy = "In use by:\r\n\t" + usedBy;

                if (MessageBox.Show(this, "An InstructionSet named " + fileName + " already exists in this space. Overwrite?\r\n" + usedBy, "Overwrite?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
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

                _InstructionSets.Add(fd);
            }

            _UIActor.SubmitConfigurationUpdate();
            
            save.Enabled = _DIRTY = false;

            if (onSaved != null)
                try
                {
                    onSaved(this, EventArgs.Empty);
                }
                catch { }

            Bind(_InstructionSets, _InstructionSet, _Macros, _UIActor, continuousExecution.Visible, _RelPath);
        
            MessageBox.Show(this, "Save Complete!", "", MessageBoxButtons.OK);
        }
        
        bool _DIRTY = false;

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

        void instructionProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            _DIRTY = true;
            save.Enabled = _DIRTY;
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

        private void usedByControllers_Click(object sender, EventArgs e)
        {
            string usedBy = String.Join("\r\n\t", _UIActor.DeploymentManagerConfiguration.DeploymentControllers.Where(i => i.Content != null && (i.StringContent.ToUpper().Contains((">" + _InstructionSet.ProcessName + "<").ToUpper()) || i.StringContent.ToUpper().Contains((_InstructionSet.ProcessName + ".is<").ToUpper()))).Select(i => STEM.Sys.IO.Path.GetFileNameWithoutExtension(i.Filename)).ToList());

            if (usedBy == "")
                usedBy = "Not in use by any Deployment Controller.";
            else
                usedBy = "In use by:\r\n\t" + usedBy;

            MessageBox.Show(usedBy, "Used By...", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void history_Click(object sender, EventArgs e)
        {

        }

        private void cachePostMortem_CheckedChanged(object sender, EventArgs e)
        {
            _DIRTY = true;
            save.Enabled = _DIRTY;
        }

        private void RepeatInterval_TextChanged(object sender, EventArgs e)
        {
            _DIRTY = true;
            save.Enabled = _DIRTY;
        }
    }
}
