using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace STEM.Surge.ControlPanel
{
    public partial class ColumnFilter : Form
    {
        public List<string> Filters
        {
            get
            {
                return _Filters.ToList();
            }
        }

        List<string> _Filters = new List<string>();

        DataGridViewColumnCollection _ColumnCollection;

        public ColumnFilter(DataGridViewColumnCollection columnCollection)
        {
            _ColumnCollection = columnCollection;

            InitializeComponent();

            string[] headerParts;

            headerParts = _ColumnCollection[0].HeaderText.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
            managerIP.Text = "";
            if (headerParts.Length > 1)
                managerIP.Text = headerParts[1].Replace("*", "");

            headerParts = _ColumnCollection[1].HeaderText.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
            branchIP.Text = "";
            if (headerParts.Length > 1)
                branchIP.Text = headerParts[1].Replace("*", "");

            headerParts = _ColumnCollection[2].HeaderText.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
            deploymentController.Text = "";
            if (headerParts.Length > 1)
                deploymentController.Text = headerParts[1].Replace("*", "");

            headerParts = _ColumnCollection[3].HeaderText.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
            source.Text = "";
            if (headerParts.Length > 1)
                source.Text = headerParts[1].Replace("*", "");

            source.TextChanged += Source_TextChanged;
            deploymentController.TextChanged += DeploymentController_TextChanged;
            branchIP.TextChanged += BranchIP_TextChanged;
            managerIP.TextChanged += ManagerIP_TextChanged;

            _Filters.Add("");
            _Filters.Add("");
            _Filters.Add("");
            _Filters.Add("");

            _Filters[0] = managerIP.Text.Trim();
            _Filters[1] = branchIP.Text.Trim();
            _Filters[2] = deploymentController.Text.Trim();
            _Filters[3] = source.Text.Trim();
        }

        private void ManagerIP_TextChanged(object sender, EventArgs e)
        {
            _Filters[0] = managerIP.Text.Trim();
        }

        private void BranchIP_TextChanged(object sender, EventArgs e)
        {
            _Filters[1] = branchIP.Text.Trim();
        }

        private void DeploymentController_TextChanged(object sender, EventArgs e)
        {
            _Filters[2] = deploymentController.Text.Trim();
        }

        private void Source_TextChanged(object sender, EventArgs e)
        {
            _Filters[3] = source.Text.Trim();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            managerIP.Text = "";
            branchIP.Text = "";
            deploymentController.Text = "";
            source.Text = "";

            button1_Click(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _ColumnCollection[0].HeaderText = _ColumnCollection[0].HeaderText.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries)[0];
            _ColumnCollection[1].HeaderText = _ColumnCollection[1].HeaderText.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries)[0];
            _ColumnCollection[3].HeaderText = _ColumnCollection[3].HeaderText.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries)[0];
            _ColumnCollection[4].HeaderText = _ColumnCollection[4].HeaderText.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries)[0];

            _ColumnCollection[0].HeaderCell.Style.ForeColor = _ColumnCollection[5].HeaderCell.Style.ForeColor;
            _ColumnCollection[1].HeaderCell.Style.ForeColor = _ColumnCollection[5].HeaderCell.Style.ForeColor;
            _ColumnCollection[3].HeaderCell.Style.ForeColor = _ColumnCollection[5].HeaderCell.Style.ForeColor;
            _ColumnCollection[4].HeaderCell.Style.ForeColor = _ColumnCollection[5].HeaderCell.Style.ForeColor;

            if (managerIP.Text.Trim().Length > 0)
            {
                _ColumnCollection[0].HeaderText += " - *" + managerIP.Text.Trim() + "*";
                _ColumnCollection[0].HeaderCell.Style.ForeColor = Color.Red;
            }

            if (branchIP.Text.Trim().Length > 0)
            {
                _ColumnCollection[1].HeaderText += " - *" + branchIP.Text.Trim() + "*";
                _ColumnCollection[1].HeaderCell.Style.ForeColor = Color.Red;
            }

            if (deploymentController.Text.Trim().Length > 0)
            {
                _ColumnCollection[3].HeaderText += " - *" + deploymentController.Text.Trim() + "*";
                _ColumnCollection[3].HeaderCell.Style.ForeColor = Color.Red;
            }

            if (source.Text.Trim().Length > 0)
            {
                _ColumnCollection[4].HeaderText += " - *" + source.Text.Trim() + "*";
                _ColumnCollection[4].HeaderCell.Style.ForeColor = Color.Red;
            }

            Close();
        }

        private void clearManagerIP_Click(object sender, EventArgs e)
        {
            managerIP.Text = "";
        }

        private void clearBranchIP_Click(object sender, EventArgs e)
        {
            branchIP.Text = "";
        }

        private void clearController_Click(object sender, EventArgs e)
        {
            deploymentController.Text = "";
        }

        private void clearSource_Click(object sender, EventArgs e)
        {
            source.Text = "";
        }
    }
}
