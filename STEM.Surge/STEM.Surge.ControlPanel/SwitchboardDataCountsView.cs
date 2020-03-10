using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Windows;
using STEM.Sys;
using STEM.Surge;
using STEM.Surge.Messages;

namespace STEM.Surge.ControlPanel
{
    public partial class SwitchboardDataCountsView : UserControl
    {
        internal bool _PauseSourceCountsRefresh = false;

        UIActor _UIActor;

        public SwitchboardDataCountsView(UIActor messageClient)
        {
            InitializeComponent();

            _UIActor = messageClient;
            
            switchboardDataCountsBindingSource.DataSource = _SwitchboardDataCountsDataTable;
            switchboardDataCountsBindingSource.SuspendBinding();
            switchboardDataCountsBindingSource.Sort = "Backlog DESC";

            switchboardDataCountsGridView.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(switchboardDataCountsGridView_DataBindingComplete);
            switchboardDataCountsGridView.CellContentClick += new DataGridViewCellEventHandler(switchboardDataCountsGridView_CellContentClick);
            
            switchboardDataCountsGridView.RowHeaderMouseDoubleClick += new DataGridViewCellMouseEventHandler(switchboardDataCountsGridView_RowHeaderMouseDoubleClick);
                        
            sourceRowMenu.ItemClicked += new ToolStripItemClickedEventHandler(sourceRowMenu_ItemClicked);

            Type dgt = switchboardDataCountsGridView.GetType();
            PropertyInfo dgtPi = dgt.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            dgtPi.SetValue(switchboardDataCountsGridView, true);
        }

        void switchboardDataCountsGridView_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            lock (switchboardDataCountsGridView)
            {
                DataGridViewRow r = switchboardDataCountsGridView.Rows[e.RowIndex];

                SourceRowPollerDetails pd = new SourceRowPollerDetails(_UIActor, r.Cells[20].Value.ToString());
                pd.Start(this);
            }
        }
                          
