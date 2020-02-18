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
    public partial class ControllerEditor : UserControl
    {
        UIActor _UIActor = null;

        STEM.Sys.IO.FileDescription _DC = null;
        STEM.Sys.IO.FileDescription _IS = null;

        STEM.Surge._DeploymentController _ActiveController = null;

        bool _SaveToManager = false;

        public EventHandler onSaved;

        public ControllerEditor()
        {
            InitializeComponent();

            deploymentControllerName.TextChanged += new EventHandler(deploymentControllerName_TextChanged);

            controllerProperties.PropertyValueChanged += new PropertyValueChangedEventHandler(controllerProperties_PropertyValueChanged);

            controllerProperties.SelectedGridItemChanged += controllerProperties_SelectedGridItemChanged;

            macroPlaceholderGrid.CellClick += macroPlaceholderGrid_CellClick;

            macroPlaceholderGrid.UserDeletingRow += macroPlaceholderGrid_UserDeletingRow;
            macroPlaceholderGrid.RowsAdded += macroPlaceholderGrid_RowsAdded;
            macroPlaceholderGrid.CurrentCellDirtyStateChanged += macroPlaceholderGrid_CurrentCellDirtyStateChanged;
        }

        void controllerProperties_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            if (e.NewSelection.PropertyDescriptor == null)
                return;

            if (e.NewSelection.PropertyDescriptor.PropertyType == typeof(List<string>))
            {
                StringCollectionEditor strings = new StringCollectionEditor(e.NewSelection.PropertyDescriptor, controllerProperties.SelectedObject);
                strings.ShowDialog(this);

                if (strings.PropertyValueChanged)
                    controllerProperties_PropertyValueChanged(sender, null);

                return;
            }

            if (e.NewSelection.PropertyDescriptor.PropertyType.ToString().Contains("STEM.Sys.Serialization.Dictionary"))
            {
                DictionaryEditorForm dict = new DictionaryEditorForm(e.NewSelection.PropertyDescriptor, controllerProperties.SelectedObject);
                dict.ShowDialog(this);

                if (dict.PropertyValueChanged)
                    controllerProperties_PropertyValueChanged(sender, null);

                return;
            }

            SetAutocomplete(controllerProperties);
        }
        
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

                    t.AutoCompleteCustomSource.Add(t.Text.Substring(0, t.Text.Length-1) + r.Cells[1].Value.ToString());
                }
            }
        }

        public void Bind(STEM.Sys.IO.FileDescription dc, STEM.Sys.IO.FileDescription iSet, UIActor uiActor, bool saveToManager)
        {
            _UIActor = uiActor;
            _SaveToManager = saveToManager;

            controllerProperties.SelectedObject = null;
            _ActiveController = null;
            openTemplate.Visible = false;
            detailsLabel.Text = "";

            _DC = dc;
            _IS = iSet;
                
            _ActiveController = STEM.Surge._DeploymentController.Deserialize(_DC.StringContent) as STEM.Surge._DeploymentController;

            openTemplate.Visible = true;

            _ActiveController.InstructionSetTemplate = STEM.Sys.IO.Path.GetFileNameWithoutExtension(_IS.Filename);
            
            controllerProperties.SelectedObject = _ActiveController;

            macroPlaceholderGrid.Rows.Clear();

            foreach (SwitchboardConfig.ConfigurationMacroMapRow r in _UIActor.DeploymentManagerConfiguration.SwitchboardConfiguration.ConfigurationMacroMap)
                _ActiveController.TemplateKVP[r.Placeholder] = "Reserved";

            foreach (string k in _ActiveController.TemplateKVP.Keys.OrderBy(i => i))
                macroPlaceholderGrid.Rows.Add(null, k, _ActiveController.TemplateKVP[k]);

            foreach (DataGridViewRow r in macroPlaceholderGrid.Rows)
            {
                if (r.Cells[2].Value == null)
                    break;

                if (r.Cells[2].Value.ToString() == "Reserved")
                    r.ReadOnly = true;
            }

            deploymentControllerName.Text = STEM.Sys.IO.Path.GetFileNameWithoutExtension(_DC.Filename);

            detailsLabel.Text = _ActiveController.VersionDescriptor.TypeName;

            save.Enabled = _DIRTY = false;

            if (!_UIActor.DeploymentManagerConfiguration.DeploymentControllers.Exists(i => i == _DC))
            {
                _DIRTY = true;
                save.Enabled = _DIRTY;
            }
        }

        public void Bind(string controllerFilename, UIActor uiActor, bool saveToManager)
        {
            _UIActor = uiActor;
            _SaveToManager = saveToManager;

            _DC = null;
            _IS = null;

            try
            {
                if (controllerFilename == null)
                {
                    NewController nc = new NewController();
                    nc.ShowDialog(this);

                    if (nc.SelectedController != null)
                    {
                        _ActiveController = nc.SelectedController;

                        _ActiveController.InstructionSetTemplate = STEM.Sys.IO.Path.GetFileName(_ActiveController.InstructionSetTemplate);

                        if (_ActiveController.InstructionSetTemplate.EndsWith(".is", StringComparison.InvariantCultureIgnoreCase))
                            _ActiveController.InstructionSetTemplate = STEM.Sys.IO.Path.GetFileNameWithoutExtension(_ActiveController.InstructionSetTemplate);

                        controllerProperties.SelectedObject = _ActiveController;

                        macroPlaceholderGrid.Rows.Clear();

                        foreach (SwitchboardConfig.ConfigurationMacroMapRow r in _UIActor.DeploymentManagerConfiguration.SwitchboardConfiguration.ConfigurationMacroMap)
                            _ActiveController.TemplateKVP[r.Placeholder] = "Reserved";

                        foreach (string k in _ActiveController.TemplateKVP.Keys.OrderBy(i => i))
                            macroPlaceholderGrid.Rows.Add(null, k, _ActiveController.TemplateKVP[k]);

                        foreach (DataGridViewRow r in macroPlaceholderGrid.Rows)
                        {
                            if (r.Cells[2].Value == null)
                                break;

                            if (r.Cells[2].Value.ToString().Equals("Reserved", StringComparison.InvariantCultureIgnoreCase))
                                r.ReadOnly = true;
                        }

                        openTemplate.Visible = true;

                        deploymentControllerName.Text = "NewController";

                        _DC = new FileDescription();
                        _DC.CreationTimeUtc = DateTime.UtcNow;
                        _DC.Filename = "";

                        detailsLabel.Text = _ActiveController.VersionDescriptor.TypeName;

                        _DIRTY = true;
                        save.Enabled = _DIRTY;
                    }
                }
                else
                {
                    controllerProperties.SelectedObject = null;
                    _ActiveController = null;
                    openTemplate.Visible = false;
                    detailsLabel.Text = "";

                    _DC = _UIActor.DeploymentManagerConfiguration.DeploymentControllers.FirstOrDefault(i => i.Filename.Equals(controllerFilename, StringComparison.InvariantCultureIgnoreCase) && i.StringContent != null);

                    if (_DC != null)
                        _ActiveController = STEM.Surge._DeploymentController.Deserialize(_DC.StringContent) as STEM.Surge._DeploymentController;

                    openTemplate.Visible = true;

                    _ActiveController.InstructionSetTemplate = STEM.Sys.IO.Path.GetFileName(_ActiveController.InstructionSetTemplate);

                    if (_ActiveController.InstructionSetTemplate.EndsWith(".is", StringComparison.InvariantCultureIgnoreCase))
                        _ActiveController.InstructionSetTemplate = STEM.Sys.IO.Path.GetFileNameWithoutExtension(_ActiveController.InstructionSetTemplate);

                    controllerProperties.SelectedObject = _ActiveController;
                    
                    macroPlaceholderGrid.Rows.Clear();

                    foreach (SwitchboardConfig.ConfigurationMacroMapRow r in _UIActor.DeploymentManagerConfiguration.SwitchboardConfiguration.ConfigurationMacroMap)
                        _ActiveController.TemplateKVP[r.Placeholder] = "Reserved";

                    foreach (string k in _ActiveController.TemplateKVP.Keys.OrderBy(i => i))
                        macroPlaceholderGrid.Rows.Add(null, k, _ActiveController.TemplateKVP[k]);
                    
                    foreach (DataGridViewRow r in macroPlaceholderGrid.Rows)
                    {
                        if (r.Cells[2].Value == null)
                            break;

                        if (r.Cells[2].Value.ToString() == "Reserved")
                            r.ReadOnly = true;
                    }

                    deploymentControllerName.Text = STEM.Sys.IO.Path.GetFileNameWithoutExtension(_DC.Filename);

                    detailsLabel.Text = _ActiveController.VersionDescriptor.TypeName;

                    save.Enabled = _DIRTY = false;
                }
            }
            catch (Exception ex)
            {
                ExceptionViewer ev = new ExceptionViewer(ex);
                ev.ShowDialog(this);

                controllerProperties.SelectedObject = null;
                detailsLabel.Text = "";
                save.Enabled = _DIRTY = false;
            }
        }
                                
        void macroPlaceholderGrid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            _DIRTY = true;
            save.Enabled = _DIRTY;
        }

        void macroPlaceholderGrid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            _DIRTY = true;
            save.Enabled = _DIRTY;
        }

        void macroPlaceholderGrid_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            e.Cancel = e.Row.Cells[2].Value.ToString().Equals("Reserved", StringComparison.InvariantCultureIgnoreCase);

            if (!e.Cancel)
                _DIRTY = true;

            save.Enabled = _DIRTY;
        }

        bool _DIRTY = false;
        public bool IsDirty
        {
            get
            {
                return _DIRTY;
            }
        }

        void deploymentControllerName_TextChanged(object sender, EventArgs e)
        {
            _DIRTY = true;
            save.Enabled = _DIRTY;
        }

        private void save_Click(object sender, EventArgs e)
        {
            if (controllerProperties.SelectedObject == null)
                return;

            string fileName = deploymentControllerName.Text.Trim();

            if (fileName.Length < 1)
            {
                MessageBox.Show(this, "You must set a name for this Deployment Controller in the 'Deployment Controller Name' box.", "Name required.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            while (fileName.ToUpper().EndsWith(".DC"))
                fileName = fileName.Remove(fileName.Length - 4);

            fileName += ".dc";

            FileDescription fd = _UIActor.DeploymentManagerConfiguration.DeploymentControllers.FirstOrDefault(i => i.Filename.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
            
            if (fd != null)
            {
                if (fd.Content != null)
                    if (MessageBox.Show(this, "A Deployment Controller named " + fileName + " already exists. Overwrite?", "Overwrite?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        return;

                _DC = fd;
            }
            else
            {
                _DC = new FileDescription();
                _DC.CreationTimeUtc = DateTime.UtcNow;
                _DC.Filename = fileName;
            }

            _DeploymentController dc = controllerProperties.SelectedObject as _DeploymentController;

            dc.TemplateKVP.Clear();

            foreach (DataGridViewRow r in macroPlaceholderGrid.Rows)
            {
                if (r.Cells[1].Value == null)
                    continue;

                if (r.Cells[2].Value == null)
                    dc.TemplateKVP[r.Cells[1].Value.ToString()] = "";
                else
                    dc.TemplateKVP[r.Cells[1].Value.ToString()] = r.Cells[2].Value.ToString();
            }

            try
            {
                KVPMapUtils.ApplyKVP("", dc.TemplateKVP, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Macro Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            dc.InstructionSetTemplate = STEM.Sys.IO.Path.GetFileName(dc.InstructionSetTemplate);

            if (dc.InstructionSetTemplate.EndsWith(".is", StringComparison.InvariantCultureIgnoreCase))
                dc.InstructionSetTemplate = STEM.Sys.IO.Path.GetFileNameWithoutExtension(dc.InstructionSetTemplate);
                
            _DC.StringContent = dc.Serialize();
            _DC.LastWriteTimeUtc = DateTime.UtcNow;

            if (_SaveToManager)
            {
                if (!_UIActor.DeploymentManagerConfiguration.DeploymentControllers.Exists(i => i.Filename.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)))
                    _UIActor.DeploymentManagerConfiguration.DeploymentControllers.Add(_DC);
                    
                _UIActor.SubmitConfigurationUpdate();
            }

            save.Enabled = _DIRTY = false;

            if (onSaved != null)
                try
                {
                    onSaved(_DC, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.ToString(), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            MessageBox.Show(this, "Save Complete!", "", MessageBoxButtons.OK);
        }
                
        void controllerProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            _DIRTY = true;
            save.Enabled = _DIRTY;
        }

        void macroPlaceholderGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                if (controllerProperties.SelectedGridItem.PropertyDescriptor.PropertyType == typeof(string))
                {
                    string v;
                    string x = v = controllerProperties.SelectedGridItem.PropertyDescriptor.GetValue(_ActiveController) as string;
                    v += macroPlaceholderGrid[1, e.RowIndex].Value;

                    controllerProperties.SelectedGridItem.PropertyDescriptor.SetValue(_ActiveController, v);

                    controllerProperties_PropertyValueChanged(_ActiveController, new PropertyValueChangedEventArgs(controllerProperties.SelectedGridItem, x));

                    controllerProperties.SelectedGridItem.Select();
                }
            }
        }
                
        private void openTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                Surge.InstructionSet iSet = new Surge.InstructionSet();

                iSet.ProcessName = _ActiveController.InstructionSetTemplate;

                if (_IS == null || !_IS.Filename.Equals(_ActiveController.InstructionSetTemplate + ".is", StringComparison.InvariantCultureIgnoreCase))
                    _IS = _UIActor.DeploymentManagerConfiguration.InstructionSetTemplates.FirstOrDefault(i => i.Filename.Equals(_ActiveController.InstructionSetTemplate + ".is", StringComparison.InvariantCultureIgnoreCase));

                if (_IS != null)
                {
                    if (_IS.Content != null)
                    {
                        iSet = Surge.InstructionSet.Deserialize(_IS.StringContent) as Surge.InstructionSet;
                        iSet.ProcessName = _ActiveController.InstructionSetTemplate;
                    }
                    else
                    {
                        _IS = null;
                    }
                }
                
                if (_IS == null)
                {
                    _IS = new FileDescription();
                    _IS.Filename = iSet.ProcessName + ".is";
                    _IS.CreationTimeUtc = DateTime.UtcNow;
                    _IS.LastWriteTimeUtc = DateTime.UtcNow;
                    _IS.StringContent = iSet.Serialize();
                }

                Dictionary<string, string> macros = new Dictionary<string, string>();
                foreach (DataGridViewRow r in macroPlaceholderGrid.Rows)
                {
                    if (r.Cells[2].Value == null)
                        break;

                    macros[r.Cells[1].Value.ToString()] = r.Cells[2].Value.ToString();
                }

                InstructionSetEditorForm ief = new InstructionSetEditorForm(_UIActor.DeploymentManagerConfiguration.InstructionSetTemplates, _IS, macros, _UIActor, "Templates", _SaveToManager);
                
                ief.Show(this);

                _ActiveController.InstructionSetTemplate = ief.ProcessName;
            }
            catch (Exception ex)
            {
                ExceptionViewer ev = new ExceptionViewer(ex);
                ev.ShowDialog(this);
            }
        }

        private void history_Click(object sender, EventArgs e)
        {

        }
    }
}
