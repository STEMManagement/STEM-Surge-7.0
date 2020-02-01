using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;
using STEM.Surge.Messages;
using STEM.Surge;

namespace STEM.Surge.ControlPanel
{
    public partial class VersionUpdates : UserControl
    {
        UIActor _UIActor;
        
        public VersionUpdates(UIActor messageClient)
        {
            InitializeComponent();

            if (messageClient.DeploymentManagerConfiguration.MessageConnection == null)
            {
                MessageBox.Show(this, "No active connection.", "Connect", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            _UIActor = messageClient;
                        
            LoadTypes();
            LoadActiveVersions();

            updateEntries.RowValidated += new DataGridViewCellEventHandler(updateEntries_RowValidated);
            dataGridView1.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(dataGridView1_EditingControlShowing);
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

        Dictionary<string, Dictionary<string, List<string>>> _FileMap = new Dictionary<string, Dictionary<string, List<string>>>();
        void LoadActiveVersions()
        {
            updateVersions.Enabled = false;

            _FileMap = new Dictionary<string, Dictionary<string, List<string>>>();
            List<string> found = new List<string>();

            foreach (STEM.Sys.IO.FileDescription fd in _UIActor.DeploymentManagerConfiguration.DeploymentControllers.Where(i => i.Content != null).ToList())
            {
                try
                {
                    XDocument doc = XDocument.Parse(fd.StringContent);

                    foreach (XElement vn in doc.Descendants("VersionDescriptor"))
                    {
                        XElement an = vn.Descendants("AssemblyName").FirstOrDefault();
                        XElement tn = vn.Descendants("TypeName").FirstOrDefault();


                        string v = an.Value.Split(',')[0].Trim() + ", " + an.Value.Split(',')[1].Trim();
                        if (!found.Contains(v))
                        {
                            _FileMap[v] = new Dictionary<string, List<string>>();
                            found.Add(v);
                        }

                        if (!_FileMap[v].ContainsKey(tn.Value))
                            _FileMap[v][tn.Value] = new List<string>();

                        if (!_FileMap[v][tn.Value].Contains("Controller, " + STEM.Sys.IO.Path.GetFileNameWithoutExtension(fd.Filename)))
                            _FileMap[v][tn.Value].Add("Controller, " + STEM.Sys.IO.Path.GetFileNameWithoutExtension(fd.Filename));
                    }
                }
                catch { }
            }

            foreach (string sub in _UIActor.DeploymentManagerConfiguration.InstructionSetStatics.Keys)
                foreach (STEM.Sys.IO.FileDescription fd in _UIActor.DeploymentManagerConfiguration.InstructionSetStatics[sub].Where(i => i.Content != null).ToList())
                {
                    try
                    {
                        XDocument doc = XDocument.Parse(fd.StringContent);

                        foreach (XElement vn in doc.Descendants("VersionDescriptor"))
                        {
                            XElement an = vn.Descendants("AssemblyName").FirstOrDefault();
                            XElement tn = vn.Descendants("TypeName").FirstOrDefault();


                            string v = an.Value.Split(',')[0].Trim() + ", " + an.Value.Split(',')[1].Trim();
                            if (!found.Contains(v))
                            {
                                _FileMap[v] = new Dictionary<string, List<string>>();
                                found.Add(v);
                            }

                            if (!_FileMap[v].ContainsKey(tn.Value))
                                _FileMap[v][tn.Value] = new List<string>();

                            if (!_FileMap[v][tn.Value].Contains("Static, " + sub + ", " + STEM.Sys.IO.Path.GetFileNameWithoutExtension(fd.Filename)))
                                _FileMap[v][tn.Value].Add("Static, " + sub + ", " + STEM.Sys.IO.Path.GetFileNameWithoutExtension(fd.Filename));
                        }
                    }
                    catch { }
                }

            foreach (STEM.Sys.IO.FileDescription fd in _UIActor.DeploymentManagerConfiguration.InstructionSetTemplates.Where(i => i.Content != null).ToList())
            {
                try
                {
                    XDocument doc = XDocument.Parse(fd.StringContent);

                    foreach (XElement vn in doc.Descendants("VersionDescriptor"))
                    {
                        XElement an = vn.Descendants("AssemblyName").FirstOrDefault();
                        XElement tn = vn.Descendants("TypeName").FirstOrDefault();


                        string v = an.Value.Split(',')[0].Trim() + ", " + an.Value.Split(',')[1].Trim();
                        if (!found.Contains(v))
                        {
                            _FileMap[v] = new Dictionary<string, List<string>>();
                            found.Add(v);
                        }

                        if (!_FileMap[v].ContainsKey(tn.Value))
                            _FileMap[v][tn.Value] = new List<string>();

                        if (!_FileMap[v][tn.Value].Contains("Template, " + STEM.Sys.IO.Path.GetFileNameWithoutExtension(fd.Filename)))
                            _FileMap[v][tn.Value].Add("Template, " + STEM.Sys.IO.Path.GetFileNameWithoutExtension(fd.Filename));
                    }
                }
                catch { }
            }

            found.Sort();
            dataGridView1.Rows.Clear();
            updateEntries.Rows.Clear();
            foreach (string s in found)
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = s.Split(',')[0];
                dataGridView1.Rows[index].Cells[1].Value = s;

                string aName = s.Split(',')[0];

                if (!_LoadedAssemblies.ContainsKey(aName))
                    _LoadedAssemblies[aName] = new List<AssemblyName>();

                AssemblyName fullName = new AssemblyName(s);
                if (!_LoadedAssemblies[aName].Exists(i => i.ToString() == fullName.ToString()))
                    _LoadedAssemblies[aName].Add(fullName);

                foreach (AssemblyName n in _LoadedAssemblies[aName].OrderBy(i => i.Version))
                    ((DataGridViewComboBoxCell)dataGridView1.Rows[index].Cells[2]).Items.Add(n.Version.ToString());

                ((DataGridViewComboBoxCell)dataGridView1.Rows[index].Cells[2]).Value = s.Split(',')[1].Replace("Version=", "").Trim();

                dataGridView1.Rows[index].Cells[0].Style.BackColor = Color.WhiteSmoke;
            }
        }

        private void versionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int row = dataGridView1.CurrentCell.RowIndex;
            int col = dataGridView1.CurrentCell.ColumnIndex;

            ComboBox cb = ((ComboBox)sender);

            string oldVersion = dataGridView1.Rows[row].Cells[1].Value as string;
            string version = cb.SelectedItem as string;

            string oldVersionNumber = oldVersion.Split(',')[1].Replace("Version=", "").Trim();

            if (oldVersionNumber == version)
            {
                dataGridView1.Rows[row].Cells[0].Style.BackColor = Color.WhiteSmoke;
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
                dataGridView1.Rows[row].Cells[0].Style.BackColor = Color.LawnGreen;
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

            updateVersions.Enabled = IsDirty;
        }
        
        void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == 2)
            {
                ComboBox cb = e.Control as ComboBox;
                cb.SelectedIndexChanged -= new EventHandler(versionComboBox_SelectedIndexChanged);
                cb.SelectedIndexChanged += new EventHandler(versionComboBox_SelectedIndexChanged);
            }
        }

