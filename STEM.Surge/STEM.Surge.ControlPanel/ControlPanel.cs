using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using STEM.Surge.Messages;
using STEM.Sys.Messaging;

namespace STEM.Surge.ControlPanel
{
    public partial class ControlPanel : Form
    {
        delegate void ControlAccess();

        internal SwitchboardEditor _SwitchboardEditor = null;

        internal TableDataSources _TableDataSources = new TableDataSources();

        System.Timers.Timer _ClockTimer = new System.Timers.Timer(1000);

        internal string _ManagerIP = null;

        System.Timers.Timer _CollectorRefreshTimer = new System.Timers.Timer(2000);
        System.Timers.Timer _ServiceButtonRefresh = null;

        UIActor _UIActor = new UIActor(STEM.Surge.Control.CommunicationPort);
        
        internal SwitchboardDataCountsView SwitchboardDataCountsView
        {
            get
            {
                if (_ActivityControl != null)
                    return _ActivityControl._SwitchboardDataCountsView;

                return null;
            }
        }
        
        public ControlPanel()
        {
            InitializeComponent();
                        
            panelActive.Visible = false;
            controlPanelMainTools.Visible = false;

            _UIActor.onPrimaryConnectionOpened += _UIActor_onPrimaryConnectionOpened;
            _UIActor.onPrimaryConnectionClosed += _UIActor_onPrimaryConnectionClosed;
            _UIActor.onUpdateStatusMessage += _UIActor_onUpdateStatusMessage;
            _UIActor.onSwitchboardConfigUpdated += _UIActor_onSwitchboardConfigUpdated;

            STEM.Sys.Global.ThreadPool.BeginAsync(new System.Threading.ThreadStart(ShowStatusMessages), TimeSpan.FromSeconds(1));

            _SwitchboardEditor = new SwitchboardEditor(_UIActor);
            _ActivityControl = new ActivityControl(_UIActor);
                                                
            branchMenu.ItemClicked += new ToolStripItemClickedEventHandler(branchMenu_ItemClicked);
            
            branchDetailsBindingSource.DataSource = _TableDataSources;
            branchDetailsBindingSource.SuspendBinding();
            branchDetailsBindingSource.Sort = "BranchIP";

            branchListGrid.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(branchListGrid_DataBindingComplete);

            branchListGrid.CellDoubleClick += new DataGridViewCellEventHandler(branchListGrid_CellDoubleClick);

            branchListGrid.RowHeaderMouseDoubleClick += new DataGridViewCellMouseEventHandler(branchListGrid_RowHeaderMouseDoubleClick);

            Type dgt = branchListGrid.GetType();
            PropertyInfo dgtPi = dgt.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            dgtPi.SetValue(branchListGrid, true);

            connectedDM_Click(this, null);

            _CollectorRefreshTimer.Elapsed += new System.Timers.ElapsedEventHandler(_CollectorRefreshTimer_Elapsed);
            _CollectorRefreshTimer.AutoReset = false;
            _CollectorRefreshTimer.Enabled = true;
            
            _ClockTimer.AutoReset = false;
            _ClockTimer.Elapsed += new System.Timers.ElapsedEventHandler(_ClockTimer_Elapsed);
            _ClockTimer.Enabled = true;

            this.Shown += ControlPanel_Shown;
        }

        void branchListGrid_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            lock (branchListGrid)
            {
                string branchIP = branchListGrid[2, e.RowIndex].Value.ToString();

                string d = "\\\\" + branchIP + @"\C$\Program Files\STEM Management\STEM.Surge\InstructionCache";

                System.Diagnostics.Process.Start(d);
            }
        }
        
        private void ControlPanel_Shown(object sender, EventArgs e)
        {
            if (connectedDM.Text == "No Connection")
                Close();
        }

        void _UIActor_onSwitchboardConfigUpdated(object sender, EventArgs e)
        {
            //if (panelActive.Controls[0] == _SwitchboardEditor)
                _SwitchboardEditor.Reload();
        }

