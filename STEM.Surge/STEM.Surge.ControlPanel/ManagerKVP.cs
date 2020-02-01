using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STEM.Surge.ControlPanel
{
    public partial class ManagerKVP : Form
    {
        SwitchboardConfig _SwitchboardConfiguration = new SwitchboardConfig();

        SwitchboardConfig.ConfigurationMacroMapDataTable _Clone = null;

        public SwitchboardConfig SwitchboardConfiguration
        {
            get
            {
                return _SwitchboardConfiguration;
            }

            set
            {
                _SwitchboardConfiguration = value;

                _Clone = (SwitchboardConfig.ConfigurationMacroMapDataTable)_SwitchboardConfiguration.ConfigurationMacroMap.Clone();

                foreach (SwitchboardConfig.ConfigurationMacroMapRow r in _SwitchboardConfiguration.ConfigurationMacroMap)
                    _Clone.ImportRow(r);

                Bind();
            }
        }

        public ManagerKVP(SwitchboardConfig config)
        {
            InitializeComponent();
            
            macroPlaceholderGrid.RowHeaderMouseClick += macroPlaceholderGrid_RowHeaderMouseClick;

            SwitchboardConfiguration = config;
        }

        bool _DirtyConfig = false;
        public bool IsDirty
        {
            get
            {
                return _DirtyConfig;
            }
        }

        public void Bind()
        {
            macroPlaceholderGrid.CurrentCellDirtyStateChanged -= new EventHandler(GridView_CurrentCellDirtyStateChanged);
            _SwitchboardConfiguration.ConfigurationMacroMap.ConfigurationMacroMapRowDeleted -= new SwitchboardConfig.ConfigurationMacroMapRowChangeEventHandler(Cfg_RowChanged);

            try
            {
                _SwitchboardConfiguration.ConfigurationMacroMap.Clear();

                foreach (SwitchboardConfig.ConfigurationMacroMapRow r in _Clone)
                    _SwitchboardConfiguration.ConfigurationMacroMap.ImportRow(r);

                if (_SwitchboardConfiguration.ConfigurationMacroMap.FindByPlaceholder("[BRANCHES]") == null)
                    _SwitchboardConfiguration.ConfigurationMacroMap.AddConfigurationMacroMapRow("[BRANCHES]", "Reserved");

                if (_SwitchboardConfiguration.ConfigurationMacroMap.FindByPlaceholder("[MANAGERS]") == null)
                    _SwitchboardConfiguration.ConfigurationMacroMap.AddConfigurationMacroMapRow("[MANAGERS]", "Reserved");

                if (_SwitchboardConfiguration.ConfigurationMacroMap.FindByPlaceholder("[NOSOURCE]") == null)
                    _SwitchboardConfiguration.ConfigurationMacroMap.AddConfigurationMacroMapRow("[NOSOURCE]", "Reserved");

                if (_SwitchboardConfiguration.ConfigurationMacroMap.FindByPlaceholder("[DeploymentManagerIP]") == null)
                    _SwitchboardConfiguration.ConfigurationMacroMap.AddConfigurationMacroMapRow("[DeploymentManagerIP]", "Reserved");

                _SwitchboardConfiguration.AcceptChanges();

                configurationMacroMapDataTableBindingSource.DataSource = _SwitchboardConfiguration.ConfigurationMacroMap;

                _DirtyConfig = false;
                Save.Enabled = false;
                Cancel.Enabled = false;
            }
            finally
            {
                macroPlaceholderGrid.CurrentCellDirtyStateChanged += new EventHandler(GridView_CurrentCellDirtyStateChanged);
                _SwitchboardConfiguration.ConfigurationMacroMap.ConfigurationMacroMapRowDeleted += new SwitchboardConfig.ConfigurationMacroMapRowChangeEventHandler(Cfg_RowChanged);

                foreach (DataGridViewRow r in macroPlaceholderGrid.Rows)
                    if ((string)r.Cells[2].Value == "[BRANCHES]" || (string)r.Cells[2].Value == "[MANAGERS]" || (string)r.Cells[2].Value == "[NOSOURCE]" || (string)r.Cells[2].Value == "[DeploymentManagerIP]")
                        r.ReadOnly = true;

                filterMask_TextChanged(this, null);
            }
        }
        void GridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            _DirtyConfig = true;
            Save.Enabled = true;
            Cancel.Enabled = true;
        }

        void macroPlaceholderGrid_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            macroPlaceholderGrid.ClearSelection();
            macroPlaceholderGrid.Rows[e.RowIndex].Selected = true;
        }

        void Cfg_RowChanged(object sender, object e)
        {
            _DirtyConfig = true;
            Save.Enabled = true;
            Cancel.Enabled = true;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Bind();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            macroPlaceholderGrid.EndEdit();

            Close();
        }

        private void filterMask_TextChanged(object sender, EventArgs e)
        {
            string f = filterMask.Text.Trim();

            if (f.Length == 0)
            {
                configurationMacroMapDataTableBindingSource.Filter = "";
            }
            else
            {
                f = f.Replace("]", "]]");
                f = f.Replace("[", "[[]");
                f = f.Replace("]]", "[]]");
                f = f.Replace("*", "[*]");
                f = f.Replace("%", "[%]");

                configurationMacroMapDataTableBindingSource.Filter =
                    "Placeholder LIKE '%" + f + "%' OR " +
                    "Value LIKE '%" + f + "%'";
            }
        }

        private void ClearFilter_Click(object sender, EventArgs e)
        {
            filterMask.Text = "";
            filterMask_TextChanged(this, null);
        }
    }
}