        private void updateVersions_Click(object sender, EventArgs e)
        {
            try
            {
                XDocument doc = null;

                foreach (DataGridViewRow r in updateEntries.Rows)
                    if (((bool)r.Cells[0].Value) == true)
                    {
                        string file = (string)r.Cells[2].Value;

                        string[] fParts = file.Split(new char[] { ',' });

                        STEM.Sys.IO.FileDescription fd = null;

                        switch (fParts[0].Trim())
                        {
                            case "Controller":
                                fd = _UIActor.DeploymentManagerConfiguration.DeploymentControllers.FirstOrDefault(i => i.Filename.Equals(fParts[1].Trim() + ".dc", StringComparison.InvariantCultureIgnoreCase) && i.Content != null);
                                break;

                            case "Template":
                                fd = _UIActor.DeploymentManagerConfiguration.InstructionSetTemplates.FirstOrDefault(i => i.Filename.Equals(fParts[1].Trim() + ".is", StringComparison.InvariantCultureIgnoreCase) && i.Content != null);
                                break;

                            case "Static":
                                string sub = fParts[1].Trim();
                                fd = _UIActor.DeploymentManagerConfiguration.InstructionSetStatics[sub].FirstOrDefault(i => i.Filename.Equals(fParts[2].Trim() + ".is", StringComparison.InvariantCultureIgnoreCase) && i.Content != null);
                                break;
                        }

                        if (fd != null)
                        {
                            doc = XDocument.Parse(fd.StringContent);

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

                _UIActor.SubmitConfigurationUpdate();
                LoadTypes();
                LoadActiveVersions();
            }
            catch (Exception ex)
            {
                ExceptionViewer ev = new ExceptionViewer(ex);
                ev.ShowDialog(this);
            }

            LoadTypes();
            LoadActiveVersions();

            MessageBox.Show(this, "Save Complete!", "", MessageBoxButtons.OK);
        }

        private void selectAll_Click(object sender, EventArgs e)
        {
            foreach(DataGridViewRow r in updateEntries.Rows)
                r.Cells[0].Value = true;

            updateVersions.Enabled = IsDirty;
        }

        private void deselectAll_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow r in updateEntries.Rows)
                r.Cells[0].Value = false;

            updateVersions.Enabled = IsDirty;
        }

        void updateEntries_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
                updateVersions.Enabled = IsDirty || (bool)updateEntries.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
        }
    }
}
