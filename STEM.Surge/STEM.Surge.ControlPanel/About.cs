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
    public partial class About : Form
    {
        public About(UIActor messageClient)
        {
            InitializeComponent();

            if (messageClient.DeploymentManagerConfiguration.MessageConnection == null)
            {
                MessageBox.Show(this, "No active connection.", "Connect", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            
            label11.Text = "Manager IP: " + messageClient.DeploymentManagerConfiguration.MessageConnection.RemoteAddress;

            label4.Text = "Build Date: " + messageClient.DeploymentManagerConfiguration.BuildDate.ToString("G");


            if (messageClient.DeploymentManagerConfiguration.IsSES)
            {
                label10.Text = "Limited Execution License, Please contact STEM Management for a full license.";
                label9.Text = "Allowable Branches: 1";
                label8.Text = "No Expiration";
            }
            else
            {
                label10.Text = "Licensed Install";
                if (messageClient.DeploymentManagerConfiguration.AllowableBranches < Int32.MaxValue)
                {
                    label9.Text = "Allowable Branches: " + messageClient.DeploymentManagerConfiguration.AllowableBranches;
                }
                else
                {
                    label9.Text = "Unlimited Branches";
                }

                if (messageClient.DeploymentManagerConfiguration.DaysRemaining > 0)
                {
                    label8.Text = "License Expiration: " + DateTime.UtcNow.AddDays(messageClient.DeploymentManagerConfiguration.DaysRemaining).ToString("MM/dd/yyyy");
                }
                else
                {
                    label8.Text = "No Expiration";
                }
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
    }
}
