using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Compression;
using System.IO;
using Microsoft.Win32;
using System.Windows.Forms;

namespace Installer
{
    public partial class Form1 : Form
    {
        Agreement _Agreement = new Agreement();
        InstallNet4 _InstallNet4 = new InstallNet4();
        Existing _Existing = new Existing();
        Finished _Finished = new Finished();
        InstallSurge _Install = new InstallSurge();

        int _NetFrameworkVersion = 0;

        public Form1()
        {
            InitializeComponent();

            if (Directory.Exists(@".\Package"))
                Directory.Delete(@".\Package", true);

            ZipFile.ExtractToDirectory(@".\pkg.zip", @".\Package");

            _Agreement.onComplete += onComplete;
            _Agreement.Dock = DockStyle.Fill;
            _InstallNet4.onComplete += onComplete;
            _InstallNet4.Dock = DockStyle.Fill;
            _Existing.onComplete += onComplete;
            _Existing.Dock = DockStyle.Fill;
            _Finished.onComplete += onComplete;
            _Finished.Dock = DockStyle.Fill;
            _Install.onComplete += onComplete;
            _Install.Dock = DockStyle.Fill;

            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                _NetFrameworkVersion = Convert.ToInt32(ndpKey.GetValue("Release"));
            }

            if (Directory.Exists(@"C:\Program Files\STEM Management\STEM.Surge") && 
                File.Exists(@"C:\Program Files\STEM Management\STEM.Surge\STEM.SurgeService.exe"))
            {
                panel2.Controls.Add(_Existing);
            }
            else
            {
                panel2.Controls.Add(_Agreement);
            }
        }
        
        void onComplete(object sender, EventArgs args)
        {
            if (sender == _Agreement)
            {
                if (_Agreement.Advance)
                {
                    if (_NetFrameworkVersion < 461808)
                    {
                        panel2.Controls.Clear();
                        panel2.Controls.Add(_InstallNet4);
                    }
                    else
                    {
                        panel2.Controls.Clear();
                        panel2.Controls.Add(_Install);
                    }
                }
                else
                {
                    panel2.Controls.Clear();
                    panel2.Controls.Add(_Finished);
                }
            }
            else if (sender == _InstallNet4)
            {
                if (_InstallNet4.Advance)
                {
                    if (_NetFrameworkVersion < 461808)
                    {
                        MessageBox.Show(this, "You must install the .NET Framework.", "Cannot Continue.", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                    else
                    {
                        if (_InstallNet4.Advance)
                        {
                            panel2.Controls.Clear();
                            panel2.Controls.Add(_Install);
                        }
                        else
                        {
                            panel2.Controls.Clear();
                            panel2.Controls.Add(_Finished);
                        }
                    }
                }
            }
            else if (sender == _Install)
            {
                panel2.Controls.Clear();
                panel2.Controls.Add(_Finished);
            }
            else if (sender == _Existing)
            {
                switch (_Existing.SelectedAction)
                {
                    case Existing.Operation.Exit:
                        {
                            panel2.Controls.Clear();
                            panel2.Controls.Add(_Finished);
                            break;
                        }

                    case Existing.Operation.Uninstall:
                        {
                            if (File.Exists(@"C:\Program Files\STEM Management\STEM.Surge\STEM.SurgeService.exe"))
                            {
                                string tfn = System.IO.Path.GetTempFileName() + ".bat";

                                try
                                {
                                    File.WriteAllText(tfn, "SC.EXE STOP STEM.Surge");
                                    System.Diagnostics.Process p = System.Diagnostics.Process.Start(tfn);

                                    while (!p.HasExited)
                                        System.Threading.Thread.Sleep(1000);
                                }
                                catch { }
                                finally
                                {
                                    File.Delete(tfn);
                                }

                                tfn = System.IO.Path.GetTempFileName() + ".bat";

                                try
                                {
                                    File.WriteAllText(tfn, "SC.EXE DELETE STEM.Surge");
                                    System.Diagnostics.Process p = System.Diagnostics.Process.Start(tfn);

                                    while (!p.HasExited)
                                        System.Threading.Thread.Sleep(1000);
                                }
                                finally
                                {
                                    File.Delete(tfn);
                                }
                            }

                            if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "STEM Surge.url")))
                                File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "STEM Surge.url"));

                            foreach (string f in Directory.GetFiles(@"C:\Program Files\STEM Management\STEM.Surge", "*.dll"))
                            {
                                if (!f.EndsWith("STEM.Auth.dll", StringComparison.InvariantCultureIgnoreCase))
                                    try
                                    {
                                        File.Delete(f);
                                    }
                                    catch { }
                            }

                            foreach (string f in Directory.GetFiles(@"C:\Program Files\STEM Management\STEM.Surge", "*.exe"))
                            {
                                try
                                {
                                    File.Delete(f);
                                }
                                catch { }
                            }

                            if (Directory.Exists(@"C:\Program Files\STEM Management\STEM.Surge\ControlPanel"))
                                try
                                {
                                    Directory.Delete(@"C:\Program Files\STEM Management\STEM.Surge\ControlPanel", true);
                                }
                                catch { }

                            panel2.Controls.Clear();
                            panel2.Controls.Add(_Finished);
                            break;
                        }

                    case Existing.Operation.Update:
                        {
                            bool manager = false;
                            if (File.Exists(@"C:\Program Files\STEM Management\STEM.Surge\STEM.Auth.dll"))
                                manager = true;

                            foreach (string f in Directory.GetFiles(@"C:\Program Files\STEM Management\STEM.Surge", "*.dll"))
                            {
                                if (!f.EndsWith("STEM.Auth.dll", StringComparison.InvariantCultureIgnoreCase))
                                    try
                                    {
                                        File.Delete(f);
                                    }
                                    catch { }
                            }

                            foreach (string f in Directory.GetFiles(@"C:\Program Files\STEM Management\STEM.Surge", "*.exe"))
                            {
                                try
                                {
                                    File.Delete(f);
                                }
                                catch { }
                            }

                            if (Directory.Exists(@"C:\Program Files\STEM Management\STEM.Surge\ControlPanel"))
                                try
                                {
                                    Directory.Delete(@"C:\Program Files\STEM Management\STEM.Surge\ControlPanel", true);
                                }
                                catch { }

                            if (manager)
                                _Install.CopyManager();
                            else
                                _Install.CopyBranch();

                            panel2.Controls.Clear();
                            panel2.Controls.Add(_Finished);
                            break;
                        }
                }
            }
            else if (sender == _Finished)
            {
                if (Directory.Exists(@".\Package"))
                    Directory.Delete(@".\Package", true);

                Close();
            }
        }
    }
}
