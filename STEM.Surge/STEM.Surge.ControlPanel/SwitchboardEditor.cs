using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using STEM.Sys.Security;

namespace STEM.Surge.ControlPanel
{
    public partial class SwitchboardEditor : UserControl
    {
        UIActor _UIActor;
        SwitchboardConfig _SwitchboardConfiguration = new SwitchboardConfig();

        public SwitchboardEditor(UIActor messageClient)
        {
            InitializeComponent();

            _UIActor = messageClient;
            
            _SwitchboardConfiguration.FileSources.FileSourcesRowDeleted += new SwitchboardConfig.FileSourcesRowChangeEventHandler(Cfg_RowChanged);
            _SwitchboardConfiguration.IpPreferenceMap.IpPreferenceMapRowDeleted += new SwitchboardConfig.IpPreferenceMapRowChangeEventHandler(Cfg_RowChanged);

            _SwitchboardConfiguration.FileSources.RowChanging += FileSources_RowChanging;

            fileSourcesDataTableBindingSource.DataSource = _SwitchboardConfiguration.FileSources;

            fileSourcesGridView.CellClick += new DataGridViewCellEventHandler(FileSourcesGridView_CellClick);
            fileSourcesGridView.CellEnter += new DataGridViewCellEventHandler(fileSourcesGridView_CellEnter);
            fileSourcesGridView.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(fileSourcesGridView_EditingControlShowing);
            fileSourcesGridView.CellFormatting += FileSourcesGridView_CellFormatting;
            fileSourcesGridView.RowHeaderMouseDoubleClick += fileSourcesGridView_RowHeaderMouseDoubleClick;                       
            fileSourcesGridView.DataError += fileSourcesGridView_DataError;

            Bind();
        }

        private void FileSourcesGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {            
            if (e.ColumnIndex == 0)
            {
                e.Value = Surge.ControlPanel.Properties.Resources.notes;
            }

            if (e.ColumnIndex == 17) // && !(e.Value is System.DBNull) && !String.IsNullOrEmpty((string)e.Value))
            {
                fileSourcesGridView.Rows[e.RowIndex].Tag = e.Value;

                if (!(e.Value is System.DBNull) && !String.IsNullOrEmpty((string)e.Value))
                    e.Value = new String('*', 5);
                else
                    e.Value = "";
            }

            if (fileSourcesGridView.Rows[e.RowIndex].IsNewRow && e.ColumnIndex == 13)
            {
                e.Value = "[MANAGERS]";
            }

            if (fileSourcesGridView.Rows[e.RowIndex].IsNewRow && e.ColumnIndex == 14)
            {
                e.Value = "[BRANCHES]";
            }
        }

        private void FileSourcesGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                string sourceDirectory = (string)fileSourcesGridView.Rows[e.RowIndex].Cells[2].Value;
                string directoryFilter = (string)fileSourcesGridView.Rows[e.RowIndex].Cells[3].Value;
                string fileFilter = (string)fileSourcesGridView.Rows[e.RowIndex].Cells[4].Value;

                string srID = GenerateSwitchboardRowIDs.GenerateSwitchboardRowID(sourceDirectory, directoryFilter, fileFilter);

                Notes n = new Notes(_UIActor, srID);
                n.ShowDialog(this);

