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
    public partial class DMConnect : Form
    {
        public string IP { get; private set; }
        public bool UseSSL { get; private set; }

        public DMConnect()
        {
            InitializeComponent();

            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);

            if (config.AppSettings.Settings["IPHistory"] == null)
                config.AppSettings.Settings.Add("IPHistory", STEM.Sys.IO.Net.MachineIP());

            if (config.AppSettings.Settings["LastIP"] == null)
                config.AppSettings.Settings.Add("LastIP", STEM.Sys.IO.Net.MachineIP());

            string ipHistory = config.AppSettings.Settings["IPHistory"].Value;
            string lastIP = config.AppSettings.Settings["LastIP"].Value;

            ipAddress.Text = lastIP;

            ipAddress.Items.AddRange(config.AppSettings.Settings["IPHistory"].Value.Split(new char[] { ',' }));

            config.Save(System.Configuration.ConfigurationSaveMode.Modified);
        }
        
        private void ok_Click(object sender, EventArgs e)
        {
            IP = ipAddress.Text.Trim();
            UseSSL = useSSL.Checked;

            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);

            config.AppSettings.Settings["LastIP"].Value = IP;

            if (!config.AppSettings.Settings["IPHistory"].Value.Contains(IP))
            {
                List<string> ips = new List<string>(config.AppSettings.Settings["IPHistory"].Value.Split(new char[] { ',' }));
                if (!ips.Contains(IP))
                    ips.Insert(0, IP);

                while (ips.Count > 10)
                    ips.RemoveAt(11);

                config.AppSettings.Settings["IPHistory"].Value = String.Join(",", ips);
            }

            config.Save(System.Configuration.ConfigurationSaveMode.Modified);

            DialogResult = System.Windows.Forms.DialogResult.OK;

            IP = STEM.Sys.IO.Net.MachineAddress(IP);

            Close();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }
    }
}
