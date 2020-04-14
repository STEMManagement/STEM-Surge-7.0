using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Net.Sockets;

namespace Installer
{
    public partial class InstallSurge : UserControl
    {
        public EventHandler onComplete;
        public bool Advance { get; set; }

        public InstallSurge()
        {
            InitializeComponent();

            userGroup.Enabled = runAsUserCB.Checked;

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    envManagers.Text = ip.ToString();
                    break;
                }
            }
        }

        private void managerRB_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void branchRB_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void uiRB_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void runAsUserCB_CheckedChanged(object sender, EventArgs e)
        {
            userGroup.Enabled = runAsUserCB.Checked;
        }

        public void CopyManager()
        {
            string installPath = @"C:\Program Files\STEM Management\STEM.Surge";

            string extensionsPath = remoteConfigurationDir.Text.Trim().Replace("[MANAGERS]", "localhost");

            extensionsPath = Path.Combine(extensionsPath, "Extensions");

            CopyBranch();
            
            if (!File.Exists(Path.Combine(installPath, "STEM.Auth.dll")))
                File.Copy(@".\Package\STEM.SurgeService\STEM.Auth.dll", Path.Combine(installPath, "STEM.Auth.dll"));
              
            if (!Directory.Exists(extensionsPath))
            {
                try
                {
                    Directory.CreateDirectory(extensionsPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Error Creating Install Directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            
            foreach (string dll in Directory.GetFiles(@".\Package\STEM.SurgeService\Extensions", "*"))
                try
                {
                    File.Copy(dll, Path.Combine(extensionsPath, Path.GetFileName(dll)), true);
                }
                catch { }

            foreach (string dir in Directory.GetDirectories(@".\Package\STEM.SurgeService\Extensions"))
            {
                string d = Path.GetFileName(dir);

                string np = Path.Combine(extensionsPath, d);

                if (!Directory.Exists(np))
                    Directory.CreateDirectory(np);

                foreach (string dll in Directory.GetFiles(dir, "*"))
                    try
                    {
                        File.Copy(dll, Path.Combine(np, Path.GetFileName(dll)), true);
                    }
                    catch { }
            }

            CopyUI();
        }

        public void CopyUI()
        {
            string installPath = @"C:\Program Files\STEM Management\STEM.Surge\ControlPanel";

            if (!Directory.Exists(installPath))
            {
                try
                {
                    Directory.CreateDirectory(installPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Error Creating Install Directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            
            foreach (string file in Directory.GetFiles(@".\Package\STEM.SurgeService\ControlPanel", "*", SearchOption.TopDirectoryOnly))
            {
                File.Copy(file, Path.Combine(installPath, Path.GetFileName(file)), true);
            }
            
            if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "STEM Surge.url")))
                File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "STEM Surge.url"));

            using (StreamWriter writer = new StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "STEM Surge.url")))
            {
                string app = System.Reflection.Assembly.GetExecutingAssembly().Location;
                writer.WriteLine("[InternetShortcut]");
                writer.WriteLine("URL=file:///" + @"C:\Program Files\STEM Management\STEM.Surge\ControlPanel\STEM.Surge.ControlPanel.exe");
                writer.WriteLine("IconIndex=0");
                string icon = app.Replace('\\', '/');
                writer.WriteLine("IconFile=" + @"C:\Program Files\STEM Management\STEM.Surge\ControlPanel\SurgeLogoFull.ico");
                writer.Flush();
            }
        }

        public void CopyBranch()
        {
            string installPath = @"C:\Program Files\STEM Management\STEM.Surge";

            if (!Directory.Exists(installPath))
            {
                try
                {
                    Directory.CreateDirectory(installPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Error Creating Install Directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            foreach (string file in Directory.GetFiles(@".\Package\STEM.SurgeService", "*", SearchOption.TopDirectoryOnly))
            {
                if (!file.EndsWith("STEM.Auth.dll", StringComparison.InvariantCultureIgnoreCase))
                {
                    File.Copy(file, Path.Combine(installPath, Path.GetFileName(file)), true);
                }
            }
            
            string cfg = @"<?xml version='1.0' standalone='yes'?>
<ConfigurationDS xmlns='http://tempuri.org/ConfigurationDS.xsd'>
  <Settings>
    <SurgeCommunicationPort>[PORT]</SurgeCommunicationPort>
    <SurgeDeploymentManagerAddress>[ADDRESS]</SurgeDeploymentManagerAddress>
    <ProcessorOverload>[OVERLOAD]</ProcessorOverload>
    <PostMortemDirectory>[POSTMORTEM]</PostMortemDirectory>
    <RemoteConfigurationDirectory>[REMOTECFG]</RemoteConfigurationDirectory>
    <AlternateAssemblyStore />
    <UseSSL>[USESSL]</UseSSL>
  </Settings>
</ConfigurationDS>";

            cfg = cfg.Replace("[PORT]", envPort.Text.Trim());
            cfg = cfg.Replace("[ADDRESS]", envManagers.Text.Trim());
            cfg = cfg.Replace("[OVERLOAD]", processorOverload.Text.Trim());
            cfg = cfg.Replace("[POSTMORTEM]", postmortemOutputDir.Text.Trim());
            cfg = cfg.Replace("[REMOTECFG]", remoteConfigurationDir.Text.Trim());
            cfg = cfg.Replace("[USESSL]", useSSL.Checked.ToString().ToLower());

            File.WriteAllText(Path.Combine(installPath, "SurgeService.cfg"), cfg);
        }
        
        private void button1_Click(object sender, EventArgs e)
        {                                    
            if (managerRB.Checked)
            {
                CopyManager();

                string exe = "SC.EXE CREATE STEM.Surge binPath= \"C:\\Program Files\\STEM Management\\STEM.Surge\\STEM.SurgeService.exe\" start= auto DisplayName= STEM.Surge";
                if (runAsUserCB.Checked)
                    exe += " obj= " + userName.Text.Trim() + " password= " + password.Text.Trim();

                string tfn = System.IO.Path.GetTempFileName() + ".bat";

                try
                {
                    File.WriteAllText(tfn, exe);
                    System.Diagnostics.Process p = System.Diagnostics.Process.Start(tfn);

                    while (!p.HasExited)
                        System.Threading.Thread.Sleep(1000);
                }
                finally
                {
                    File.Delete(tfn);
                }
            }
            else if (branchRB.Checked)
            {
                CopyBranch();

                string exe = "SC.EXE CREATE STEM.Surge binPath= \"C:\\Program Files\\STEM Management\\STEM.Surge\\STEM.SurgeService.exe\" start= auto DisplayName= STEM.Surge";
                if (runAsUserCB.Checked)
                    exe += " obj= " + userName.Text.Trim() + " password= " + password.Text.Trim();

                string tfn = System.IO.Path.GetTempFileName() + ".bat";

                try
                {
                    File.WriteAllText(tfn, exe);
                    System.Diagnostics.Process p = System.Diagnostics.Process.Start(tfn);

                    while (!p.HasExited)
                        System.Threading.Thread.Sleep(1000);
                }
                finally
                {
                    File.Delete(tfn);
                }
            }
            else
            {
                CopyUI();
            }

            Advance = false;
            onComplete(this, EventArgs.Empty);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Advance = false;
            onComplete(this, EventArgs.Empty);
        }
    }
}