        Queue<string> _StatusMessages = new Queue<string>();
        void _UIActor_onUpdateStatusMessage(UIActor connection, string message)
        {
            lock (_StatusMessages)
                _StatusMessages.Enqueue(message + " " + DateTime.UtcNow.ToString("G").PadLeft(25, ' '));
        }

        void ShowStatusMessages()
        {
            if (InvokeRequired)
            {
                string connectionsText = "Connected to: " + String.Join(", ", _UIActor.Connections()).PadRight(40, ' ');
                lock (_StatusMessages)
                    if (_StatusMessages.Count == 0 && connectionsText == toolStripStatusLabel2.Text)
                        return;

                BeginInvoke(new System.Threading.ThreadStart(ShowStatusMessages));
            }
            else
            {
                lock (_StatusMessages)
                    while (_StatusMessages.Count > 0)                        
                        toolStripStatusLabel1.Text = _StatusMessages.Dequeue();

                toolStripStatusLabel2.Text = "Connected to: " + String.Join(", ", _UIActor.Connections()).PadRight(40, ' ');
            }
        }

        void _UIActor_onPrimaryConnectionClosed(UIActor connection)
        {
            HideRight();
        }

        void _UIActor_onPrimaryConnectionOpened(UIActor connection)
        {
            ShowRight();
        }

        ConnectionNotification _ConnectionNotification = null;
        void ShowRight()
        {
            if (panelActive.InvokeRequired)
            {
                panelActive.BeginInvoke(new System.Threading.ThreadStart(ShowRight));
            }
            else
            {
                panelActive.Visible = true;
                controlPanelMainTools.Visible = true;

                if (_ConnectionNotification != null)
                {
                    _ConnectionNotification.Close();
                    _ConnectionNotification = null;
                }

                _SwitchboardEditor.Reload();

                if (panelActive.Controls.Count == 0)
                    SetDetailPanel(_SwitchboardEditor, panelActive);
            }
        }

        void HideRight()
        {
            if (panelActive.InvokeRequired)
            {
                panelActive.BeginInvoke(new System.Threading.ThreadStart(HideRight));
            }
            else
            {
                panelActive.Visible = false;
                controlPanelMainTools.Visible = false;

                if (_ManagerIP == _UIActor.PrimaryDeploymentManagerIP)
                {
                    _ConnectionNotification = new ConnectionNotification(_UIActor.PrimaryDeploymentManagerIP);
                    _ConnectionNotification.ShowDialog(this);
                }
            }
        }

