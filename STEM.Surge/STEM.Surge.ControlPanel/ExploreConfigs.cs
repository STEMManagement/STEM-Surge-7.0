using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Xml;
using System.Windows.Forms;

namespace STEM.Surge.ControlPanel
{
    public partial class ExploreConfigs : Form
    {
        UIActor _UIActor;
        SwitchboardConfig _SwitchboardConfig;
        Dictionary<string, string> _SwitchboardKVP;
        Dictionary<string, STEM.Sys.IO.FileDescription> _DeploymentControllers = new Dictionary<string, Sys.IO.FileDescription>();
        Dictionary<string, STEM.Sys.IO.FileDescription> _ISetTemplates = new Dictionary<string, Sys.IO.FileDescription>();
        Dictionary<string, STEM.Sys.IO.FileDescription> _ISetStatics = new Dictionary<string, Sys.IO.FileDescription>();

        public ExploreConfigs(UIActor messageClient)
        {
            InitializeComponent();

            _UIActor = messageClient;

            _SwitchboardConfig = new SwitchboardConfig();
            _SwitchboardConfig.ReadXml(new System.IO.StringReader(_UIActor.DeploymentManagerConfiguration.SwitchboardConfigurationDescription.StringContent));

            _SwitchboardKVP = _SwitchboardConfig.ConfigurationMacroMap.ToDictionary(i => i[0].ToString(), i => i[1].ToString());

            foreach (STEM.Sys.IO.FileDescription f in _UIActor.DeploymentManagerConfiguration.DeploymentControllers)
            {
                _DeploymentControllers[f.Filename] = f;
                try
                {
                    _DeploymentController dc = _DeploymentController.Deserialize(f.StringContent) as _DeploymentController;

                    if (dc != null)
                        continue;
                }
                catch { }

                messageText.SelectionColor = System.Drawing.Color.Red;
                messageText.AppendText("Failed to load " + f.Filename + "\r\n");
            }

            foreach (STEM.Sys.IO.FileDescription f in _UIActor.DeploymentManagerConfiguration.InstructionSetTemplates)
            {
                _ISetTemplates[f.Filename] = f;
                try
                {
                    _InstructionSet iSet = _InstructionSet.Deserialize(f.StringContent) as _InstructionSet;

                    if (iSet != null)
                        continue;
                }
                catch { }

                messageText.SelectionColor = System.Drawing.Color.Red;
                messageText.AppendText("Failed to load " + f.Filename + "\r\n");
            }

            foreach (string key in _UIActor.DeploymentManagerConfiguration.InstructionSetStatics.Keys)
                foreach (STEM.Sys.IO.FileDescription f in _UIActor.DeploymentManagerConfiguration.InstructionSetStatics[key])
                {
                    _ISetStatics[key + "\\" + f.Filename] = f;
                    try
                    {
                        _InstructionSet iSet = _InstructionSet.Deserialize(f.StringContent) as _InstructionSet;

                        if (iSet != null)
                            continue;
                    }
                    catch { }

                    messageText.SelectionColor = System.Drawing.Color.Red;
                    messageText.AppendText("Failed to load " + key + "\\" + f.Filename + "\r\n");
                }

            messageText.SelectionColor = System.Drawing.Color.Black;

            messageText.AppendText(_SwitchboardConfig.FileSources.Count + " Switchboard Rows.\r\n");
            messageText.AppendText(_SwitchboardConfig.FileSources.Where(i => i.Enable == true).Count() + " Enabled Switchboard Rows.\r\n");

            List<string> sbControllers = _SwitchboardConfig.FileSources.Select(i => i.ControllerFilename + ".dc").Distinct().ToList();

            messageText.AppendText(sbControllers.Count() + " Deployment Controllers Used.\r\n");

            foreach (string c in sbControllers)
                if (!_DeploymentControllers.ContainsKey(c))
                {
                    messageText.SelectionColor = System.Drawing.Color.Red;
                    messageText.AppendText(c + " is in use in the switchboard but does not exist.\r\n");
                }

            foreach (string c in _DeploymentControllers.Keys)
                if (!sbControllers.Contains(c))
                {
                    messageText.SelectionColor = System.Drawing.Color.Blue;
                    messageText.AppendText(c + " is not in use in the switchboard.\r\n");
                }

            messageText.SelectionColor = System.Drawing.Color.Black;
            messageText.AppendText("Initialization Complete.\r\n");

            configsFound.RowHeaderMouseDoubleClick += ConfigsFound_RowHeaderMouseDoubleClick;
        }

