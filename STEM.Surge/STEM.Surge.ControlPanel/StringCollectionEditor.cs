using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace STEM.Surge.ControlPanel
{
    public partial class StringCollectionEditor : Form
    {
        List<string> _Original;
        System.ComponentModel.PropertyDescriptor _Descriptor;
        object _BoundObject;

        public bool PropertyValueChanged { get; private set; }

        public StringCollectionEditor(System.ComponentModel.PropertyDescriptor descriptor, object boundObject)
        {
            InitializeComponent();

            PropertyValueChanged = false;

            _Descriptor = descriptor;
            _BoundObject = boundObject;

            _Original = _Descriptor.GetValue(boundObject) as List<string>;

            this.Text = _Descriptor.DisplayName;

            strings.Lines = _Original.ToArray();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (Updated())
            {
                if (MessageBox.Show(this, "Cancel Changes?", "Unsaved", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            base.OnFormClosing(e);
        }

        private void apply_Click(object sender, EventArgs e)
        {
            if (Updated())
            {
                PropertyValueChanged = true;
                _Descriptor.SetValue(_BoundObject, new List<string>(strings.Lines));
                _Original = strings.Lines.ToList();
            }

            Close();
        }

        bool Updated()
        {
            List<string> edited = new List<string>(strings.Lines);

            bool updated = false;
            foreach (string s in _Original)
                if (!edited.Contains(s))
                {
                    updated = true;
                    break;
                }

            if (!updated)
                foreach (string s in edited)
                    if (!_Original.Contains(s))
                    {
                        updated = true;
                        break;
                    }

            return updated;
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            strings.Lines = _Original.ToArray();

            Close();
        }
    }
}