        void _ClockTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                RefreshTime();
            }
            catch { }
            finally
            {
                _ClockTimer.Enabled = true;
            }
        }

        private void RefreshTime()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ThreadStart(RefreshTime));
            }
            else
            {
                curTime.Text = DateTime.UtcNow.ToString("G") + " UTC";
            }
        }
        
        void branchMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (branchListGrid.SelectedRows.Count < 1)
            {
                MessageBox.Show(this, "There are no Branch rows selected.", "Select Rows", MessageBoxButtons.OK);
                return;
            }

            STEM.Surge.Messages.SetServiceConfiguration setConfiguration = new SetServiceConfiguration();

            if (e.ClickedItem.Name == "updateSurge")
            {
                if (MessageBox.Show(this, "Update all selected Branch Servers?", "Are you sure?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    return;
            }
            else if (e.ClickedItem.Name == "updateConfiguration")
            {
                GetServiceConfiguration m = new GetServiceConfiguration();
                m.MachineIP = branchListGrid.SelectedRows[0].Cells[2].Value.ToString();

                STEM.Sys.Messaging.Message r = _UIActor.Send(m, TimeSpan.FromSeconds(5));

                if (r is GetServiceConfiguration)
                {
                    m = r as GetServiceConfiguration;
                    ServiceConfiguration bc = new ServiceConfiguration(m);

                    if (bc.ShowDialog(this) == DialogResult.Cancel)
                        return;

                    if (MessageBox.Show(this, "Update all selected Branch Servers?", "Are you sure?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                        return;

                    setConfiguration.SurgeCommunicationPort = bc.SurgeCommunicationPort;
                    setConfiguration.SurgeDeploymentManagerAddress = bc.SurgeDeploymentManagerAddress;
                    setConfiguration.ProcessorOverload = bc.ProcessorOverload;
                    setConfiguration.PostMortemDirectory = bc.PostMortemDirectory;
                    setConfiguration.RemoteConfigurationDirectory = bc.RemoteConfigurationDirectory;
                    setConfiguration.UseSSL = bc.UseSSL;
                }
                else
                {
                    MessageBox.Show(this, "There was a problem getting the existing configuration from the Branch.", "Exception", MessageBoxButtons.OK);
                    return;
                }
            }

            List<string> ips = new List<string>();
            foreach (DataGridViewRow r in branchListGrid.SelectedRows)
            {
                if (e.ClickedItem.Name == "takeOffline")
                {
                    _UIActor.Send(new TakeOffline(r.Cells[2].Value.ToString()));
                }
                else if (e.ClickedItem.Name == "bringOnline")
                {
                    _UIActor.Send(new BringOnline(r.Cells[2].Value.ToString()));
                }
                else if (e.ClickedItem.Name == "clearErrors")
                {
                    _UIActor.Send(new ClearErrors(r.Cells[2].Value.ToString()));
                }
                else if (e.ClickedItem.Name == "updateSurge")
                {
                    _UIActor.Send(new UpdateSurge(r.Cells[2].Value.ToString()));
                }
                else if (e.ClickedItem.Name == "updateConfiguration")
                {
                    setConfiguration.MachineIP = r.Cells[2].Value.ToString();
                    _UIActor.Send(setConfiguration);
                }
                else
                {
                    ips.Add(r.Cells[2].Value.ToString());
                }
            }

            if (e.ClickedItem.Name == "viewErrors")
            {
                ViewErrors(ips);
            }
            else
            {
                MessageBox.Show(this, branchListGrid.SelectedRows.Count + " operation(s) started.", "Branch commands issued.");
            }
        }

        void ViewErrors(object param)
        {
            List<string> ips = param as List<string>;

            ErrorViewer ev = new ErrorViewer(_UIActor, ips);
            ev.Show(this);
        }
                        
        void _CollectorRefreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (_UIActor != null)
                    _UIActor.IsConnected();

                RefreshTables();
            }
            catch
            {
                _CollectorRefreshTimer.Enabled = true;
            }
        }

        TableDataSources.BranchDetailsDataTable _BranchDetailsDataTable = new TableDataSources.BranchDetailsDataTable();
        TableDataSources.AssignmentDetailsDataTable _AssignmentDetailsDataTable = new Surge.ControlPanel.TableDataSources.AssignmentDetailsDataTable();
        TableDataSources.SwitchboardDataCountsDataTable _SwitchboardDataCountsDataTable = new Surge.ControlPanel.TableDataSources.SwitchboardDataCountsDataTable();
        List<Backlogs.Entry> _Backlogs = new List<Backlogs.Entry>();

        private void RefreshTables()
        {
            if (InvokeRequired)
            {
                if (_UIActor != null)
                {
                    List<ActiveDeployments.Entry> deployments = _UIActor.Deployments;

                    Dictionary<string, string> branchMap = new Dictionary<string, string>();

                    try
                    {
                        _BranchDetailsDataTable = new TableDataSources.BranchDetailsDataTable();

                        foreach (Branches.Entry b in _UIActor.BranchEntries)
                        {
                            try
                            {
                                IEnumerable<ActiveDeployments.Entry> ad = deployments.Where(i => i.BranchIP == b.BranchIP && i.Completed == DateTime.MinValue);
                                _BranchDetailsDataTable.AddBranchDetailsRow(
                                    b.BranchState.ToString(),
                                    b.BranchIP,
                                    b.OSDescription,
                                    b.BranchName,
                                    ad.Count(),
                                    ad.Count(i => i.Received > DateTime.MinValue),
                                    b.ErrorIDs.Count,
                                    b.ThreadCount,
                                    b.MBRam);

                                branchMap[b.BranchIP] = b.BranchName;
                            }
                            catch (Exception ex)
                            {
                                STEM.Sys.EventLog.WriteEntry("ControlPanel.RefreshBranchDetailsDataTable", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                            }
                        }
                    }
                    catch { }

                    if (_ActivityControl != null)
                    {
                        _AssignmentDetailsDataTable = new TableDataSources.AssignmentDetailsDataTable();

                        foreach (STEM.Surge.Messages.ActiveDeployments.Entry x in deployments)
                        {
                            try
                            {
                                double exeTime = x.ExecutionTime;
                                if (x.Completed == DateTime.MinValue)
                                {
                                    if (x.Received > DateTime.MinValue)
                                        exeTime = (DateTime.UtcNow - x.Received).TotalSeconds;
                                    else
                                        exeTime = (DateTime.UtcNow - x.Issued).TotalSeconds;
                                }

                                _AssignmentDetailsDataTable.AddAssignmentDetailsRow(
                                       x.DeploymentManagerIP,
                                       x.BranchIP,
                                       (branchMap.ContainsKey(x.BranchIP) ? branchMap[x.BranchIP] : ""),
                                       x.DeploymentController,
                                       x.InitiationSource,
                                       x.Issued,
                                       x.Received,
                                       x.Completed,
                                       x.LastModified,
                                       exeTime,
                                       x.Exceptions,
                                       x.InstructionSetID,
                                       x.DeploymentControllerID.ToString(),
                                       x.SwitchboardRowID.ToString());
                            }
                            catch (Exception ex)
                            {
                                STEM.Sys.EventLog.WriteEntry("ControlPanel.RefreshAssignmentDetailsDataTable", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                            }
                        }

                        _Backlogs = _UIActor.Backlogs;

                        _SwitchboardDataCountsDataTable = new TableDataSources.SwitchboardDataCountsDataTable();

                        Dictionary<string, bool> enableds = new Dictionary<string, bool>();

                        foreach (SwitchboardConfig.FileSourcesRow fsr in _UIActor.DeploymentManagerConfiguration.SwitchboardConfiguration.FileSources)
                            enableds[STEM.Surge.GenerateSwitchboardRowIDs.GenerateDeploymentControllerID(fsr, fsr.SourceDirectory)] = fsr.Enable;

                        foreach (string sw in enableds.Keys)
                        {
                            try
                            {
                                List<Backlogs.Entry> tmp = _Backlogs.Where(i => i.SwitchboardRowID == sw).ToList();

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

                                double avgExecutionTime = 0;

                                List<ActiveDeployments.Entry> tmpDeployments = deployments.Where(i => i.SwitchboardRowID == sw).ToList();
                                IEnumerable <TimeSpan> deltas = tmpDeployments.Where(i => i.Received > DateTime.MinValue && i.Completed > DateTime.MinValue).Select(i => i.Completed - i.Received);
                                if (deltas.Count() > 0)
                                {
                                    TimeSpan a = new TimeSpan((long)deltas.Select(i => i.Ticks).Average());
                                    avgExecutionTime = a.TotalSeconds;
                                }

                                if ((_ActivityControl._SwitchboardDataCountsView.EnabledRowsOnly && enableds[sw]) || !_ActivityControl._SwitchboardDataCountsView.EnabledRowsOnly)
                                    _SwitchboardDataCountsDataTable.AddSwitchboardDataCountsRow(
                                                enableds[sw],
                                                tmp.First().SwitchboardRowDescription,
                                                tmp.First().DeploymentController,
                                                tmp.Sum(i => i.BacklogCount),
                                                tmp.Sum(i => i.PerceivedBacklogCount),
                                                tmpDeployments.Count(i => i.Completed == DateTime.MinValue),
                                                tmpDeployments.Count(i => i.Completed == DateTime.MinValue && i.Received > DateTime.MinValue),
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
                                                deltas.Count(),
                                                tmp.Any(i => i.Assigning),
                                                sw);
                            }
                            catch (Exception ex)
                            {
                                STEM.Sys.EventLog.WriteEntry("ControlPanel.RefreshSwitchboardDataCountsDataTable", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                            }
                        }
                    }
                }

                BeginInvoke(new ThreadStart(RefreshTables));
            }
            else
            {
                if (_ActivityControl != null)
                {
                    _ActivityControl._SwitchboardDataCountsView.SetBindingSource(_SwitchboardDataCountsDataTable, _Backlogs);
                    _ActivityControl._AssignmentDetailsView.SetBindingSource(_AssignmentDetailsDataTable);
                }

                branchListGrid.SuspendLayout();
                try
                {
                    List<string> selectedIps = new List<string>();
                    foreach (DataGridViewRow r in branchListGrid.SelectedRows)
                        selectedIps.Add(r.Cells[2].Value.ToString());

                    int vs = branchListGrid.FirstDisplayedScrollingRowIndex;
                    int hs = branchListGrid.FirstDisplayedScrollingColumnIndex;

                    _TableDataSources.BranchDetails.Clear();
                    _TableDataSources.BranchDetails.Merge(_BranchDetailsDataTable);

                    if (vs > -1 && vs < branchListGrid.RowCount)
                        branchListGrid.FirstDisplayedScrollingRowIndex = vs;
                    if (hs > -1 && hs < branchListGrid.ColumnCount)
                        branchListGrid.FirstDisplayedScrollingColumnIndex = hs;

                    List<UIActor.ManagerReportTimes> allTimes = _UIActor.LatestManagerReportTimes;

                    if (branchListGrid.Rows.Count > 0)
                    {
                        branchListGrid.ClearSelection();

                        foreach (DataGridViewRow r in branchListGrid.Rows)
                        {
                            try
                            {
                                if (selectedIps.Contains(r.Cells[2].Value.ToString()))
                                    r.Selected = true;

                                string tip = "Connection Summary: " + _TableDataSources.BranchDetails.FindByBranchIP(r.Cells[2].Value.ToString()).OSDescription + "\r\n";
                                foreach (UIActor.ManagerReportTimes mrt in allTimes)
                                {
                                    if (mrt.LastBranchReport.ContainsKey(r.Cells[2].Value.ToString()))
                                    {
                                        tip += r.Cells[2].Value.ToString().PadRight(16, ' ') + " Last seen by " + mrt.ManagerIP.PadRight(16, ' ') + " at " + mrt.LastBranchReport[r.Cells[2].Value.ToString()].ToString("G").PadLeft(26, ' ') + "\r\n";
                                    }
                                }

                                r.Cells[2].ToolTipText = tip;
                            }
                            catch { }
                        }
                    }
                }
                catch (Exception ex)
                {
                    STEM.Sys.EventLog.WriteEntry("ControlPanel.RefreshTables", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                }
                finally
                {
                    branchListGrid.ResumeLayout();
                    _CollectorRefreshTimer.Enabled = true;
                }
            }
        }

        void branchListGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                branchListGrid.Rows[e.RowIndex].Selected = true;
            }
            catch { }
        }

        void _ServiceButtonRefresh_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                SetServiceButton();
            }
            catch { }
        }

        System.ServiceProcess.ServiceControllerStatus _LastServiceState = System.ServiceProcess.ServiceControllerStatus.Paused;
        void SetServiceButton()
        {
            if (InvokeRequired)
            {
                System.ServiceProcess.ServiceController serviceController = null;
                try
                {
                    serviceController = new System.ServiceProcess.ServiceController("STEM.Surge", connectedDM.Text);

                    bool invoked = false;
                    if (serviceController != null)
                        if (serviceController.Status != _LastServiceState)
                        {
                            BeginInvoke(new ThreadStart(SetServiceButton));
                            invoked = true;
                        }

                    if (!invoked)
                        _ServiceButtonRefresh.Enabled = true;
                }
                catch { }
                finally
                {
                    if (serviceController != null)
                        try
                        {
                            serviceController.Close();
                        }
                        catch { }
                }
            }
            else
            {
                System.ServiceProcess.ServiceController serviceController = null;
                try
                {
                    serviceController = new System.ServiceProcess.ServiceController("STEM.Surge", connectedDM.Text);

                    if (serviceController != null)
                    {
                        _LastServiceState = serviceController.Status;
                        switch (serviceController.Status)
                        {
                            case System.ServiceProcess.ServiceControllerStatus.Running:
                                if (startStopService.Image != global::STEM.Surge.ControlPanel.Properties.Resources.stop)
                                {
                                    startStopService.Image = global::STEM.Surge.ControlPanel.Properties.Resources.stop;
                                    startStopService.ToolTipText = "Running... Stop Service?";
                                    startStopService.Enabled = true;
                                }
                                break;

                            case System.ServiceProcess.ServiceControllerStatus.Stopped:
                                if (startStopService.Image != global::STEM.Surge.ControlPanel.Properties.Resources.play)
                                {
                                    startStopService.Image = global::STEM.Surge.ControlPanel.Properties.Resources.play;
                                    startStopService.ToolTipText = "Stopped... Start Service?";
                                    startStopService.Enabled = true;
                                }
                                break;

                            case System.ServiceProcess.ServiceControllerStatus.StartPending:
                                if (startStopService.ToolTipText != "Starting...")
                                {
                                    startStopService.ToolTipText = "Starting...";
                                    startStopService.Enabled = false;
                                }
                                break;

                            case System.ServiceProcess.ServiceControllerStatus.StopPending:
                                if (startStopService.ToolTipText != "Stopping...")
                                {
                                    startStopService.ToolTipText = "Stopping...";
                                    startStopService.Enabled = false;
                                }
                                break;
                        }
                    }
                }
                catch { }
                finally
                {
                    if (serviceController != null)
                        try
                        {
                            serviceController.Close();
                        }
                        catch { }

                    _ServiceButtonRefresh.Enabled = true;
                }
            }
        }

        public void SetDetailPanel(System.Windows.Forms.Control active, Panel p)
        {
            if (p.Controls.Count > 0 && p.Controls[0] is SwitchboardEditor)
                if (((SwitchboardEditor)p.Controls[0]).IsDirty)
                {
                    if (MessageBox.Show(this, "Cancel Changes?", "Unsaved", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                        return;

                    ((SwitchboardEditor)p.Controls[0]).Bind();
                }

            if (p.Controls.Count > 0 && p.Controls[0] is SourceRowPollerStats)
                ((SourceRowPollerStats)p.Controls[0]).Stop();

            if (p.Controls.Count > 0 && p.Controls[0] is ControllerEditor)
                if (((ControllerEditor)p.Controls[0]).IsDirty)
                    if (MessageBox.Show(this, "Cancel Changes?", "Unsaved", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                        return;

            if (p.Controls.Count > 0 && p.Controls[0] is InstructionSetListEditor)
                if (((InstructionSetListEditor)p.Controls[0]).IsDirty)
                    if (MessageBox.Show(this, "Cancel Changes?", "Unsaved", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                        return;

            if (p.Controls.Count > 0 && p.Controls[0] is StaticsListEditor)
                if (((StaticsListEditor)p.Controls[0]).IsDirty)
                    if (MessageBox.Show(this, "Cancel Changes?", "Unsaved", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                        return;

            if (p.Controls.Count > 0 && p.Controls[0] is VersionUpdates)
                if (((VersionUpdates)p.Controls[0]).IsDirty)
                    if (MessageBox.Show(this, "Cancel Changes?", "Unsaved", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                        return;            
            
            p.Controls.Clear();
            if (active != null)
            {
                active.Dock = DockStyle.Fill;
                p.Controls.Add(active);
            }
        }

        private void connectedDM_Click(object sender, EventArgs e)
        {
            DMConnect c = new DMConnect();
            if (c.ShowDialog(this) == System.Windows.Forms.DialogResult.Cancel)
                return;

            connectedDM.Text = STEM.Sys.IO.Net.MachineAddress(c.IP.Trim());
            _ManagerIP = connectedDM.Text.Trim();
            panelActive.Controls.Clear();

            if (_ConnectionNotification != null)
            {
                _ConnectionNotification.Close();
                _ConnectionNotification = null;
            }

            this.Visible = false;
            //Splash splash = new Splash();
            //splash.Start(this);

            if (!_UIActor.InitializeConnection(_ManagerIP, Control.CommunicationPort, c.UseSSL))
            {
                MessageBox.Show(this, "The server did not accept the connection request, it might not be running. I will continue to try.", "Trying", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
            //splash.Stop();
            this.Visible = true;

            try
            {
                if (_ServiceButtonRefresh == null)
                {
                    _ServiceButtonRefresh = new System.Timers.Timer(3000);
                    _ServiceButtonRefresh.AutoReset = false;
                    _ServiceButtonRefresh.Elapsed += new System.Timers.ElapsedEventHandler(_ServiceButtonRefresh_Elapsed);
                    _ServiceButtonRefresh.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "Service request failed!");
            }

            switchboardConfig_Click(this, EventArgs.Empty);

            if (_UIActor.DeploymentManagerConfiguration.MessageConnection != null)
                about_Click(this, EventArgs.Empty);
        }

        void branchListGrid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                for (int row = 0; row < branchListGrid.Rows.Count; row++)
                {
                    try
                    {
                        DataGridViewImageCell displayCell = (DataGridViewImageCell)branchListGrid[1, row];

                        STEM.Surge.BranchState state = (STEM.Surge.BranchState)Enum.Parse(typeof(STEM.Surge.BranchState), branchListGrid[0, row].Value.ToString());
                        string branchIP = branchListGrid[2, row].Value.ToString();

                        switch (state)
                        {
                            case STEM.Surge.BranchState.Online:
                                displayCell.Value = Surge.ControlPanel.Properties.Resources.ok;
                                displayCell.ToolTipText = "Online";
                                break;

                            case STEM.Surge.BranchState.Offline:
                                displayCell.Value = Surge.ControlPanel.Properties.Resources.offline;
                                displayCell.ToolTipText = "Offline";
                                break;

                            case STEM.Surge.BranchState.Silent:
                                displayCell.Value = Surge.ControlPanel.Properties.Resources.silent;
                                displayCell.ToolTipText = "Silent (may be down)";
                                break;

                            case STEM.Surge.BranchState.RegisteredSpare:
                                displayCell.Value = Surge.ControlPanel.Properties.Resources.spare;
                                displayCell.ToolTipText = "Registered Spare";
                                break;
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }
        
        ActivityControl _ActivityControl = null;

        public static bool IsSES = false;
        
        private void startStopService_Click(object sender, EventArgs e)
        {
            System.ServiceProcess.ServiceController serviceController = null;
            try
            {
                serviceController = new System.ServiceProcess.ServiceController("STEM.Surge", connectedDM.Text);

                if (serviceController.Status != System.ServiceProcess.ServiceControllerStatus.Running)
                {            
                    serviceController.Start();
                }
                else
                    serviceController.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "Service request failed!");
            }
            finally
            {
                if (serviceController != null)
                    try
                    {
                        serviceController.Close();
                    }
                    catch { }
            }
        }

        private void licenseAccess_Click(object sender, EventArgs e)
        {
            LicenseTextarea l = new LicenseTextarea(_UIActor);
            l.ShowDialog(this);

            About a = new About(_UIActor);
            a.ShowDialog(this);
        }
        
        private void about_Click(object sender, EventArgs e)
        {
            About a = new About(_UIActor);
            a.ShowDialog(this);
        }

        private void switchboardConfig_Click(object sender, EventArgs e)
        {
            SetDetailPanel(_SwitchboardEditor, panelActive);
        }

        private void activity_Click(object sender, EventArgs e)
        {
            _ActivityControl.Activate();
            SetDetailPanel(_ActivityControl, panelActive);
        }

        private void deploymentControllerEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ControllerListEditor editor = new ControllerListEditor(_UIActor);
            SetDetailPanel(editor, panelActive);
        }

        private void instructionSetEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InstructionSetListEditor editor = new InstructionSetListEditor(false);
            editor.Initialize(_UIActor.DeploymentManagerConfiguration.InstructionSetTemplates, _UIActor, "Templates");
            SetDetailPanel(editor, panelActive);
        }

        private void editManagerStaticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StaticsListEditor editor = new StaticsListEditor(_UIActor);
            SetDetailPanel(editor, panelActive);
        }
        
        private void attach_Click(object sender, EventArgs e)
        {
            connectedDM_Click(sender, e);
        }
        
        private void changeVersionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VersionUpdates v = new VersionUpdates(_UIActor);
            SetDetailPanel(v, panelActive);
        }
        
        private void contextHelp_Click(object sender, EventArgs e)
        {
            ContextHelpForm chf = new ContextHelpForm(Surge.ControlPanel.Properties.Resources.SwitchboardEditor);

            if (chf != null)
                chf.Show(this);
        }

        private void uploadExtensions_Click(object sender, EventArgs e)
        {
            STEM.Sys.Messaging.Message response = new Undeliverable();

            if (openExtensions.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                if (openExtensions.FileNames != null)
                {
                    TestExtensionUpload m = new TestExtensionUpload(openExtensions.FileNames.ToList());

                    response = _UIActor.Send(m, TimeSpan.FromSeconds(30));

                    while (!(response is TestExtensionUpload))
                    {
                        if (MessageBox.Show(this, "The server didn't respond in a reasonable period of time.\r\nRetry?", "Retry?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                            return;

                        response = m.GetNextResponse(TimeSpan.FromSeconds(30));
                    }
                }
            }

            if (response is TestExtensionUpload)
            {
                TestExtensionUpload r = response as TestExtensionUpload;

                string conflicts = "";
                foreach (TestExtensionUpload.Entry existing in r.ExistingEntries)
                {
                    TestExtensionUpload.Entry candidate = r.CandidateEntries.FirstOrDefault(i => i.TransformedFilename.Equals(existing.TransformedFilename, StringComparison.InvariantCultureIgnoreCase));
                    if (candidate != null)
                    {
                        if (candidate.LastModified != existing.LastModified)
                        {
                            conflicts += candidate.TransformedFilename + ": " + candidate.LastModified.ToString("yyyy-MM-dd HH.mm.ss") + ", " + existing.LastModified.ToString("yyyy-MM-dd HH.mm.ss") + "\r\n";;
                        }
                    }
                }

                if (conflicts != "")
                {
                    MessageBox.Show(this, "One or more files confilcted at the server. Please reversion and try again.\r\n" + conflicts, "Version Conflicts.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                foreach (string s in openExtensions.FileNames)
                {
                    FileTransfer m = new FileTransfer(s, "Extensions", STEM.Sys.Serialization.VersionManager.TransformFilename(s), STEM.Sys.IO.FileExistsAction.Overwrite, true, true);
                    _UIActor.Send(m);
                }

                MessageBox.Show(this, "Upload Complete.", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void viewLocks_Click(object sender, EventArgs e)
        {
            LockViewer ev = new LockViewer(_UIActor);
            ev.Show(this);
        }

        private void manageExtensionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VersionsManagement v = new VersionsManagement(_UIActor);
            SetDetailPanel(v, panelActive);
        }

        private void exploreConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExploreConfigs explore = new ExploreConfigs(_UIActor);
            explore.Show(this);
        }

        private void Grafana_Click(object sender, EventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(_UIActor.DeploymentManagerConfiguration.SwitchboardConfiguration.Settings[0].GrafanaURL);
            Process.Start(sInfo);
        }

        private void pollerStats_Click(object sender, EventArgs e)
        {
            SourceRowPollerStats stats = new SourceRowPollerStats(_UIActor);
            stats.Start();
            SetDetailPanel(stats, panelActive);
        }

        private void adHocInstructionSet_Click(object sender, EventArgs e)
        {
            AdHoc adHoc = new AdHoc(_UIActor);
            adHoc.Show(this);
        }

        private void sandboxConfigurationUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SandboxConfigUpdate cfg = new SandboxConfigUpdate(_UIActor);
            cfg.ShowDialog(this);
        }
    }
}