        STEM.Sys.IO.FileDescription _Active = null;
        private void ConfigsFound_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            xmlText.Clear();
            editorControlPanel.Controls.Clear();

            string key = configsFound.Rows[e.RowIndex].Cells[1].Value.ToString();

            _Active = null;
            if (_DeploymentControllers.ContainsKey(key))
            {
                _Active = _DeploymentControllers[key];

                try
                {
                    _DeploymentController dc = _DeploymentController.Deserialize(_Active.StringContent) as _DeploymentController;
                    ControllerEditor i = new ControllerEditor();
                    i.Bind(_UIActor, key);

                    i.Dock = DockStyle.Fill;

                    editorControlPanel.Controls.Add(i);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load Controller:\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (_ISetTemplates.ContainsKey(key))
            {
                _Active = _ISetTemplates[key];

                try
                {
                    InstructionSet iSet = InstructionSet.Deserialize(_Active.StringContent) as InstructionSet;
                    InstructionSetEditor i = new InstructionSetEditor();
                    i.Bind(_UIActor.DeploymentManagerConfiguration.InstructionSetTemplates, iSet, new Dictionary<string, string>(), _UIActor, false, "Templates");

                    i.Dock = DockStyle.Fill;

                    editorControlPanel.Controls.Add(i);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load Template:\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (_ISetStatics.ContainsKey(key))
            {
                _Active = _ISetStatics[key];

                try
                {
                    InstructionSet iSet = InstructionSet.Deserialize(_Active.StringContent) as InstructionSet;
                    InstructionSetEditor i = new InstructionSetEditor();
                    i.Bind(_UIActor.DeploymentManagerConfiguration.InstructionSetTemplates, iSet, new Dictionary<string, string>(), _UIActor, true, Path.GetDirectoryName(key));

                    i.Dock = DockStyle.Fill;

                    editorControlPanel.Controls.Add(i);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load Static Template:\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            try
            {
                XmlTextReader reader = new XmlTextReader(new StringReader(_Active.StringContent));
                int depth = -1;
                bool inValue = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:

                            depth++;

                            inValue = false;

                            xmlText.AppendText("\n");

                            for (int i = 0; i < depth; i++)
                                xmlText.AppendText("   ");

                            xmlText.SelectionColor = System.Drawing.Color.Blue;

                            xmlText.AppendText("<");

                            xmlText.SelectionColor = System.Drawing.Color.Brown;

                            xmlText.AppendText(reader.Name);

                            if (reader.HasAttributes)
                            {
                                xmlText.SelectionColor = System.Drawing.Color.Red;

                                reader.MoveToFirstAttribute();

                                do
                                {
                                    xmlText.AppendText(" " + reader.Name + "='" + reader.Value + "'");
                                } while (reader.MoveToNextAttribute());
                            }

                            xmlText.SelectionColor = System.Drawing.Color.Blue;

                            if (!reader.IsEmptyElement)
                            {
                                xmlText.AppendText(">");
                            }
                            else
                            {
                                depth--;
                                xmlText.AppendText(" />");
                            }

                            break;

                        case XmlNodeType.Text:

                            inValue = true;

                            xmlText.SelectionColor = System.Drawing.Color.Black;

                            xmlText.AppendText(reader.Value);

                            break;

                        case XmlNodeType.EndElement:

                            if (!inValue)
                            {
                                xmlText.AppendText("\n");

                                for (int i = 0; i < depth; i++)
                                    xmlText.AppendText("   ");
                            }

                            depth--;
                            inValue = false;

                            xmlText.SelectionColor = System.Drawing.Color.Blue;

                            xmlText.AppendText("</");

                            xmlText.SelectionColor = System.Drawing.Color.Brown;

                            xmlText.AppendText(reader.Name);

                            xmlText.SelectionColor = System.Drawing.Color.Blue;

                            xmlText.AppendText(">");

                            break;

                    }

                }
            }
            catch { }
        }

        private void applyFilter_Click(object sender, EventArgs e)
        {
            configsFound.Rows.Clear();

            string f = filterBox.Text.Trim();

            if (f == "")
            {
                tabControl1.SelectTab("messageText");
                messageText.SelectionColor = System.Drawing.Color.Black;
                messageText.AppendText("No search string.\r\n");

                return;
            }

            foreach (string key in _DeploymentControllers.Keys)
            {
                STEM.Sys.IO.FileDescription d = _DeploymentControllers[key];

                if (f.StartsWith("[") && f.EndsWith("]"))
                {
                    if (d.StringContent != null)
                        try
                        {
                            _DeploymentController dc = _DeploymentController.Deserialize(d.StringContent) as _DeploymentController;

                            if (dc != null)
                            {
                                foreach (string k in dc.TemplateKVP.Keys.ToList())
                                {
                                    dc.TemplateKVP[Guid.NewGuid().ToString()] = dc.TemplateKVP[k];
                                    dc.TemplateKVP.Remove(k);
                                }

                                string s = dc.Serialize();
                                if (s.ToUpper().Contains(f.ToUpper()))
                                    configsFound.Rows.Add(true, d.Filename);
                            }
                        }
                        catch { }
                }
                else
                {
                    if (d.StringContent != null)
                        if (d.StringContent.ToUpper().Contains(f.ToUpper()))
                            configsFound.Rows.Add(true, d.Filename);
                }
            }

            foreach (string key in _ISetTemplates.Keys)
            {
                STEM.Sys.IO.FileDescription d = _ISetTemplates[key];
                if (d.StringContent != null)
                    if (d.StringContent.ToUpper().Contains(f.ToUpper()))
                        configsFound.Rows.Add(true, key);
            }

            foreach (string key in _ISetStatics.Keys)
            {
                STEM.Sys.IO.FileDescription d = _ISetStatics[key];
                if (d.StringContent != null)
                    if (d.StringContent.ToUpper().Contains(f.ToUpper()))
                        configsFound.Rows.Add(true, key);
            }

            messageText.SelectionColor = System.Drawing.Color.Black;
            messageText.AppendText("Searching " + f + "...\r\n");

            tabControl1.SelectTab("searchResults");

            if (configsFound.Rows.Count == 0 && (f.StartsWith("[") && f.EndsWith("]")))
            {
                _SwitchboardConfig = new SwitchboardConfig();
                _SwitchboardConfig.ReadXml(new System.IO.StringReader(_UIActor.DeploymentManagerConfiguration.SwitchboardConfigurationDescription.StringContent));

                int inSwitchboard = _SwitchboardConfig.FileSources.Count(x =>
                x.SourceDirectory.ToUpper().Contains(f.ToUpper()) ||
                x.FileFilter.ToUpper().Contains(f.ToUpper()) ||
                x.DirectoryFilter.ToUpper().Contains(f.ToUpper()) ||
                (!x.IsCoordinatedManagerIPsNull() && x.CoordinatedManagerIPs.ToUpper().Contains(f.ToUpper())) ||
                (!x.IsLimitToBranchIPsNull() && x.LimitToBranchIPs.ToUpper().Contains(f.ToUpper())));

                if (inSwitchboard == 0)
                    if (MessageBox.Show(this, "The macro was not found in any configuration. Would you like to remove all references to it?", "Cleanup?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        _SwitchboardConfig = new SwitchboardConfig();
                        _SwitchboardConfig.ReadXml(new System.IO.StringReader(_UIActor.DeploymentManagerConfiguration.SwitchboardConfigurationDescription.StringContent));

                        int removed = 0;
                        foreach (SwitchboardConfig.ConfigurationMacroMapRow row in _SwitchboardConfig.ConfigurationMacroMap.ToList())
                        {
                            if (row.Placeholder.ToUpper() == f.ToUpper())
                            {
                                if (row.Value.ToUpper() == "RESERVED")
                                    continue;

                                removed++;
                                row.Delete();
                            }
                        }

                        if (removed > 0)
                        {
                            _UIActor.SubmitSwitchboardConfigurationUpdate(_SwitchboardConfig);
                        }

                        _SwitchboardKVP = _SwitchboardConfig.ConfigurationMacroMap.ToDictionary(i => i[0].ToString(), i => i[1].ToString());

                        foreach (string key in _DeploymentControllers.Keys)
                        {
                            STEM.Sys.IO.FileDescription d = _DeploymentControllers[key];

                            try
                            {
                                _DeploymentController dc = _DeploymentController.Deserialize(d.StringContent) as _DeploymentController;

                                if (dc != null)
                                {
                                    foreach (string k in dc.TemplateKVP.Keys.ToList())
                                    {
                                        if (k.ToUpper() == f.ToUpper())
                                        {
                                            if (!_SwitchboardKVP.Keys.ToList().Exists(x => x.ToUpper() == f.ToUpper()))
                                            {
                                                if (!dc.TemplateKVP[k].Equals("Reserved", StringComparison.InvariantCultureIgnoreCase))
                                                {
                                                    dc.TemplateKVP.Remove(k);
                                                    d.StringContent = dc.Serialize();
                                                    d.LastWriteTimeUtc = DateTime.UtcNow;
                                                    removed++;
                                                }
                                            }

                                            break;
                                        }
                                    }
                                }
                            }
                            catch { }
                        }

                        _UIActor.SubmitConfigurationUpdate();

                        MessageBox.Show(this, "Cleanup complete. Removed " + removed + " references.", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
            }
        }

        private void clearFilter_Click(object sender, EventArgs e)
        {
            filterBox.Text = "";
            tabControl1.SelectTab("messageText");

            messageText.SelectionColor = System.Drawing.Color.Black;
            messageText.AppendText("Search Cleared.\r\n");
        }

        private void saveXml_Click(object sender, EventArgs e)
        {
            _Active.StringContent = xmlText.Text;
            _Active.LastWriteTimeUtc = DateTime.UtcNow;
            _UIActor.SubmitConfigurationUpdate();
        }

        private void replaceText_Click(object sender, EventArgs e)
        {
            string f = findForReplace.Text.Trim();

            if (f == "")
            {
                MessageBox.Show("No replacements made.", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string r = replaceFromFind.Text.Trim();

            List<string> applyTo = new List<string>();
            foreach (DataGridViewRow row in configsFound.Rows)
            {
                if (((bool)row.Cells[0].Value) == true)
                    applyTo.Add((string)row.Cells[1].Value);
            }

            Dictionary<string, string> modified = new Dictionary<string, string>();

            foreach (string key in _DeploymentControllers.Keys)
            {
                if (applyTo.Contains(key))
                {
                    STEM.Sys.IO.FileDescription d = _DeploymentControllers[key];
                    if (d.StringContent.Contains(f))
                    {
                        modified[key] = d.StringContent.Replace(f, r);
                    }
                }
            }

            foreach (string key in _ISetTemplates.Keys)
            {
                if (applyTo.Contains(key))
                {
                    STEM.Sys.IO.FileDescription d = _ISetTemplates[key];
                    if (d.StringContent.Contains(f))
                    {
                        modified[key] = d.StringContent.Replace(f, r);
                    }
                }
            }

            foreach (string key in _ISetStatics.Keys)
            {
                if (applyTo.Contains(key))
                {
                    STEM.Sys.IO.FileDescription d = _ISetStatics[key];
                    if (d.StringContent.Contains(f))
                    {
                        modified[key] = d.StringContent.Replace(f, r);
                    }
                }
            }

            foreach (DataGridViewRow row in configsFound.Rows)
            {
                if (modified.ContainsKey((string)row.Cells[1].Value))
                {
                    row.Cells[1].Style.ForeColor = Color.Red;
                }
            }

            if (MessageBox.Show(this, modified.Count + " files modified. Save changes?", "Results", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                foreach (DataGridViewRow row in configsFound.Rows)
                {
                    if (modified.ContainsKey((string)row.Cells[1].Value))
                    {
                        row.Cells[1].Style.ForeColor = Color.Black;
                    }
                }

                return;
            }
            
            foreach (string key in _DeploymentControllers.Keys)
            {
                if (modified.ContainsKey(key))
                {
                    STEM.Sys.IO.FileDescription d = _DeploymentControllers[key];
                    d.StringContent = modified[key];
                    d.LastWriteTimeUtc = DateTime.UtcNow;
                }
            }

            foreach (string key in _ISetTemplates.Keys)
            {
                if (modified.ContainsKey(key))
                {
                    STEM.Sys.IO.FileDescription d = _ISetTemplates[key];
                    d.StringContent = modified[key];
                    d.LastWriteTimeUtc = DateTime.UtcNow;
                }
            }

            foreach (string key in _ISetStatics.Keys)
            {
                if (modified.ContainsKey(key))
                {
                    STEM.Sys.IO.FileDescription d = _ISetStatics[key];
                    d.StringContent = modified[key];
                    d.LastWriteTimeUtc = DateTime.UtcNow;
                }
            }

            _UIActor.SubmitConfigurationUpdate();
        }
    }
}
