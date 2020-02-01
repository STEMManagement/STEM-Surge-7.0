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
    public partial class ServiceConfiguration : Form
    {
        STEM.Surge.Messages.GetServiceConfiguration _ServiceConfiguration = null;
        public int SurgeCommunicationPort { get { return Int32.Parse(envPort.Text.Trim()); } set { envPort.Text = value.ToString(); } }
        public string SurgeDeploymentManagerAddress { get { return envManagers.Text.Trim(); } set { envManagers.Text = value; } }
        public double ProcessorOverload { get { return Double.Parse(processorOverload.Text.Trim()); } set { processorOverload.Text = value.ToString(); } }
        public string PostMortemDirectory { get { return postmortemOutputDir.Text.Trim(); } set { postmortemOutputDir.Text = value; } }
        public string RemoteConfigurationDirectory { get { return remoteConfigurationDir.Text.Trim(); } set { remoteConfigurationDir.Text = value; } }
        public bool UseSSL { get { return useSSL.Checked; } set { useSSL.Checked = value; } }

        public ServiceConfiguration(STEM.Surge.Messages.GetServiceConfiguration branchConfiguration)
        {
            InitializeComponent();

            _ServiceConfiguration = branchConfiguration;

            SurgeCommunicationPort = _ServiceConfiguration.SurgeCommunicationPort;
            SurgeDeploymentManagerAddress = _ServiceConfiguration.SurgeDeploymentManagerAddress;
            ProcessorOverload = _ServiceConfiguration.ProcessorOverload;
            PostMortemDirectory = _ServiceConfiguration.PostMortemDirectory;
            RemoteConfigurationDirectory = _ServiceConfiguration.RemoteConfigurationDirectory;
            UseSSL = _ServiceConfiguration.UseSSL;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
