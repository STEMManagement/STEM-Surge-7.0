using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Threading;
using System.Windows;
using STEM.Surge;
using STEM.Surge.Messages;

namespace STEM.Surge.ControlPanel
{
    public partial class AssignmentDetailsView : UserControl
    {
        internal bool _PauseJobDetailsRefresh = false;

        UIActor _UIActor;

        public AssignmentDetailsView(UIActor messageClient)
        {
            InitializeComponent();

            _UIActor = messageClient;

            assignmentDetailsBindingSource.DataSource = _AssignmentDetailsDataTable;
            assignmentDetailsBindingSource.SuspendBinding();
            assignmentDetailsBindingSource.Sort = "Issued ASC";
                        
            assignmentDetailsGridView.RowHeaderMouseDoubleClick += new DataGridViewCellMouseEventHandler(jobDetailsGridView_RowHeaderMouseDoubleClick);

            _ColumnFilter = new ColumnFilter(assignmentDetailsGridView.Columns);
            
            detailsMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(detailsMenuStrip_ItemClicked);

            Type dgt = assignmentDetailsGridView.GetType();
            PropertyInfo dgtPi = dgt.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            dgtPi.SetValue(assignmentDetailsGridView, true);

            //assignmentsFilterTextbox.TextChanged += assignmentsFilterTextbox_TextChanged;
        }

        void assignmentsFilterTextbox_TextChanged(object sender, EventArgs e)
        {
            RefreshTables();
        }
                        
        void detailsMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int canceled = 0;
            lock (assignmentDetailsGridView)
            {
                foreach (DataGridViewRow r in assignmentDetailsGridView.SelectedRows)
                {
                    if (e.ClickedItem.Name == "viewDetails")
                    {
                        canceled = -1;

                        InstructionSetRequested m = new InstructionSetRequested(r.Cells[1].Value.ToString(), Guid.Parse(r.Cells[10].Value.ToString()), r.Cells[11].Value.ToString());

                        m = _UIActor.Send(m, r.Cells[0].Value.ToString(), TimeSpan.FromSeconds(2)) as InstructionSetRequested;

                        if (m != null && m.InstructionSetXml != null)
                            ShowJobDetails(m.InstructionSet);
                    }
                    else if (e.ClickedItem.Name == "cancelJob")
                    {
                        _UIActor.Send(new CancelExecution(Guid.Parse(r.Cells[10].Value.ToString())), r.Cells[0].Value.ToString());
                        canceled++;
                    }
                }
            }

            if (canceled >= 0)
                MessageBox.Show(canceled + " jobs were found and canceled.");
        }

        public void SetBindingSource(TableDataSources.AssignmentDetailsDataTable assignmentDetailsDataTable)
        {
            if (!_PauseJobDetailsRefresh)
                _AssignmentDetailsDataTable = assignmentDetailsDataTable;

            RefreshTables();
        }

