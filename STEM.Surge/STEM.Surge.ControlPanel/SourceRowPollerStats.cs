using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using STEM.Surge.Messages;

namespace STEM.Surge.ControlPanel
{
    public partial class SourceRowPollerStats : UserControl
    {
        bool _Stop = false;
        UIActor _UIActor;
        System.Timers.Timer _RefreshTimer = null;

        public SourceRowPollerStats(UIActor messageClient)
        {
            InitializeComponent();

            _UIActor = messageClient;

            pollerDetailsBindingSource.Sort = "Backlog DESC";

            pollerDetailsGridView.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(pollerDetailsGridView_DataBindingComplete);

            pollerDetailsGridView.RowHeaderMouseDoubleClick += new DataGridViewCellMouseEventHandler(pollerDetailsGridView_RowHeaderMouseDoubleClick);

            tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
        }

        string _SelectedTab = "activePollers";
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _SelectedTab = tabControl1.SelectedTab.Name;
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

        public void Stop()
        {
            lock (this)
            {
                _Stop = true;
            }
        }
        
        public void Start()
        {
            lock (this)
            {
                _Stop = false;

                if (_RefreshTimer != null)
                {
                    _RefreshTimer.Enabled = true;
                    return;
                }

                _RefreshTimer = new System.Timers.Timer(500);
            }

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
            catch
            {
                lock (this)
                {
                    if (_RefreshTimer != null)
                        _RefreshTimer.Enabled = !_Stop;
                }
            }
        }

        TableDataSources.SwitchboardDataCountsDataTable _SwitchboardDataCountsDataTable = new TableDataSources.SwitchboardDataCountsDataTable();
        void RefreshGrid()
        {
            if (InvokeRequired)
            {
                try
                {
                    _SwitchboardDataCountsDataTable.Clear();

                    List<Backlogs.Entry> backlogs = new List<Backlogs.Entry>();
                    List<ActiveDeployments.Entry> deployments = new List<ActiveDeployments.Entry>();

                    bool groupBySwitchboardRowID = false;

                    switch (_SelectedTab)
                    {
                        case "activePollers":
                            backlogs = _UIActor.BacklogsFromActiveAssigners();
                            deployments = _UIActor.DeploymentsByDeploymentController(backlogs.Select(i => i.DeploymentControllerID).Distinct().ToList());
                            break;

                        case "stuckPollers":
                            backlogs = _UIActor.BacklogsFromActiveAssigners().Where(i => (DateTime.UtcNow - i.LastWalkStart).TotalMinutes > 1).ToList();
                            deployments = _UIActor.DeploymentsByDeploymentController(backlogs.Select(i => i.DeploymentControllerID).Distinct().ToList());
                            break;

                        case "deadPollers":
                            groupBySwitchboardRowID = true;
                            backlogs = _UIActor.Backlogs.Where(i => i.Enabled == true).ToList();

                            foreach (string x in backlogs.Select(i => i.SwitchboardRowID).Distinct().ToList())
                            {
                                DateTime t = backlogs.Where(i => i.SwitchboardRowID == x).Max(i => i.LastAssignment);

                                if (t > DateTime.MinValue)
                                    backlogs.RemoveAll(i => i.SwitchboardRowID == x);
                            }

                            break;
                    }

                    foreach (Backlogs.Entry x in backlogs)
                    {
                        double avgExecutionTime = 0;

                        if (groupBySwitchboardRowID)
                        {
                            if (_SwitchboardDataCountsDataTable.FindByID(x.SwitchboardRowID) != null)
                                continue;

                            List<Backlogs.Entry> tmp = backlogs.Where(i => i.SwitchboardRowID == x.SwitchboardRowID).ToList();

                            if (tmp.Count == 0)
                                continue;

                            string toolTip = "";
                            if (tmp.Count(i => i.PollError != null && i.PollError.Trim() != "") > 0)
                                toolTip = String.Join("\r\n", tmp.Where(i => i.PollError != null && i.PollError.Trim() != "").Select(i => i.PollError).ToArray());

                            DateTime maxHealthy = DateTime.MinValue;
                            DateTime minHealthy = DateTime.MinValue;
                            if (tmp.Count(i => !i.PingFailure && !i.PollFailure && !i.ControllerLoadError) > 0)
                            {
                                maxHealthy = tmp.Where(i => !i.PingFailure && !i.PollFailure && !i.ControllerLoadError).Max(i => i.LastPoll);
                                minHealthy = tmp.Where(i => !i.PingFailure && !i.PollFailure && !i.ControllerLoadError).Min(i => i.LastPoll);
                            }

                            _SwitchboardDataCountsDataTable.AddSwitchboardDataCountsRow(
                                                true,
                                                tmp.First().SwitchboardRowDescription,
                                                tmp.First().DeploymentController,
                                                tmp.Sum(i => i.BacklogCount),
                                                tmp.Sum(i => i.PerceivedBacklogCount),
                                                0,
                                                0,
                                                tmp.First().MaxBranchLoad,
                                                maxHealthy,
                                                tmp.Max(i => i.LastPoll),
                                                minHealthy,
                                                tmp.Min(i => i.LastPoll),
                                                tmp.Max(i => i.LastWalkStart),
                                                tmp.Max(i => i.LastAssignment),
                                                tmp.Count(),
                                                tmp.Count(i => !i.PingFailure && !i.PollFailure && !i.ControllerLoadError),
                                                tmp.Count(i => i.Assigning),
                                                "",
                                                toolTip,
                                                avgExecutionTime,
                                                0,
                                                tmp.Any(i => i.Assigning),
                                                x.SwitchboardRowID);
                        }
                        else
                        {
                            IEnumerable<TimeSpan> deltas = deployments.Where(i => i.DeploymentControllerID == x.DeploymentControllerID && i.Received > DateTime.MinValue && i.Completed > DateTime.MinValue).Select(i => i.Completed - i.Received);
                            if (deltas.Count() > 0)
                            {
                                TimeSpan a = new TimeSpan((long)deltas.Select(i => i.Ticks).Average());
                                avgExecutionTime = a.TotalSeconds;
                            }

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
                }
                catch (Exception ex)
                {
                    STEM.Sys.EventLog.WriteEntry("SourceRowPollerStats.RefreshGrid", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
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

                    _TableDataSources.SwitchboardDataCounts.Clear();
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
                    lock (this)
                    {
                        _RefreshTimer.Enabled = !_Stop;
                    }
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
    }
}