                return;
            }
        }

        void fileSourcesGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DISCARD")
                return;

            if (e.Exception.Message.Contains("InvalidArgument=Value of"))
                return;

            MessageBox.Show(this, e.Exception.ToString());
        }

        void fileSourcesGridView_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            lock (fileSourcesGridView)
            {
                DataGridViewRow r = fileSourcesGridView.Rows[e.RowIndex];

                string d = r.Cells[8].Value.ToString();

                ControllerEditorForm cef = new ControllerEditorForm(_UIActor, d + ".dc");

                cef.Show(this);
            }
        }
                
        internal void Reload()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new System.Threading.ThreadStart(Reload));
            }
            else
            {
                if (_Initialized)
                {
                    if (IsDirty)
                        MessageBox.Show(this, "The Switchboard Configuration has been updated and will be reloaded.", "Changes will be lost", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                Bind();
            }
        }

        bool _DirtyConfig = false;
        public bool IsDirty
        {
            get
            {
                return _DirtyConfig;
            }
        }
        
        bool _Initialized = false;
        public void Bind()
        {
            if (_UIActor.DeploymentManagerConfiguration.MessageConnection == null || _UIActor.DeploymentManagerConfiguration.SwitchboardConfiguration == null)
                return;

            _Initialized = true;

            fileSourcesGridView.CurrentCellDirtyStateChanged -= new EventHandler(fileSourcesGridView_CurrentCellDirtyStateChanged);

            lock (_SwitchboardConfiguration)
                try
                {
                    UpdateControllerFilenames();

                    _SwitchboardConfiguration.Clear();
                    _SwitchboardConfiguration.Merge(_UIActor.DeploymentManagerConfiguration.SwitchboardConfiguration);

                    _SwitchboardConfiguration.AcceptChanges();

                    foreach (SwitchboardConfig.FileSourcesRow r in _SwitchboardConfiguration.FileSources)
                    {
                        if (r.IsImpersonateUserNull() || string.IsNullOrEmpty(r.ImpersonateUser.Trim()))
                            r.ImpersonationPassword = "";
                    }

                    synchronizedManagers.Text = _SwitchboardConfiguration.Settings[0].RedundantDeploymentManagers;
                    grafanaUrl.Text = _SwitchboardConfiguration.Settings[0].GrafanaURL;
                    offlineTol.Text = _SwitchboardConfiguration.Settings[0].OfflineTolerance.ToString();

                    if (_ManagerKVP == null)
                    {
                        _ManagerKVP = new ManagerKVP(_SwitchboardConfiguration);
                    }
                    else
                    {
                        _ManagerKVP.SwitchboardConfiguration = _SwitchboardConfiguration;
                    }

                    if (_IpPreferenceMap == null)
                    {
                        _IpPreferenceMap = new IpPreferenceMap(_SwitchboardConfiguration);
                    }
                    else
                    {
                        _IpPreferenceMap.SwitchboardConfiguration = _SwitchboardConfiguration;
                    }

                    _DirtyConfig = false;
                    Save.Enabled = false;
                    Cancel.Enabled = false;
                }
                finally
                {
                    fileSourcesGridView.CurrentCellDirtyStateChanged += new EventHandler(GridView_CurrentCellDirtyStateChanged);

                    filterMask_TextChanged(this, null);
                }
        }

        void GridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            _DirtyConfig = true;
            Save.Enabled = true;
            Cancel.Enabled = true;
        }
                
        void FileSources_RowChanging(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add || e.Action == DataRowAction.Change)
            {
                string errMsg = "";

                SwitchboardConfig.FileSourcesRow r = (SwitchboardConfig.FileSourcesRow)e.Row;

                if (r.SourceDirectory == "")
                {
                    errMsg += "Source Directory cannot be empty.\r\n";
                }

                if (r.DirectoryFilter == "")
                {
                    errMsg += "Directory Filter cannot be empty.\r\n";
                }

                if (r.FileFilter == "")
                {
                    errMsg += "File Filter cannot be empty.\r\n";
                }

                if (String.IsNullOrEmpty(r.ControllerFilename))
                {
                    errMsg += "Deployment Controller cannot be empty.\r\n";
                }

                if (errMsg != "")
                    if (MessageBox.Show(this, errMsg + "Discard Changes?", "Configuration Error", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                    }
                    else
                    {
                        throw new Exception("DISCARD");
                    }
            }
        }
        
        void fileSourcesGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            _DirtyConfig = true;
            Save.Enabled = true;
            Cancel.Enabled = true;
        }

        void fileSourcesGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 7)
                UpdateControllerFilenames();
        }

        void fileSourcesGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox t = e.Control as TextBox;
            if (t != null && fileSourcesGridView.CurrentCell.ColumnIndex == 17)
                t.UseSystemPasswordChar = true;
            else if (t != null)
                t.UseSystemPasswordChar = false;
        }

        void Cfg_RowChanged(object sender, object e)
        {
            _DirtyConfig = true;
            Save.Enabled = true;
            Cancel.Enabled = true;
        }

        private void filterMask_TextChanged(object sender, EventArgs e)
        {
            string f = filterMask.Text.Trim();

            if (f.Length == 0)
            {
                fileSourcesDataTableBindingSource.Filter = "";
            }
            else
            {
                f = f.Replace("]", "]]");
                f = f.Replace("[", "[[]");
                f = f.Replace("]]", "[]]");
                f = f.Replace("*", "[*]");
                f = f.Replace("%", "[%]");

                fileSourcesDataTableBindingSource.Filter =
                    "SourceDirectory LIKE '%" + f + "%' OR " +
                    "DirectoryFilter LIKE '%" + f + "%' OR " +
                    "FileFilter LIKE '%" + f + "%' OR " +
                    "ControllerFilename LIKE '%" + f + "%'";
            }
        }

        void UpdateControllerFilenames()
        {
            if (_UIActor.DeploymentManagerConfiguration.MessageConnection == null || _UIActor.DeploymentManagerConfiguration.SwitchboardConfiguration == null)
                return;

            List<string> cf = _UIActor.DeploymentManagerConfiguration.SwitchboardConfiguration.FileSources.Select(i => i.ControllerFilename).ToList();
            cf.AddRange(_UIActor.DeploymentManagerConfiguration.DeploymentControllers.Select(i => STEM.Sys.IO.Path.GetFileNameWithoutExtension(i.Filename)));

            cf = cf.Distinct().ToList();

            ControllerFilename.Items.Clear();
            cf.Sort();
            ControllerFilename.Items.AddRange(cf.ToArray());
        }
        
        public void Save_Click(object sender, EventArgs e)
        {
            lock (_SwitchboardConfiguration)
            {
                foreach (DataGridViewRow r in fileSourcesGridView.Rows)
                {
                    if ((string)r.Cells[2].Value == "")
                    {
                        if (fileSourcesGridView.Rows.Count > 1)
                        {
                            MessageBox.Show(this, "You must enter a Source Directory.", "Configuration Error", MessageBoxButtons.OK);
                            return;
                        }
                    }

                    if ((string)r.Cells[3].Value == "")
                    {
                        MessageBox.Show(this, "You must enter a Directory Filter.", "Configuration Error", MessageBoxButtons.OK);
                        return;
                    }

                    if ((string)r.Cells[4].Value == "")
                    {
                        MessageBox.Show(this, "You must enter a File Filter.", "Configuration Error", MessageBoxButtons.OK);
                        return;
                    }

                    if (r.Cells[8].Value is DBNull)
                    {
                        MessageBox.Show(this, "You must select a Deployment Controller.", "Configuration Error", MessageBoxButtons.OK);
                        return;
                    }
                }

                fileSourcesGridView.EndEdit();
                fileSourcesDataTableBindingSource.EndEdit();

                if (_SwitchboardConfiguration.FileSources.Count == 1 && _SwitchboardConfiguration.FileSources[0].SourceDirectory == "")
                {
                    _SwitchboardConfiguration.FileSources.Clear();
                }

                foreach (SwitchboardConfig.FileSourcesRow r in _SwitchboardConfiguration.FileSources)
                {
                    if (r.RowState == DataRowState.Deleted)
                        continue;

                    if (r.IsImpersonateUserNull() || String.IsNullOrEmpty(r.ImpersonateUser.Trim()))
                        r.ImpersonationPassword = "";

                    if (!r.IsImpersonationPasswordNull() && !String.IsNullOrEmpty(r.ImpersonationPassword.Trim()))
                        r.ImpersonationPassword = r.Entangle(r.ImpersonationPassword);
                }

                _SwitchboardConfiguration.AcceptChanges();

                _DirtyConfig = false;

                _UIActor.SubmitSwitchboardConfigurationUpdate(_SwitchboardConfiguration);

                Save.Enabled = false;
                Cancel.Enabled = false;
            }
        }

        public void Cancel_Click(object sender, EventArgs e)
        {
            Bind();
        }

        private void clearFilter_Click(object sender, EventArgs e)
        {
            filterMask.Text = "";
        }

        private void synchronizedManagers_TextChanged(object sender, EventArgs e)
        {
            if (_UIActor.DeploymentManagerConfiguration.MessageConnection == null || _UIActor.DeploymentManagerConfiguration.SwitchboardConfiguration == null)
                return;

            if (_SwitchboardConfiguration.Settings.Count == 0)
                _SwitchboardConfiguration.Settings.AddSettingsRow("", 30, "localhost:8080");

            if (_SwitchboardConfiguration.Settings[0].RedundantDeploymentManagers != synchronizedManagers.Text.Trim())
            {
                _SwitchboardConfiguration.Settings[0].RedundantDeploymentManagers = synchronizedManagers.Text.Trim();
                _DirtyConfig = true;
                Save.Enabled = true;
                Cancel.Enabled = true;
            }
        }

        private void grafanaUrl_TextChanged(object sender, EventArgs e)
        {
            if (_UIActor.DeploymentManagerConfiguration.MessageConnection == null || _UIActor.DeploymentManagerConfiguration.SwitchboardConfiguration == null)
                return;

            if (_SwitchboardConfiguration.Settings.Count == 0)
                _SwitchboardConfiguration.Settings.AddSettingsRow("", 30, "localhost:8080");

            if (_SwitchboardConfiguration.Settings[0].GrafanaURL != grafanaUrl.Text.Trim())
            {
                _SwitchboardConfiguration.Settings[0].GrafanaURL = grafanaUrl.Text.Trim();
                _DirtyConfig = true;
                Save.Enabled = true;
                Cancel.Enabled = true;
            }
        }

        private void offlineTol_TextChanged(object sender, EventArgs e)
        {
            if (_UIActor.DeploymentManagerConfiguration.MessageConnection == null || _UIActor.DeploymentManagerConfiguration.SwitchboardConfiguration == null)
                return;

            if (_SwitchboardConfiguration.Settings[0].OfflineTolerance != Int32.Parse(offlineTol.Text.Trim()))
            {
                _SwitchboardConfiguration.Settings[0].OfflineTolerance = Int32.Parse(offlineTol.Text.Trim());
                _DirtyConfig = true;
                Save.Enabled = true;
                Cancel.Enabled = true;
            }
        }

        private void PostMortemCache_TextChanged(object sender, EventArgs e)
        {
            if (_UIActor.DeploymentManagerConfiguration.MessageConnection == null || _UIActor.DeploymentManagerConfiguration.SwitchboardConfiguration == null)
                return;

            if (_SwitchboardConfiguration.Settings.Count == 0)
                _SwitchboardConfiguration.Settings.AddSettingsRow("", 30, "localhost:8080");

            //if (_SwitchboardConfiguration.Settings[0].PostMortemCacheDirectory != postMortemCache.Text.Trim())
            //{
            //    _SwitchboardConfiguration.Settings[0].PostMortemCacheDirectory = postMortemCache.Text.Trim();
            //    _DirtyConfig = true;
            //    Save.Enabled = true;
            //    Cancel.Enabled = true;
            //}
        }

        ManagerKVP _ManagerKVP = null;

        private void ManagerMacroMap_Click(object sender, EventArgs e)
        {
            _ManagerKVP.Show(this);
            _ManagerKVP.FormClosed += _ManagerKVP_FormClosed;
        }

        private void _ManagerKVP_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_ManagerKVP.IsDirty)
            {
                _DirtyConfig = true;
                Save.Enabled = true;
                Cancel.Enabled = true;
            }

            _ManagerKVP = new ManagerKVP(_SwitchboardConfiguration);
        }


        IpPreferenceMap _IpPreferenceMap = null;

        private void IpPreferenceMap_Click(object sender, EventArgs e)
        {
            _IpPreferenceMap.Show(this);
            _IpPreferenceMap.FormClosed += _IpPreferenceMap_FormClosed;
        }

        private void _IpPreferenceMap_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_IpPreferenceMap.IsDirty)
            {
                _DirtyConfig = true;
                Save.Enabled = true;
                Cancel.Enabled = true;
            }

            _IpPreferenceMap = new IpPreferenceMap(_SwitchboardConfiguration);
        }

        private void managerConfiguration_Click(object sender, EventArgs e)
        {
            STEM.Surge.Messages.SetServiceConfiguration setConfiguration = new STEM.Surge.Messages.SetServiceConfiguration();

            STEM.Surge.Messages.GetServiceConfiguration m = new STEM.Surge.Messages.GetServiceConfiguration();
            m.MachineIP = _UIActor.PrimaryDeploymentManagerIP;

            STEM.Sys.Messaging.Message r = _UIActor.Send(m, TimeSpan.FromSeconds(5));

            if (r is STEM.Surge.Messages.GetServiceConfiguration)
            {
                m = r as STEM.Surge.Messages.GetServiceConfiguration;
                ServiceConfiguration bc = new ServiceConfiguration(m);

                if (bc.ShowDialog(this) == DialogResult.Cancel)
                    return;

                setConfiguration.MachineIP = _UIActor.PrimaryDeploymentManagerIP;
                setConfiguration.SurgeCommunicationPort = bc.SurgeCommunicationPort;
                setConfiguration.SurgeDeploymentManagerAddress = bc.SurgeDeploymentManagerAddress;
                setConfiguration.ProcessorOverload = bc.ProcessorOverload;
                setConfiguration.PostMortemDirectory = bc.PostMortemDirectory;
                setConfiguration.RemoteConfigurationDirectory = bc.RemoteConfigurationDirectory;
                setConfiguration.UseSSL = bc.UseSSL;

                _UIActor.Send(setConfiguration);
            }
            else
            {
                MessageBox.Show(this, "The server did not respond. Please try again.", "Message Timeout", MessageBoxButtons.OK);
            }
        }
    }
}