        TableDataSources.AssignmentDetailsDataTable _AssignmentDetailsDataTable = new Surge.ControlPanel.TableDataSources.AssignmentDetailsDataTable();
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
                    lock (_AssignmentDetailsDataTable)
                    {
                        string sort = assignmentDetailsBindingSource.Sort;
                        assignmentDetailsGridView.SuspendLayout();
                        try
                        {
                            List<string> selectedRows = new List<string>();
                            foreach (DataGridViewRow r in assignmentDetailsGridView.SelectedRows)
                                selectedRows.Add(r.Cells[10].Value.ToString());

                            int vs = assignmentDetailsGridView.FirstDisplayedScrollingRowIndex;
                            int hs = assignmentDetailsGridView.FirstDisplayedScrollingColumnIndex;

                            string f = assignmentsFilterTextbox.Text.Trim();

                            string filter = "";
                            if (f.Length > 0)
                            {
                                f = f.Replace("]", "]]");
                                f = f.Replace("[", "[[]");
                                f = f.Replace("]]", "[]]");
                                f = f.Replace("*", "[*]");
                                f = f.Replace("%", "[%]");

                                filter = "(Manager LIKE '%" + f + "%' OR " +
                                           "BranchIP LIKE '%" + f + "%' OR " +
                                           "BranchName LIKE '%" + f + "%' OR " +
                                           "Controller LIKE '%" + f + "%' OR " +
                                           "Source LIKE '%" + f + "%')";
                            }

                            int col = 0;
                            foreach (string af in _ColumnFilter.Filters)
                            {
                                if (af != "")
                                {
                                    if (filter != "")
                                        filter += " AND ";

                                    switch (col)
                                    {
                                        case 0:
                                            filter += "Manager LIKE '%" + af + "%'";
                                            break;

                                        case 1:
                                            filter += "(BranchIP LIKE '%" + af + "%' OR BranchName LIKE '%" + af + "%')";
                                            break;

                                        case 2:
                                            filter += "Controller LIKE '%" + af + "%'";
                                            break;

                                        case 3:
                                            filter += "Source LIKE '%" + af + "%'";
                                            break;
                                    }
                                }

                                col++;
                            }

                            if (_IncompleteOnly)
                            {
                                if (filter.Length > 0)
                                    filter += " AND ";

                                filter += "Completed <= '" + DateTime.MinValue + "'";
                            }

                            DataGridViewColumn sc = assignmentDetailsGridView.SortedColumn;
                            SortOrder so = assignmentDetailsGridView.SortOrder;


                            if (filter != "")
                            {
                                List<TimeSpan> deltas = _AssignmentDetailsDataTable.Select(filter).Where(i => (DateTime)i["Received"] > DateTime.MinValue && (DateTime)i["Completed"] > DateTime.MinValue).Select(i => (DateTime)i["Completed"] - (DateTime)i["Received"]).ToList();
                                if (deltas.Count > 0)
                                {
                                    TimeSpan a = new TimeSpan((long)deltas.Select(i => i.Ticks).Average());

                                    if ((int)a.TotalMinutes > 0)
                                        avgExecutionTime.Text = "Average Execution Time: " + a.TotalMinutes.ToString("0.0000") + "m";
                                    else
                                        avgExecutionTime.Text = "Average Execution Time: " + a.TotalSeconds.ToString("0.0000") + "s";
                                }
                                else
                                {
                                    avgExecutionTime.Text = "Average Execution Time: ";
                                }
                            }
                            else
                            {
                                List<TimeSpan> deltas = _AssignmentDetailsDataTable.Where(i => (DateTime)i["Received"] > DateTime.MinValue && (DateTime)i["Completed"] > DateTime.MinValue).Select(i => (DateTime)i["Completed"] - (DateTime)i["Received"]).ToList();
                                if (deltas.Count > 0)
                                {
                                    TimeSpan a = new TimeSpan((long)deltas.Select(i => i.Ticks).Average());

                                    if ((int)a.TotalMinutes > 0)
                                        avgExecutionTime.Text = "Average Execution Time: " + a.TotalMinutes.ToString("0.0000") + "m";
                                    else
                                        avgExecutionTime.Text = "Average Execution Time: " + a.TotalSeconds.ToString("0.0000") + "s";
                                }
                                else
                                {
                                    avgExecutionTime.Text = "Average Execution Time: ";
                                }
                            }

                            DataView tmpDV = new DataView(_AssignmentDetailsDataTable);

                            switch (assignmentDetailsGridView.SortOrder)
                            {
                                case SortOrder.Ascending:
                                    sort = assignmentDetailsGridView.SortedColumn.DataPropertyName + " ASC";
                                    break;

                                case SortOrder.Descending:
                                    sort = assignmentDetailsGridView.SortedColumn.DataPropertyName + " DESC";
                                    break;
                            }

                            tmpDV.Sort = sort;
                            tmpDV.RowFilter = filter;

                            TableDataSources.AssignmentDetailsDataTable dt = new TableDataSources.AssignmentDetailsDataTable();
                            if (!_PauseJobDetailsRefresh)
                            {
                                for (int i = 1; i < 500; i++)
                                {
                                    if (i > tmpDV.Count)
                                        break;

                                    dt.Rows.Add(tmpDV[tmpDV.Count - i].Row.ItemArray);
                                }

                                assignmentDetailsBindingSource.DataSource = dt;
                            }
                            else
                            {
                                assignmentDetailsBindingSource.DataSource = tmpDV;
                            }

                            incompleteLabel.Text = rowCountLabel.Text = "(" + tmpDV.Count + ")";

                            if (!_IncompleteOnly)
                            {
                                DataView dv = new DataView(_AssignmentDetailsDataTable);

                                if (filter.Length > 0)
                                    filter += " AND ";

                                filter += "Completed <= '" + DateTime.MinValue + "'";

                                dv.RowFilter = filter;

                                incompleteLabel.Text = "(" + dv.Count + ")";
                            }

                            if (assignmentDetailsGridView.Rows.Count > 0)
                            {
                                assignmentDetailsBindingSource.Sort = sort;

                                assignmentDetailsGridView.ClearSelection();

                                int firstSelectedRow = -1;
                                foreach (DataGridViewRow r in assignmentDetailsGridView.Rows)
                                    if (r.Cells[10].Value != null)
                                        if (selectedRows.Contains(r.Cells[10].Value.ToString()))
                                        {
                                            if (firstSelectedRow == -1)
                                                firstSelectedRow = r.Index;

                                            r.Selected = true;
                                        }

                                if (firstSelectedRow > -1)
                                    vs = firstSelectedRow;

                                if (vs > -1 && vs < assignmentDetailsGridView.RowCount && !scrollBottom.Checked)
                                    assignmentDetailsGridView.FirstDisplayedScrollingRowIndex = vs;
                                else
                                    assignmentDetailsGridView.FirstDisplayedScrollingRowIndex = assignmentDetailsGridView.RowCount - 1;

                                if (hs > -1 && hs < assignmentDetailsGridView.ColumnCount)
                                    assignmentDetailsGridView.FirstDisplayedScrollingColumnIndex = hs;
                            }
                        }
                        catch (Exception ex)
                        {
                            STEM.Sys.EventLog.WriteEntry("Surge.AssignmentDetailsView.RefreshTables", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                        }
                        finally
                        {
                            assignmentDetailsGridView.ResumeLayout();
                        }
                    }
                }
                catch (Exception ex)
                {
                    STEM.Sys.EventLog.WriteEntry("Surge.AssignmentDetailsView.RefreshTables", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                }
            }
        }

        ColumnFilter _ColumnFilter = null;
        private void advancedFilter_Click(object sender, EventArgs e)
        {
            _ColumnFilter.ShowDialog(this);
        }
        
        private void pauseDetailsGridButton_Click(object sender, EventArgs e)
        {
            if (_PauseJobDetailsRefresh)
            {
                _PauseJobDetailsRefresh = false;
                pauseDetailsGridButton.Image = Surge.ControlPanel.Properties.Resources.pause;
            }
            else
            {
                _PauseJobDetailsRefresh = true;
                pauseDetailsGridButton.Image = Surge.ControlPanel.Properties.Resources.play;
            }
        }
                
        void jobDetailsGridView_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewRow r = assignmentDetailsGridView.Rows[e.RowIndex];

            InstructionSetRequested m = new InstructionSetRequested(r.Cells[1].Value.ToString(), Guid.Parse(r.Cells[10].Value.ToString()), r.Cells[11].Value.ToString());

            m = _UIActor.Send(m, r.Cells[0].Value.ToString(), TimeSpan.FromSeconds(2)) as InstructionSetRequested;

            if (m != null && m.InstructionSetXml != null)
                ShowJobDetails(m.InstructionSet);
        }

        void ShowJobDetails(Surge._InstructionSet iSet)
        {
            try
            {
                if (iSet != null)
                {
                    InstructionSetDetails ed = new InstructionSetDetails(iSet, _UIActor);
                    ed.ShowDialog(this);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        bool _IncompleteOnly = false;
        private void incomplete_CheckedChanged(object sender, EventArgs e)
        {
            _IncompleteOnly = incomplete.Checked;
            RefreshTables();
        }

        private void clearAssignmentFilter_Click(object sender, EventArgs e)
        {
            assignmentsFilterTextbox.Text = "";
            RefreshTables();
        }
    }
}
