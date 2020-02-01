using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using STEM.Surge;
using System.Xml;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace STEM.Surge.ControlPanel
{
    public partial class ActivityControl : UserControl
    {
        UIActor _UIActor;

        internal AssignmentDetailsView _AssignmentDetailsView;
        internal SwitchboardDataCountsView _SwitchboardDataCountsView;

        public ActivityControl(UIActor messageClient)
        {
            InitializeComponent();

            _UIActor = messageClient;

            _AssignmentDetailsView = new AssignmentDetailsView(_UIActor);
            _SwitchboardDataCountsView = new SwitchboardDataCountsView(_UIActor);

            _SwitchboardDataCountsView.Dock = DockStyle.Fill;

            _AssignmentDetailsView.Dock = DockStyle.Fill;
        }

        public void Activate()
        {
            switchboardCountsPanel.Controls.Clear();
            switchboardCountsPanel.Controls.Add(_SwitchboardDataCountsView);
            assignmentsDetailsPanel.Controls.Clear();
            assignmentsDetailsPanel.Controls.Add(_AssignmentDetailsView);
        }

        public void RefreshTables()
        {
            try
            {
                if (_UIActor.DeploymentManagerConfiguration.MessageConnection != null)
                {
                    _AssignmentDetailsView.RefreshTables();
                    _SwitchboardDataCountsView.RefreshTables();
                }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("Surge.ControlPanel.ActivityControl.RefreshTables", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }
        }
    }
}
