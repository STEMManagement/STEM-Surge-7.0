using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using STEM.Sys;
using STEM.Surge;
using STEM.Surge.Messages;

namespace STEM.Surge.ControlPanel
{
    public partial class SourceRowPollerDetails : Form
    {
        bool _Stop = false;
        UIActor _UIActor;
        string _SwitchboardID;
        System.Timers.Timer _RefreshTimer = null;

        public SourceRowPollerDetails(UIActor messageClient, string switchboardID)
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(SourceRowPollerDetails_FormClosing);
            _UIActor = messageClient;
            _SwitchboardID = switchboardID;

            pollerDetailsBindingSource.Sort = "Backlog DESC";

            pollerDetailsGridView.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(pollerDetailsGridView_DataBindingComplete);

            pollerDetailsGridView.RowHeaderMouseDoubleClick += new DataGridViewCellMouseEventHandler(pollerDetailsGridView_RowHeaderMouseDoubleClick);
        }

        void pollerDetailsGridView_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            lock (pollerDetailsGridView)
            {
                DataGridViewRow r = pollerDetailsGridView.Rows[e.RowIndex];

                string d = r.Cells[0].Value.ToString();

                int i = d.IndexOf("(Directory Filter:");

                try
                {
                    System.Diagnostics.Process.Start(d.Substring(0, i).Trim());
                }
                catch
                {
                    MessageBox.Show(this, "Cannot reach: " + d.Substring(0, i).Trim(), "Directory Unreachable", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        void SourceRowPollerDetails_FormClosing(object sender, FormClosingEventArgs e)
        {
            _Stop = true;
        }

        public void Start(UserControl parent)
        {
            lock (this)
            {
                if (_RefreshTimer != null)
                    return;

                _RefreshTimer = new System.Timers.Timer(500);
            }

            this.Show(parent);

            _RefreshTimer.AutoReset = false;
            _RefreshTimer.Elapsed += new System.Timers.ElapsedEventHandler(_RefreshTimer_Elapsed);
            _RefreshTimer.Enabled = true;
        }
        
        void _RefreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                RefreshGrid();
            }
            catch { _RefreshTimer.Enabled = !_Stop; }
        }

        string _ControllerName = "";
        TableDataSources.SwitchboardDataCountsDataTable _SwitchboardDataCountsDataTable = new TableDataSources.SwitchboardDataCountsDataTable();
        void RefreshGrid()
        {
            if (InvokeRequired)
            {
                try
                {
                    _SwitchboardDataCountsDataTable.Clear();

                    List<Backlogs.Entry> backlogs = _UIActor.BacklogsBySwitchboardRowID(_SwitchboardID);
                    List<ActiveDeployments.Entry> deployments = _UIActor.DeploymentsBySwitchboardRowID(_SwitchboardID);

                    foreach (Backlogs.Entry x in backlogs)
                    {
                        double avgExecutionTime = 0;
                        IEnumerable<TimeSpan> deltas = deployments.Where(i => i.DeploymentControllerID == x.DeploymentControllerID && i.Received > DateTime.MinValue && i.Completed > DateTime.MinValue).Select(i => i.Completed - i.Received);
                        if (deltas.Count() > 0)
                        {
                            TimeSpan a = new TimeSpan((long)deltas.Select(i => i.Ticks).Average());
                            avgExecutionTime = a.TotalSeconds;
                        }

                        _ControllerName = x.DeploymentController;

                        if (_SwitchboardDataCountsDataTable.FindByID(x.DeploymentControllerID) == null)
                            _SwitchboardDataCountsDataTable.AddSwitchboardDataCountsRow(
                                        x.Enabled,
                                        x.DeploymentControllerDescription,
                                        x.DeploymentController,
                                        x.BacklogCount,
                                        x.PerceivedBacklogCount,
                                        deployments.Count(i => i.DeploymentControllerID == x.DeploymentControllerID && i.Completed == DateTime.MinValue),
                                        deployments.Count(i => i.DeploymentControllerID == x.DeploymentControllerID && i.Completed == DateTime.MinValue & i.Received > DateTime.MinValue),
                                        x.MaxBranchLoad,
                                        x.LastPoll,
                                        x.LastPoll,
                                        x.LastPoll,
                                        x.LastPoll,
                                        x.LastWalkStart,
                                        x.LastAssignment,
                                        1,
                                        1,
                                        x.Assigning ? 1 : 0,
                                        x.ListWalkSummary,
                                        x.PollError,
                                        avgExecutionTime,
                                        deltas.Count(),
                                        x.Assigning,
                                        x.DeploymentControllerID);
                    }
                }
                catch (Exception ex)
                {
                    STEM.Sys.EventLog.WriteEntry("SourceRowPollerDetails.RefreshGrid", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                }

                BeginInvoke(new ThreadStart(RefreshGrid));
            }
            else
            {
                try
                {
                    List<string> selectedSourceRows = new List<string>();
                    foreach (DataGridViewRow r in pollerDetailsGridView.SelectedRows)
                        selectedSourceRows.Add(r.Cells[0].Value.ToString());

                    int vs = pollerDetailsGridView.FirstDisplayedScrollingRowIndex;
                    int hs = pollerDetailsGridView.FirstDisplayedScrollingColumnIndex;
                    
                    _TableDataSources.SwitchboardDataCounts.Merge(_SwitchboardDataCountsDataTable);

                    string f = filterMask.Text.Trim();
                    if (f.Length > 0)
                    {
                        f = f.Replace("]", "]]");
                        f = f.Replace("[", "[[]");
                        f = f.Replace("]]", "[]]");
                        f = f.Replace("*", "[*]");
                        f = f.Replace("%", "[%]");

                        pollerDetailsBindingSource.Filter = "(Description LIKE '%" + f + "%' OR Controller LIKE '%" + f + "%')";
                    }
                    else
                    {
                        pollerDetailsBindingSource.Filter = "";
                    }

                    if (pollerDetailsGridView.Rows.Count > 0)
                        pollerDetailsGridView.Rows[0].Cells[0].Selected = false;

                    if (vs > -1 && vs <= (pollerDetailsGridView.RowCount - 1))
                        pollerDetailsGridView.FirstDisplayedScrollingRowIndex = vs;
                    if (hs > -1 && hs <= (pollerDetailsGridView.ColumnCount - 1))
                        pollerDetailsGridView.FirstDisplayedScrollingColumnIndex = hs;
                }
                finally
                {
                    _RefreshTimer.Enabled = !_Stop;
                }
            }
        }

        void pollerDetailsGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                for (int row = 0; row < pollerDetailsGridView.Rows.Count; row++)
                {
                    string deploymentManagerID = pollerDetailsGridView[13, row].Value.ToString();
                    DataGridViewCell valueCell = pollerDetailsGridView[12, row];
                    DataGridViewImageCell displayCell = (DataGridViewImageCell)pollerDetailsGridView[11, row];

                    displayCell.ToolTipText = _TableDataSources.SwitchboardDataCounts.Rows.Find(deploymentManagerID)["PollError"].ToString().Trim();

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

                    string listWalkSummary = _TableDataSources.SwitchboardDataCounts.Rows.Find(deploymentManagerID)["ListWalkSummary"].ToString().Trim();
                    for (int x = 0; x < 11; x++)
                    {
                        var c = pollerDetailsGridView[x, row];
                        c.ToolTipText = listWalkSummary;
                    }
                }
            }
            catch { }
        }

        private void clearFilter_Click(object sender, EventArgs e)
        {
            filterMask.Text = "";
        }

        private void openController_Click(object sender, EventArgs e)
        {
            ControllerEditorForm f = new ControllerEditorForm(_UIActor, _ControllerName + ".dc");
            f.Show(this);
        }
    }
}
