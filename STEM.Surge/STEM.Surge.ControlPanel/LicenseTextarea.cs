using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using STEM.Sys.Messaging;

namespace STEM.Surge.ControlPanel
{
    public partial class LicenseTextarea : Form
    {
        UIActor _UIActor;

        public LicenseTextarea(UIActor messageClient)
        {
            InitializeComponent();

            if (messageClient.DeploymentManagerConfiguration.MessageConnection == null)
            {
                MessageBox.Show(this, "No active connection.", "Connect", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            _UIActor = messageClient;

            licenseKeys.Text = _UIActor.DeploymentManagerConfiguration.Keys;
        }

        private void test_Click(object sender, EventArgs e)
        {
            //if (licenseKeys.Text.Trim().Length > 0)
            //{
            //    File.WriteAllText(LicFile, licenseKeys.Text.Trim());

            //    int daysRemaining = STEM.Sys.Security.License.DeploymentLimits.DaysRemaining(LicFile, IP);
            //    bool licensed = STEM.Sys.Security.License.DeploymentLimits.DeploymentLicensed(LicFile, IP);

            //    string msg = "The license keys are not valid.";

            //    if (licensed)
            //    {
            //        msg = "The license keys are valid.";

            //        if (daysRemaining >= 0)
            //            msg += " The Deployment Manager Service license has " + daysRemaining + " days remaining.";

            //        int branchLimit = STEM.Sys.Security.License.DeploymentLimits.BranchLimit(LicFile, IP);
            //        if (branchLimit < Int32.MaxValue && branchLimit > -1)
            //            msg += " Registration will be limited to " + branchLimit + " branches.";
            //    }

            //    MessageBox.Show(this, msg, "License", MessageBoxButtons.OK);
            //}
        }

        private void save_Click(object sender, EventArgs e)
        {
            try
            {
                if (licenseKeys.Text.Trim().Length > 0)
                {
                    STEM.Sys.Messaging.Message response = _UIActor.Send(new STEM.Surge.Messages.SetLicense(licenseKeys.Text.Trim()), TimeSpan.FromSeconds(30));

                    if (response is Text)
                    {
                        Text m = response as Text;
                        MessageBox.Show(this, m.TextString, "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(this, "No response.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "Exception!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
