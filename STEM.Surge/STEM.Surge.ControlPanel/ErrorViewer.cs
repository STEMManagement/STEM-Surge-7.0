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
    public partial class ErrorViewer : Form
    {
        List<string> _IPs = new List<string>();
        System.Threading.Thread _UtilThread;

        UIActor _UIActor;

        public ErrorViewer(UIActor messageClient, List<string> ips)
        {
            InitializeComponent();

            _UIActor = messageClient;

            FormClosed += new FormClosedEventHandler(ErrorViewer_FormClosed);

            if (!_UIActor.AssemblyInitializationComplete)
            {
                MessageBox.Show(this, "Still downloading data from the server. Try again later.", "Initializing", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _IPs = ips;

            contextMenuStrip1.ItemClicked += new ToolStripItemClickedEventHandler(contextMenuStrip1_ItemClicked);

            errorGridView1.RowHeaderMouseDoubleClick += new DataGridViewCellMouseEventHandler(errorGridView1_RowHeaderMouseDoubleClick);

            errorsBindingSource.Sort = "Complete ASC";

            tableDataSources.Errors.RowDeleting += new DataRowChangeEventHandler(_RowDeleting);
            tableDataSources.Errors.RowDeleted += new DataRowChangeEventHandler(_RowDeleted);

            Type dgt = errorGridView1.GetType();
            PropertyInfo dgtPi = dgt.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            dgtPi.SetValue(errorGridView1, true);

            foreach (string ip in _IPs)
                try
                {  
                    List<string> errorIDs = new List<string>();
                    foreach (Branches.Entry b in _UIActor.BranchEntries.Where(i => i.BranchIP == ip))
                        errorIDs = b.ErrorIDs.ToList();

                    foreach (string g in errorIDs)
                    {
                        InstructionSetRequested m = new InstructionSetRequested(ip, Guid.Parse(g), "");
                        m.onResponse += ErrorsRequested_onResponse;
                        _UIActor.Send(m);
                    }
                }
                catch { }

            rowCount.Text = "Count (" + errorsBindingSource.Count + ")";
                        
            _UtilThread = new System.Threading.Thread(new ThreadStart(UtilThread));
            _UtilThread.IsBackground = true;
            _UtilThread.Start();
        }
        
        List<InstructionSetRequested> _QueuedInstructionSets = new List<InstructionSetRequested>();
        List<Surge._InstructionSet> _InstructionSets = new List<Surge._InstructionSet>();
        void ErrorsRequested_onResponse(STEM.Sys.Messaging.Message delivered, STEM.Sys.Messaging.Message response)
        {
            if (response is InstructionSetRequested)
                lock (_QueuedInstructionSets)
                {
                    InstructionSetRequested m = response as InstructionSetRequested;

                    if (m != null)
                        _QueuedInstructionSets.Add(m);

                    PopulateErrors();
                }
        }

        void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            errorGridView1_RowHeaderMouseDoubleClick(sender, null);
        }

        void ErrorViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                try
                {
                    _UtilThread.Interrupt();
                }
                catch { }

                try
                {
                    _UtilThread.Abort();
                }
                catch { }
            }
            catch { }
        }

        List<string> _Delete = new List<string>();
        void UtilThread()
        {
            while (true)
            {
                try
                {
                    lock (_QueuedInstructionSets)
                        lock (_Delete)
                            if (_Delete.Count > 0)
                            {
                                ClearErrors m = null;
                                foreach (Surge.InstructionSet i in _InstructionSets.Where(i => _Delete.Contains(i.ID.ToString())).OrderBy(i => i.BranchIP).ToList())
                                {
                                    if (m == null)
                                    {
                                        m = new ClearErrors(i.BranchIP);
                                    }
                                    else if (m.BranchIP != i.BranchIP)
                                    {
                                        _UIActor.Send(m);
                                        m = new ClearErrors(i.BranchIP);
                                    }

                                    m.SpecificErrors.Add(i.ID.ToString());

                                    _InstructionSets.Remove(i);

                                    _Delete.Remove(i.ID.ToString());
                                }

                                if (m != null)
                                    _UIActor.Send(m);
                            }
                }
                catch { }

                System.Threading.Thread.Sleep(100);
            }
        }

        bool _Processing = false;
        void PopulateErrors()
        {
            if (InvokeRequired)
            {
                if (!_Processing)
                {
                    _Processing = true;
                    BeginInvoke(new ThreadStart(PopulateErrors));
                }
            }
            else
            {
                lock (_QueuedInstructionSets)
                {
                    try
                    {
                        foreach (InstructionSetRequested i in _QueuedInstructionSets.ToList())
                        {
                            if (i.InstructionSet != null)
                            {
                                string exSummary = "";

                                foreach (STEM.Surge.Instruction ii in i.InstructionSet.Instructions)
                                    foreach (Exception ex in ii.Exceptions)
                                        exSummary += ex.Message + "\r\n";

                                try
                                {
                                    tableDataSources.Errors.AddErrorsRow(i.BranchIP, i.InstructionSet.ID.ToString(), exSummary, i.InstructionSet.Completed, i.InstructionSet.ProcessName);
                                }
                                catch { }

                                _QueuedInstructionSets.Remove(i);
                                _InstructionSets.Add(i.InstructionSet);
                            }
                        }
                    }
                    finally
                    {
                        _Processing = false;
                    }
                }

                filterMask_TextChanged(this, EventArgs.Empty);
            }
        }

        void errorGridView1_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                errorGridView1.Rows[errorGridView1.SelectedCells[0].RowIndex].Selected = true;

                try
                {
                    Surge._InstructionSet iSet = _InstructionSets.FirstOrDefault(i => i.ID.ToString() == errorGridView1.SelectedRows[0].Cells[4].Value.ToString());

                    if (iSet != null)
                    {
                        InstructionSetDetails dets = new InstructionSetDetails(iSet, _UIActor);
                        dets.ShowDialog(this);
                    }
                }
                catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void _RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            lock (tableDataSources.Errors)
                rowCount.Text = "Count (" + errorsBindingSource.Count + ")";
        }

        void _RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            try
            {
                lock (tableDataSources.Errors)
                {
                    TableDataSources.ErrorsRow sr = e.Row as TableDataSources.ErrorsRow;

                    lock (_Delete)
                        _Delete.Add(sr.InstructionSetID);
                }
            }
            catch { }
        }

        private void filterMask_TextChanged(object sender, EventArgs e)
        {
            string f = filterMask.Text.Trim();
            
            if (f.Length == 0)
            {
                errorsBindingSource.Filter = "";
            }
            else
            {
                errorsBindingSource.Filter =
                    "Branch LIKE '%" + f + "%' OR " +
                    "InstructionSetID LIKE '%" + f + "%' OR " +
                    "ProcessName LIKE '%" + f + "%' OR " +
                    "ExceptionSummary LIKE '%" + f + "%'";
            }

            rowCount.Text = "Count (" + errorsBindingSource.Count + ")";
        }

        private void clearFilter_Click(object sender, EventArgs e)
        {
            filterMask.Text = "";
            filterMask_TextChanged(sender, e);
        }
    }
}
