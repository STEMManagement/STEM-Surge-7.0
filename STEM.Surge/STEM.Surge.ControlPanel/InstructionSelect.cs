using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace STEM.Surge.ControlPanel
{
    public partial class InstructionSelect : Form
    {
        public Type SelectedType { get; private set; }

        List<InstructionType> _InstructionTypes = new List<InstructionType>();

        class InstructionType
        {
            public Version Version { get; set; }
            public string TypeName { get; set; }
            public string Description { get; set; }
            public Type Type { get; set; }

            public InstructionType(Type t)
            {
                Type = t;

                TypeName = t.FullName;

                Version = new AssemblyName(Type.Assembly.FullName).Version;

                Description = GetDescription();
            }

            string GetDescription()
            {
                string ret = "";

                object[] displayName = (DisplayNameAttribute[])
                    Type.GetCustomAttributes(typeof(DisplayNameAttribute), false);

                if (displayName.Length > 0)
                {
                    ret = ((DisplayNameAttribute)displayName[0]).DisplayName;
                }

                object[] descriptions = (DescriptionAttribute[])
                    Type.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (descriptions.Length > 0)
                {
                    ret += Environment.NewLine + Environment.NewLine + ((DescriptionAttribute)descriptions[0]).Description;
                }

                return ret;
            }
        }
        
        public InstructionSelect(List<Type> types)
        {
            InitializeComponent();
                        
            foreach (Type t in types)
            {
                _InstructionTypes.Add(new InstructionType(t));
            }

            List<string> keywords = new List<string>();

            foreach (string s in _InstructionTypes.OrderBy(i => i.TypeName).Select(i => i.TypeName).Distinct())
                keywords.AddRange(s.Split('.'));
            
            List<System.Drawing.Color> colors = new List<System.Drawing.Color>();
            colors.Add(System.Drawing.Color.DarkBlue);
            colors.Add(System.Drawing.Color.DarkGreen);
            colors.Add(System.Drawing.Color.DarkRed);
            int c = 0;
            foreach (string s in keywords.Distinct().OrderBy(i => i))
            {
                keyWords.SelectionColor = colors[c % 3];
                c++;
                keyWords.AppendText(s + "   ");
            }

            Bind(_InstructionTypes);

            dataGridView1.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(dataGridView1_EditingControlShowing);
            dataGridView1.CellClick += DataGridView1_CellClick;
            dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;
        }

        void Bind(List<InstructionType> types)
        {
            dataGridView1.Rows.Clear();

            foreach (string s in types.OrderBy(i => i.TypeName).Select(i => i.TypeName).Distinct())
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = s;

                foreach (Version v in types.Where(i => i.TypeName == s).OrderBy(i => i.Version).Select(i => i.Version))
                {
                    ((DataGridViewComboBoxCell)dataGridView1.Rows[index].Cells[1]).Items.Add(v.ToString());
                    ((DataGridViewComboBoxCell)dataGridView1.Rows[index].Cells[1]).Value = v.ToString();
                }
            }

            InstructionType t = types.OrderBy(i => i.TypeName).FirstOrDefault();

            descriptionRTB.Text = "";
            if (t != null)
            {
                descriptionRTB.Text = t.Description;
            }
        }

        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            addButton_Click(sender, e);
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string selectedType = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value as string;
            string selectedVersion = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[1].Value as string;

            InstructionType t = _InstructionTypes.Where(i => i.TypeName == selectedType && i.Version.ToString() == selectedVersion).OrderByDescending(i => i.Version).First();

            descriptionRTB.Text = "";
            if (t != null)
            {
                descriptionRTB.Text = t.Description;
            }
        }

        void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == 2)
            {
                ComboBox cb = e.Control as ComboBox;
                cb.SelectedIndexChanged -= new EventHandler(versionComboBox_SelectedIndexChanged);
                cb.SelectedIndexChanged += new EventHandler(versionComboBox_SelectedIndexChanged);
            }
        }

        private void versionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedType = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value as string;
            string selectedVersion = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[1].Value as string;

            InstructionType t = _InstructionTypes.Where(i => i.TypeName == selectedType && i.Version.ToString() == selectedVersion).OrderByDescending(i => i.Version).First();

            descriptionRTB.Text = "";
            if (t != null)
            {
                descriptionRTB.Text = t.Description;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            SelectedType = null;
            this.Close();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            string selectedType = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value as string;
            string selectedVersion = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[1].Value as string;

            InstructionType t = _InstructionTypes.Where(i => i.TypeName == selectedType && i.Version.ToString() == selectedVersion).OrderByDescending(i => i.Version).First();

            SelectedType = null;
            descriptionRTB.Text = "";
            if (t != null)
            {
                SelectedType = t.Type;
                descriptionRTB.Text = t.Description;
            }

            this.Close();
        }

        private void filterBox_TextChanged(object sender, EventArgs e)
        {
            string f = filterBox.Text.Trim();

            if (f.Length == 0)
            {
                Bind(_InstructionTypes);
            }
            else
            {
                List<InstructionType> bind = new List<InstructionType>(_InstructionTypes);
                foreach (string s in f.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    if (s.Trim().Length > 0)
                        bind = bind.Where(i => i.TypeName.ToUpper().Split('.').ToList().Exists(j => j.Contains(s.ToUpper().Trim()))).ToList();

                Bind(bind.Distinct().ToList());
            }
        }
        
        private void clearFilter_Click(object sender, EventArgs e)
        {
            filterBox.Text = "";
            filterBox_TextChanged(sender, e);
        }
    }
}