        void sourceRowMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                lock (switchboardDataCountsGridView)
                {
                    foreach (DataGridViewRow r in switchboardDataCountsGridView.SelectedRows)
                    {
                        if (e.ClickedItem.Name == "countDetails")
                        {
                            SourceRowPollerDetails pd = new SourceRowPollerDetails(_UIActor, r.Cells[20].Value.ToString());
                            pd.Start(this);
                        }

                        if (e.ClickedItem.Name == "editController")
                        {
                            ControllerEditorForm cef = new ControllerEditorForm(_UIActor, r.Cells[2].Value.ToString() + ".dc");
                            cef.Show(this);
                        }
                    }
                }
            }
            catch { }
        }

        public void SetBindingSource(TableDataSources.SwitchboardDataCountsDataTable switchboardDataCountsDataTable, List<Backlogs.Entry> backlogs)
        {
            if (!_PauseSourceCountsRefresh)
                _SwitchboardDataCountsDataTable = switchboardDataCountsDataTable;

            pollerCountLabel.Text = backlogs.Count.ToString();
            totalHealthyPollers.Text = backlogs.Count(i => i.Enabled && !i.PingFailure && !i.PollFailure && !i.ControllerLoadError).ToString();

            RefreshTables();
        }

        TableDataSources.SwitchboardDataCountsDataTable _SwitchboardDataCountsDataTable = new Surge.ControlPanel.TableDataSources.SwitchboardDataCountsDataTable();
        public void RefreshTables()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ThreadStart(RefreshTables));
            }
            else
            {
                try
                {
                    lock (_SwitchboardDataCountsDataTable)
                    {
                        int sortColumn = -1;
                        SortOrder sortDirection = SortOrder.Ascending;

                        if (switchboardDataCountsGridView.SortedColumn != null)
                        {
                            sortColumn = sortColumn = switchboardDataCountsGridView.SortedColumn.Index;
                            sortDirection = switchboardDataCountsGridView.SortOrder;
                        }

                        switchboardDataCountsGridView.SuspendLayout();
                        try
                        {
                            List<string> selectedRows = new List<string>();
                            foreach (DataGridViewRow r in switchboardDataCountsGridView.SelectedRows)
                                selectedRows.Add(r.Cells[20].Value.ToString());

                            int vs = switchboardDataCountsGridView.FirstDisplayedScrollingRowIndex;
                            int hs = switchboardDataCountsGridView.FirstDisplayedScrollingColumnIndex;

                            switchboardDataCountsBindingSource.Filter = "";

                            if (!_PauseSourceCountsRefresh)
                                switchboardDataCountsBindingSource.DataSource = _SwitchboardDataCountsDataTable;

                            string f = filterTextbox.Text.Trim();

                            string filter = "";
                            if (f.Length > 0)
                            {
                                f = f.Replace("]", "]]");
                                f = f.Replace("[", "[[]");
                                f = f.Replace("]]", "[]]");
                                f = f.Replace("*", "[*]");
                                f = f.Replace("%", "[%]");

                                filter =
                                    "(Description LIKE '%" + f + "%' OR " +
                                    "Controller LIKE '%" + f + "%')";
                            }

                            if (_HideZeros)
                            {
                                if (filter.Length > 0)
                                    filter += " AND ";

                                filter += "(Backlog > 0 OR Assigned > 0 OR Processing > 0)";
                            }

                            switchboardDataCountsBindingSource.Filter = filter;

                            if (switchboardDataCountsGridView.Rows.Count > 0)
                            {
                                if (sortColumn > -1)
                                {
                                    switch (sortDirection)
                                    {
                                        case SortOrder.Ascending:
                                            switchboardDataCountsGridView.Sort(switchboardDataCountsGridView.Columns[sortColumn], System.ComponentModel.ListSortDirection.Ascending);
                                            break;
                                        case SortOrder.Descending:
                                            switchboardDataCountsGridView.Sort(switchboardDataCountsGridView.Columns[sortColumn], System.ComponentModel.ListSortDirection.Descending);
                                            break;
                                        case SortOrder.None:
                                            break;
                                    }
                                }

                                if (vs > -1 && vs < switchboardDataCountsGridView.RowCount)
                                    switchboardDataCountsGridView.FirstDisplayedScrollingRowIndex = vs;
                                if (hs > -1 && hs < switchboardDataCountsGridView.ColumnCount)
                                    switchboardDataCountsGridView.FirstDisplayedScrollingColumnIndex = hs;

                                switchboardDataCountsGridView.ClearSelection();

                                foreach (DataGridViewRow r in switchboardDataCountsGridView.Rows)
                                    if (selectedRows.Contains(r.Cells[20].Value.ToString()))
                                        r.Selected = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            STEM.Sys.EventLog.WriteEntry("Surge.ControlPanel.ActivityControl.RefreshGrids.RefreshDetails", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                        }
                        finally
                        {
                            switchboardDataCountsGridView.ResumeLayout();
                        }
                    }
                }
                catch (Exception ex)
                {
                    STEM.Sys.EventLog.WriteEntry("Surge.SwitchboardDataCountsView.RefreshTables", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                }
            }
        }
                
        private void pauseSourceCountsButton_Click(object sender, EventArgs e)
        {
            if (_PauseSourceCountsRefresh)
            {
                _PauseSourceCountsRefresh = false;
                pauseSourceCountsButton.Image = Surge.ControlPanel.Properties.Resources.pause;
            }
            else
            {
                _PauseSourceCountsRefresh = true;
                pauseSourceCountsButton.Image = Surge.ControlPanel.Properties.Resources.play;
            }
        }

        void switchboardDataCountsGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex != 0)
                    return;

                if (_UIActor.DeploymentManagerConfiguration.MessageConnection == null || _UIActor.DeploymentManagerConfiguration.SwitchboardConfiguration == null)
                    return;

                string switchboardRowID = switchboardRowID = switchboardDataCountsGridView[20, e.RowIndex].Value.ToString();
                
                DataGridViewRow row = switchboardDataCountsGridView.Rows[e.RowIndex];

                SwitchboardConfig cfg = new SwitchboardConfig();
                cfg.Merge(_UIActor.DeploymentManagerConfiguration.SwitchboardConfiguration);
                cfg.AcceptChanges();

                foreach (SwitchboardConfig.FileSourcesRow fsr in cfg.FileSources)
                {
                    if (switchboardRowID == STEM.Surge.GenerateSwitchboardRowIDs.GenerateSwitchboardRowID(fsr))
                    {
                        fsr.Enable = !fsr.Enable;

                        cfg.AcceptChanges();

                        if (!_UIActor.SubmitSwitchboardConfigurationUpdate(cfg, true))
                        {
                            MessageBox.Show(this, "There was an error saving the configuration!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        ((DataGridViewCheckBoxCell)row.Cells[0]).Value = fsr.Enable;

                        break;
                    }
                }

                if ((bool)((DataGridViewCheckBoxCell)row.Cells[0]).Value == false)
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.Gainsboro;
                else
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            }
            catch { }
        }

        void switchboardDataCountsGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                List<DataGridViewRow> del = new List<DataGridViewRow>();
                for (int row = 0; row < switchboardDataCountsGridView.Rows.Count; row++)
                {
                    string switchboardRowID = switchboardDataCountsGridView[20, row].Value.ToString();
                    
                    DataGridViewCell valueCell = switchboardDataCountsGridView[19, row];
                    DataGridViewImageCell displayCell = (DataGridViewImageCell)switchboardDataCountsGridView[1, row];

                    displayCell.ToolTipText = _SwitchboardDataCountsDataTable.Rows.Find(switchboardRowID)["PollError"].ToString().Trim(); 

                    if (displayCell.ToolTipText.Length == 0)
                    {
                        if ((bool)valueCell.Value)
                            displayCell.Value = Surge.ControlPanel.Properties.Resources.ok;
                        else
                            displayCell.Value = Surge.ControlPanel.Properties.Resources.ok_grey;
                    }
                    else
                    {
                        if ((bool)valueCell.Value)
                            displayCell.Value = Surge.ControlPanel.Properties.Resources.ok_e;
                        else
                            displayCell.Value = Surge.ControlPanel.Properties.Resources.ok_grey_e;
                    }

                    if ((bool)((DataGridViewCheckBoxCell)switchboardDataCountsGridView[0, row]).Value == false)
                        switchboardDataCountsGridView.Rows[row].DefaultCellStyle.BackColor = System.Drawing.Color.Gainsboro;
                    else
                        switchboardDataCountsGridView.Rows[row].DefaultCellStyle.BackColor = System.Drawing.Color.White;
                }
            }
            catch { }
        }

        internal bool EnabledRowsOnly = false;
        bool _HideZeros = false;

        private void hideDisabled_CheckedChanged(object sender, EventArgs e)
        {
            EnabledRowsOnly = hideDisabled.Checked;
        }

        private void hideZeroCounts_CheckedChanged(object sender, EventArgs e)
        {
            _HideZeros = hideZeroCounts.Checked;
        }
        
        private void clearSourceFilter_Click(object sender, EventArgs e)
        {
            filterTextbox.Text = "";
        }

        private void pollingInfo_Click(object sender, EventArgs e)
        {
            List<Backlogs.Entry> backlogs = _UIActor.Backlogs;

            Backlogs.Entry mDisabled = backlogs.Where(i => !i.Enabled).OrderBy(i => i.LastPoll).FirstOrDefault();
            Backlogs.Entry mPollFailure = backlogs.Where(i => i.PollFailure).OrderBy(i => i.LastPoll).FirstOrDefault();
            int data24hrs = backlogs.Count(i => !i.PollFailure && i.Enabled && i.LastPoll > DateTime.MinValue && (DateTime.UtcNow - i.LastDataActivity).TotalHours < 24);
            int data1hrs = backlogs.Count(i => !i.PollFailure && i.Enabled && i.LastPoll > DateTime.MinValue && (DateTime.UtcNow - i.LastDataActivity).TotalHours < 1);
            int data5min = backlogs.Count(i => !i.PollFailure && i.Enabled && i.LastPoll > DateTime.MinValue && (DateTime.UtcNow - i.LastDataActivity).TotalMinutes < 5);

            string message = "Polling: " + backlogs.Count() + "\r\n";
            message += "Enabled: " + backlogs.Count(i => i.Enabled == true) + "\r\n";
            message += "Poll Failures: " + backlogs.Count(i => i.PollFailure == true) + "\r\n";
            message += "With Data (Within 24 Hours): " + backlogs.Count(i => i.LastDataActivity > DateTime.MinValue && (DateTime.UtcNow - i.LastDataActivity).TotalHours < 24) + "\r\n";
            message += "Oldest Poll (Assignable with data 24hrs): " + ((data24hrs > 0) ? backlogs.Where(i => !i.PollFailure && i.Enabled && i.LastPoll > DateTime.MinValue && (DateTime.UtcNow - i.LastDataActivity).TotalHours < 24).Min(i => i.LastPoll).ToString("G") : "") + "\r\n";
            message += "Oldest Poll (Assignable with data 1hr): " + ((data1hrs > 0) ? backlogs.Where(i => !i.PollFailure && i.Enabled && i.LastPoll > DateTime.MinValue && (DateTime.UtcNow - i.LastDataActivity).TotalHours < 1).Min(i => i.LastPoll).ToString("G") : "") + "\r\n";
            message += "Oldest Poll (Assignable with data 5min): " + ((data5min > 0) ? backlogs.Where(i => !i.PollFailure && i.Enabled && i.LastPoll > DateTime.MinValue && (DateTime.UtcNow - i.LastDataActivity).TotalMinutes <= 5).Min(i => i.LastPoll).ToString("G") : "") + "\r\n";
            message += "Oldest Poll (Poll Failure): " + ((mPollFailure == null) ? "" : mPollFailure.LastPoll.ToString("G")) + "\r\n";
            message += "Oldest Poll (Disabled): " + ((mDisabled == null) ? "" : mDisabled.LastPoll.ToString("G")) + "\r\n";

            MessageBox.Show(this, message, "Polling Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
