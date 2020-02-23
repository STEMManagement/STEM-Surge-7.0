using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace STEM.Surge.ControlPanel
{
    public partial class SandboxConfigUpdate : Form
    {
        UIActor _UIActor = null;

        public SandboxConfigUpdate(UIActor uiActor)
        {
            InitializeComponent();

            FormClosing += SandboxConfigUpdate_FormClosing;

            controllerEditor1.Visible = false;
            splitContainer1.Visible = false;
            beginSandboxUpdate.Visible = false;

            updateEntries.RowValidated += new DataGridViewCellEventHandler(updateEntries_RowValidated);
            versionSelect.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(versionSelect_EditingControlShowing);

            _UIActor = uiActor;

            LoadTypes();

            deploymentControllerSelect.Items.AddRange(_UIActor.DeploymentManagerConfiguration.DeploymentControllers.Where(i => i.StringContent != null).Select(i => STEM.Sys.IO.Path.GetFileNameWithoutExtension(i.Filename)).ToArray());
        }

        private void SandboxConfigUpdate_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_Disabled != null && _Disabled.Count > 0)
            {
                foreach (SwitchboardConfig.FileSourcesRow r in _Disabled)
                {
                    SwitchboardConfig.FileSourcesRow t = _UIActor.DeploymentManagerConfiguration.SwitchboardConfiguration.FileSources.FindBySourceDirectoryDirectoryFilterFileFilter(r.SourceDirectory, r.DirectoryFilter, r.FileFilter);

                    if (t != null)
                        t.Enable = true;
                }

                _UIActor.SubmitSwitchboardConfigurationUpdate(_UIActor.DeploymentManagerConfiguration.SwitchboardConfiguration);
            }
        }

        public bool IsDirty
        {
            get
            {
                foreach (DataGridViewRow r in updateEntries.Rows)
                    if (((bool)r.Cells[0].Value) == true)
                        return true;

                return false;
            }
        }

        void updateEntries_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
                beginSandboxUpdate.Visible = (IsDirty || (bool)updateEntries.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
        }

        void versionSelect_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (versionSelect.CurrentCell.ColumnIndex == 2)
            {
                ComboBox cb = e.Control as ComboBox;
                cb.SelectedIndexChanged -= new EventHandler(versionComboBox_SelectedIndexChanged);
                cb.SelectedIndexChanged += new EventHandler(versionComboBox_SelectedIndexChanged);
            }
        }

        private void versionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int row = versionSelect.CurrentCell.RowIndex;
            int col = versionSelect.CurrentCell.ColumnIndex;

            ComboBox cb = ((ComboBox)sender);

            string oldVersion = versionSelect.Rows[row].Cells[1].Value as string;
            string version = cb.SelectedItem as string;

            string oldVersionNumber = oldVersion.Split(',')[1].Replace("Version=", "").Trim();

            if (oldVersionNumber == version)
            {
                versionSelect.Rows[row].Cells[0].Style.BackColor = Color.WhiteSmoke;
                List<DataGridViewRow> rem = new List<DataGridViewRow>();
                foreach (string t in _FileMap[oldVersion].Keys)
                    foreach (string f in _FileMap[oldVersion][t])
                        foreach (DataGridViewRow r in updateEntries.Rows)
                            if ((string)r.Cells[1].Value == t && (string)r.Cells[2].Value == f)
                                rem.Add(r);

                foreach (DataGridViewRow r in rem)
                    updateEntries.Rows.Remove(r);
            }
            else
            {
                versionSelect.Rows[row].Cells[0].Style.BackColor = Color.LawnGreen;
                foreach (string t in _FileMap[oldVersion].Keys)
                    foreach (string f in _FileMap[oldVersion][t])
                    {
                        bool exists = false;
                        foreach (DataGridViewRow r in updateEntries.Rows)
                            if ((string)r.Cells[1].Value == t && (string)r.Cells[2].Value == f)
                            {
                                exists = true;
                                r.Cells[3].Value = oldVersion;
                                r.Cells[4].Value = oldVersion.Replace(oldVersionNumber, version);
                                break;
                            }

                        if (!exists)
                            updateEntries.Rows.Add(true, t, f, oldVersion, oldVersion.Replace(oldVersionNumber, version));
                    }
            }

            beginSandboxUpdate.Visible = IsDirty;
            controllerEditor1.Visible = false;
        }


        Dictionary<string, List<AssemblyName>> _LoadedAssemblies = new Dictionary<string, List<AssemblyName>>();
        void LoadTypes()
        {
            _LoadedAssemblies.Clear();

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                AssemblyName n = a.GetName();
                if (!_LoadedAssemblies.ContainsKey(n.Name))
                    _LoadedAssemblies[n.Name] = new List<AssemblyName>();

                string s = n.ToString().Split(',')[0].Trim() + ", " + n.ToString().Split(',')[1].Trim();
                _LoadedAssemblies[n.Name].Add(new AssemblyName(s));
            }
        }

        STEM.Sys.IO.FileDescription _DCfd = null;
        STEM.Sys.IO.FileDescription _ISfd = null;

        List<SwitchboardConfig.FileSourcesRow> _Disabled = null;

        Dictionary<string, Dictionary<string, List<string>>> _FileMap = new Dictionary<string, Dictionary<string, List<string>>>();
        private void deploymentControllerSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            splitContainer1.Visible = false;
            beginSandboxUpdate.Visible = false;
            controllerEditor1.Visible = false;

            _FileMap = new Dictionary<string, Dictionary<string, List<string>>>();
            List<string> found = new List<string>();
            
            _DCfd = null;
            STEM.Surge._DeploymentController deploymentController = null;

            _ISfd = null;
            STEM.Surge.InstructionSet iSet = null;

            _DCfd = _UIActor.DeploymentManagerConfiguration.DeploymentControllers.FirstOrDefault(i => STEM.Sys.IO.Path.GetFileNameWithoutExtension(i.Filename).Equals(deploymentControllerSelect.SelectedItem.ToString()));

            if (_DCfd != null)
            {
                try
                {
                    deploymentController = STEM.Surge._DeploymentController.Deserialize(_DCfd.StringContent) as STEM.Surge._DeploymentController;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "The Deployment Controller could not be deserialized.\r\n" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (deploymentController == null)
                {
                    MessageBox.Show(this, "The Deployment Controller could not be deserialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                splitContainer1.Visible = true;

                string v = deploymentController.VersionDescriptor.AssemblyName.Split(',')[0].Trim() + ", " + deploymentController.VersionDescriptor.AssemblyName.Split(',')[1].Trim();
                if (!found.Contains(v))
                {
                    _FileMap[v] = new Dictionary<string, List<string>>();
                    found.Add(v);
                }

                if (!_FileMap[v].ContainsKey(deploymentController.VersionDescriptor.TypeName))
                    _FileMap[v][deploymentController.VersionDescriptor.TypeName] = new List<string>();

                if (!_FileMap[v][deploymentController.VersionDescriptor.TypeName].Contains("Controller, " + STEM.Sys.IO.Path.GetFileNameWithoutExtension(_DCfd.Filename)))
                    _FileMap[v][deploymentController.VersionDescriptor.TypeName].Add("Controller, " + STEM.Sys.IO.Path.GetFileNameWithoutExtension(_DCfd.Filename));

                _ISfd = _UIActor.DeploymentManagerConfiguration.InstructionSetTemplates.FirstOrDefault(i => i.StringContent != null && 
                                                    (deploymentController.InstructionSetTemplate.Equals(STEM.Sys.IO.Path.GetFileNameWithoutExtension(i.Filename), StringComparison.InvariantCultureIgnoreCase) ||
                                                    deploymentController.InstructionSetTemplate.Equals(i.Filename, StringComparison.InvariantCultureIgnoreCase)));

                if (_ISfd != null)
                {                    
                    try
                    {
                        iSet = STEM.Surge.InstructionSet.Deserialize(_ISfd.StringContent) as STEM.Surge.InstructionSet;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "The Instruction Set could not be deserialized.\r\n" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (iSet == null)
                    {
                        MessageBox.Show(this, "The Instruction Set could not be deserialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    foreach (STEM.Surge.Instruction i in iSet.Instructions)
                    {
                        v = i.VersionDescriptor.AssemblyName.Split(',')[0].Trim() + ", " + i.VersionDescriptor.AssemblyName.Split(',')[1].Trim();
                        if (!found.Contains(v))
                        {
                            _FileMap[v] = new Dictionary<string, List<string>>();
                            found.Add(v);
                        }

                        if (!_FileMap[v].ContainsKey(i.VersionDescriptor.TypeName))
                            _FileMap[v][i.VersionDescriptor.TypeName] = new List<string>();

                        if (!_FileMap[v][i.VersionDescriptor.TypeName].Contains("Template, " + STEM.Sys.IO.Path.GetFileNameWithoutExtension(_ISfd.Filename)))
                            _FileMap[v][i.VersionDescriptor.TypeName].Add("Template, " + STEM.Sys.IO.Path.GetFileNameWithoutExtension(_ISfd.Filename));
                    }
                }
                
                found.Sort();
                versionSelect.Rows.Clear();
                updateEntries.Rows.Clear();
                foreach (string s in found)
                {
                    int index = versionSelect.Rows.Add();
                    versionSelect.Rows[index].Cells[0].Value = s.Split(',')[0];
                    versionSelect.Rows[index].Cells[1].Value = s;

                    string aName = s.Split(',')[0];

                    if (!_LoadedAssemblies.ContainsKey(aName))
                        _LoadedAssemblies[aName] = new List<AssemblyName>();

                    AssemblyName fullName = new AssemblyName(s);
                    if (!_LoadedAssemblies[aName].Exists(i => i.ToString() == fullName.ToString()))
                        _LoadedAssemblies[aName].Add(fullName);

                    foreach (AssemblyName n in _LoadedAssemblies[aName].OrderBy(i => i.Version))
                        ((DataGridViewComboBoxCell)versionSelect.Rows[index].Cells[2]).Items.Add(n.Version.ToString());

                    ((DataGridViewComboBoxCell)versionSelect.Rows[index].Cells[2]).Value = s.Split(',')[1].Replace("Version=", "").Trim();

                    versionSelect.Rows[index].Cells[0].Style.BackColor = Color.WhiteSmoke;
                }
            }
        }

        private void beginSandboxUpdate_Click(object sender, EventArgs e)
        {
            controllerEditor1.Visible = false;

            STEM.Sys.IO.FileDescription dcfd = null;
            STEM.Sys.IO.FileDescription isfd = null;

            _Disabled = _UIActor.DeploymentManagerConfiguration.SwitchboardConfiguration.FileSources.
                Where(i => i.Sandbox == true && i.ControllerFilename.Equals(STEM.Sys.IO.Path.GetFileNameWithoutExtension(_DCfd.Filename), StringComparison.InvariantCultureIgnoreCase)).ToList();

            if (_Disabled.Count == 0)
            {
                MessageBox.Show(this, "There are no sandboxed rows for this controller.", "No Action To Take", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (SwitchboardConfig.FileSourcesRow r in _Disabled.ToList())
            {
                if (r.Enable)
                {
                    r.Enable = false;
                }
                else
                {
                    _Disabled.Remove(r);
                }
            }

            if (_Disabled.Count > 0)
            {
                _UIActor.SubmitSwitchboardConfigurationUpdate(_UIActor.DeploymentManagerConfiguration.SwitchboardConfiguration);
            }

            foreach (DataGridViewRow r in updateEntries.Rows)
                if (((bool)r.Cells[0].Value) == true)
                {
                    string file = (string)r.Cells[2].Value;

                    string[] fParts = file.Split(new char[] { ',' });

                    STEM.Sys.IO.FileDescription fd = null;

                    switch (fParts[0].Trim())
                    {
                        case "Controller":
                            dcfd = new Sys.IO.FileDescription();
                            dcfd.CopyFrom(_DCfd);
                            fd = dcfd;
                            break;

                        case "Template":
                            if (isfd == null)
                            {
                                isfd = new Sys.IO.FileDescription();
                                isfd.CopyFrom(_ISfd);
                            }

                            fd = isfd;
                            break;
                    }

                    if (fd != null)
                    {
                        XDocument doc = XDocument.Parse(fd.StringContent);

                        foreach (XElement vn in from b in doc.Descendants("VersionDescriptor") select b)
                        {
                            XElement tn = vn.Descendants("TypeName").FirstOrDefault(i => i.Value == (string)r.Cells[1].Value);
                            if (tn != null)
                            {
                                XElement an = vn.Descendants("AssemblyName").FirstOrDefault(i => i.Value.StartsWith((string)r.Cells[3].Value));
                                if (an != null)
                                {
                                    an.Value = an.Value.Replace((string)r.Cells[3].Value, (string)r.Cells[4].Value);
                                }
                            }
                        }

                        fd.StringContent = doc.ToString();
                        fd.LastWriteTimeUtc = DateTime.UtcNow;
                    }
                }

            if (dcfd == null)
            {
                dcfd = new Sys.IO.FileDescription();
                dcfd.CopyFrom(_DCfd);
            }

            if (isfd == null)
            {
                isfd = new Sys.IO.FileDescription();

                if (_ISfd != null)
                {
                    isfd.CopyFrom(_ISfd);
                }
                else
                {
                    isfd.Filename = "New Template";
                    isfd.LastWriteTimeUtc = DateTime.UtcNow;
                    isfd.StringContent = new InstructionSet().Serialize();
                }
            }

            controllerEditor1.Bind(dcfd, isfd, _UIActor, false);

            controllerEditor1.onInstructionSetTemplateSaved += InstructionSetTemplateSaved;
            controllerEditor1.onSaved += DeploymentControllerSaved;

            controllerEditor1.Visible = true;
        }
        
        private void InstructionSetTemplateSaved(object sender, EventArgs e)
        {
            _ISfd = sender as STEM.Sys.IO.FileDescription;
        }

        private void DeploymentControllerSaved(object sender, EventArgs e)
        {
            _DCfd = sender as STEM.Sys.IO.FileDescription;

            if (_DCfd != null)
            {
                if (_Disabled != null && _Disabled.Count > 0)
                {
                    MessageBox.Show(this, "Now we must wait for the old configuration assignment to settle.\r\n This may take a minute or two.", "Please Wait", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    foreach (SwitchboardConfig.FileSourcesRow r in _Disabled)
                    {
                        string id = STEM.Surge.GenerateSwitchboardRowIDs.GenerateSwitchboardRowID(r);

                        while (_UIActor.Backlogs.Count(i => i.SwitchboardRowID == id && i.Assigning == true) > 0)
                            System.Threading.Thread.Sleep(1000);

                        while (_UIActor.Backlogs.Count(i => i.SwitchboardRowID == id && (DateTime.UtcNow - i.LastAssignment).TotalSeconds < 30) > 0)
                            System.Threading.Thread.Sleep(1000);
                    }
                }

                STEM.Sys.IO.FileDescription fd = _UIActor.DeploymentManagerConfiguration.DeploymentControllers.FirstOrDefault(i => i.Filename.Equals(_DCfd.Filename, StringComparison.InvariantCultureIgnoreCase));

                if (fd != null)
                {
                    fd.CopyFrom(_DCfd);
                }
                else
                {
                    _UIActor.DeploymentManagerConfiguration.DeploymentControllers.Add(_DCfd);
                }

                fd = _UIActor.DeploymentManagerConfiguration.InstructionSetTemplates.FirstOrDefault(i => i.Filename.Equals(_ISfd.Filename, StringComparison.InvariantCultureIgnoreCase));

                if (fd != null)
                {
                    fd.CopyFrom(_ISfd);
                }
                else
                {
                    _UIActor.DeploymentManagerConfiguration.InstructionSetTemplates.Add(_ISfd);
                }

                _UIActor.SubmitConfigurationUpdate();
            }

            if (_Disabled != null && _Disabled.Count > 0)
            {
                foreach (SwitchboardConfig.FileSourcesRow r in _Disabled)
                {
                    SwitchboardConfig.FileSourcesRow t = _UIActor.DeploymentManagerConfiguration.SwitchboardConfiguration.FileSources.FindBySourceDirectoryDirectoryFilterFileFilter(r.SourceDirectory, r.DirectoryFilter, r.FileFilter);
                    
                    if (t != null)
                        t.Enable = true;
                }

                _UIActor.SubmitSwitchboardConfigurationUpdate(_UIActor.DeploymentManagerConfiguration.SwitchboardConfiguration);
            }

            _Disabled = null;
        }
    }
}
