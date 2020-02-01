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
    public partial class Notes : Form
    {
        string _SwitchboardRowID = null;
        UIActor _UIActor = null;
        Messages.Notes _Notes = null;

        public Notes(UIActor uiActor, string switchboardRowID)
        {
            InitializeComponent();

            _UIActor = uiActor;
            _SwitchboardRowID = switchboardRowID;

            Messages.Notes n = new Messages.Notes { SwitchboardRowID = _SwitchboardRowID, Action = Messages.Notes.ActionType.Get };
            n.onResponse += Notes_onResponse;
            if (!_UIActor.Send(n))
            {
                MessageBox.Show(this, "There was a problem communicating with the server.", "Message Delivery Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void Notes_onResponse(Sys.Messaging.Message delivered, Sys.Messaging.Message response)
        {
            if (response is Messages.Notes)
            {
                _Notes = response as Messages.Notes;
                UpdateText();
            }
            else
            {
                MessageBox.Show(this, "No notes were found.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
        }

        void UpdateText()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new System.Threading.ThreadStart(UpdateText));
            }
            else
            {
                richTextBox1.Text = _Notes.Text;
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = _Notes.Text;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            _Notes.Text = richTextBox1.Text;
            _Notes.Action = Messages.Notes.ActionType.Set;
            if (!_UIActor.Send(_Notes))
                MessageBox.Show(this, "There was a problem saving your changes.", "Message Delivery Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);            
            else
                Close();
        }
    }
}
