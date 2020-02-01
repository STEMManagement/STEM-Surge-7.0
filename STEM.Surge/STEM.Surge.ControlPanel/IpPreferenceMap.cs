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
    public partial class IpPreferenceMap : Form
    {
        SwitchboardConfig _SwitchboardConfiguration = new SwitchboardConfig();

        SwitchboardConfig.IpPreferenceMapDataTable _Clone = null;
        
        public SwitchboardConfig SwitchboardConfiguration
        {
            get
            {
                return _SwitchboardConfiguration;
            }

            set
            {
                _SwitchboardConfiguration = value;

                _Clone = (SwitchboardConfig.IpPreferenceMapDataTable)_SwitchboardConfiguration.IpPreferenceMap.Clone();

                foreach (SwitchboardConfig.IpPreferenceMapRow r in _SwitchboardConfiguration.IpPreferenceMap)
                    _Clone.ImportRow(r);

                Bind();
            }
        }

        public IpPreferenceMap(SwitchboardConfig config)
        {
            InitializeComponent();

            ipPreferenceGrid.RowHeaderMouseClick += ipPreferenceGrid_RowHeaderMouseClick;

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
            ipPreferenceGrid.CurrentCellDirtyStateChanged -= new EventHandler(GridView_CurrentCellDirtyStateChanged);
            _SwitchboardConfiguration.IpPreferenceMap.IpPreferenceMapRowDeleted -= new SwitchboardConfig.IpPreferenceMapRowChangeEventHandler(Cfg_RowChanged);

            try
            {
                _SwitchboardConfiguration.IpPreferenceMap.Clear();

                foreach (SwitchboardConfig.IpPreferenceMapRow r in _Clone)
                    _SwitchboardConfiguration.IpPreferenceMap.ImportRow(r);
                
                _SwitchboardConfiguration.AcceptChanges();

                ipPreferenceMapDataTableBindingSource.DataSource = _SwitchboardConfiguration.IpPreferenceMap;

                _DirtyConfig = false;
                Save.Enabled = false;
                Cancel.Enabled = false;
            }
            finally
            {
                ipPreferenceGrid.CurrentCellDirtyStateChanged += new EventHandler(GridView_CurrentCellDirtyStateChanged);
                _SwitchboardConfiguration.IpPreferenceMap.IpPreferenceMapRowDeleted += new SwitchboardConfig.IpPreferenceMapRowChangeEventHandler(Cfg_RowChanged);
            }
        }

        void ipPreferenceGrid_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            ipPreferenceGrid.ClearSelection();
            ipPreferenceGrid.Rows[e.RowIndex].Selected = true;
        }

        void GridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            _DirtyConfig = true;
            Save.Enabled = true;
            Cancel.Enabled = true;
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
            List<System.Data.DataRowView> rem = new List<System.Data.DataRowView>();
            foreach (System.Data.DataRowView r in ipPreferenceMapDataTableBindingSource)
            {
                if (r["SourceOctets"] is DBNull || r["DestinationOctets"] is DBNull)
                    rem.Add(r);
            }

            foreach (System.Data.DataRowView r in rem)
                ipPreferenceMapDataTableBindingSource.Remove(r);

            ipPreferenceGrid.EndEdit();
            ipPreferenceMapDataTableBindingSource.EndEdit();

            Close();
        }
    }
}
