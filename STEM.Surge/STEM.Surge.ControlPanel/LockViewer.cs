using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using STEM.Surge.Messages;

namespace STEM.Surge.ControlPanel
{
    public partial class LockViewer : Form
    {
        List<LockDetail> _LockDetails = new List<LockDetail>();

        STEM.Surge.ControlPanel.TableDataSources _TableDataSources = new TableDataSources();

        UIActor _UIActor;
        
        public LockViewer(UIActor messageClient)
        {
            InitializeComponent();

            _UIActor = messageClient;

            locksDataTableBindingSource.DataSource = _TableDataSources.Locks;
            locksDataTableBindingSource.Sort = "LockTime ASC";
            
            Type dgt = lockGridView1.GetType();
            PropertyInfo dgtPi = dgt.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            dgtPi.SetValue(lockGridView1, true);

            try
            {
                KeysLockedLongerThan m = new KeysLockedLongerThan();
                m.onResponse += Requested_onResponse;
                _UIActor.SendToAll(m);
            }
            catch { }

            rowCount.Text = "Count (" + locksDataTableBindingSource.Count + ")";
        }

        void Requested_onResponse(STEM.Sys.Messaging.Message delivered, STEM.Sys.Messaging.Message response)
        {
            if (response is KeysLockedLongerThan)
                lock (_LockDetails)
                {
                    KeysLockedLongerThan m = response as KeysLockedLongerThan;

                    foreach (KeysLockedLongerThan.LockInfo info in m.LockedKeys)
                        _LockDetails.Add(new LockDetail { Key = info.Key, LockTime = info.LockTime, LastLockAttempt = info.LastLockAttempt, Description = info.Description, DeploymentManagerIP = m.MessageConnection.RemoteAddress });

                    PopulateTable();
                }
        }

        void PopulateTable()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ThreadStart(PopulateTable));
            }
            else
            {
                lock (_LockDetails)
                {
                    _TableDataSources.Locks.Clear();

                    foreach (LockDetail d in _LockDetails)
                        _TableDataSources.Locks.AddLocksRow(d.DeploymentManagerIP, d.Key, d.Description, d.LockTime, d.LastLockAttempt);
                }

                filterMask_TextChanged(this, EventArgs.Empty);
            }
        }

        private void filterMask_TextChanged(object sender, EventArgs e)
        {
            string f = filterMask.Text.Trim();

            if (f.Length == 0)
            {
                locksDataTableBindingSource.Filter = "";
            }
            else
            {
                locksDataTableBindingSource.Filter =
                    "ManagerIP LIKE '%" + f + "%' OR " +
                    "Key LIKE '%" + f + "%'";
            }

            rowCount.Text = "Count (" + locksDataTableBindingSource.Count + ")";
        }

        private void clearFilter_Click(object sender, EventArgs e)
        {
            filterMask.Text = "";
            filterMask_TextChanged(sender, e);
        }
    }

    public class LockDetail
    {
        public string DeploymentManagerIP { get; set; }
        public string Key { get; set; }
        public string Description { get; set; }
        public DateTime LockTime { get; set; }
        public DateTime LastLockAttempt { get; set; }
    }
}
