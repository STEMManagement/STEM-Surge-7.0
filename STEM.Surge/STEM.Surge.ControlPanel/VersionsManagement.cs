using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Reflection;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using STEM.Surge.Messages;
using STEM.Surge;

namespace STEM.Surge.ControlPanel
{
    public partial class VersionsManagement : UserControl
    {
        UIActor _UIActor;
        
        public VersionsManagement(UIActor messageClient)
        {
            InitializeComponent();
            _UIActor = messageClient;

            GetExtensionsSubdirectories subDirs = _UIActor.Send(new GetExtensionsSubdirectories(), TimeSpan.FromSeconds(10)) as GetExtensionsSubdirectories;
            
            extensionsFolders.Items.AddRange(subDirs.ExtensionsSubdirectories.Select(i => System.IO.Path.GetFileName(i)).ToArray());
        }

        void DrillDown(Assembly a, List<string> names)
        {
            string asmLocation = STEM.Sys.Serialization.VersionManager.AssemblyLocation(a);

            if (asmLocation == null)
                return;

            if (names.Contains(asmLocation))
                return;

            foreach (AssemblyName n in a.GetReferencedAssemblies())
            {
                Assembly na = _CachedAsms.FirstOrDefault(x => x.GetName() == n);

                if (na == null)
                    continue;

                DrillDown(na, names);
            }

            names.Add(asmLocation);
        }

        List<Assembly> _CachedAsms = new List<Assembly>();

        private void evaluate_Click(object sender, EventArgs e)
        {
            try
            {
                _CachedAsms = STEM.Sys.Serialization.VersionManager.LoadedAssemblies();
                                
                List<string> needed = new List<string>();

                foreach (STEM.Sys.IO.FileDescription d in _UIActor.DeploymentManagerConfiguration.DeploymentControllers.ToList())
                    try
                    {
                        _DeploymentController dc = _DeploymentController.Deserialize(d.StringContent) as _DeploymentController;

                        if (dc != null)
                            DrillDown(dc.GetType().Assembly, needed);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK);
                    }

                foreach (STEM.Sys.IO.FileDescription d in _UIActor.DeploymentManagerConfiguration.InstructionSetTemplates.ToList())
                    try
                    {
                        Surge.InstructionSet s = Surge.InstructionSet.Deserialize(d.StringContent) as Surge.InstructionSet;
                        if (s != null)
                        {
                            foreach (STEM.Surge.Instruction i in s.Instructions)
                                DrillDown(i.GetType().Assembly, needed);
                            
                            DrillDown(s.GetType().Assembly, needed);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK);
                    }
                
                foreach (string k in _UIActor.DeploymentManagerConfiguration.InstructionSetStatics.Keys)
                    foreach (STEM.Sys.IO.FileDescription d in _UIActor.DeploymentManagerConfiguration.InstructionSetStatics[k].ToList())
                        try
                        {
                            Surge.InstructionSet s = Surge.InstructionSet.Deserialize(d.StringContent) as Surge.InstructionSet;
                            if (s != null)
                            {
                                foreach (STEM.Surge.Instruction i in s.Instructions)
                                    DrillDown(i.GetType().Assembly, needed);

                                DrillDown(s.GetType().Assembly, needed);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK);
                        }

                needed = new List<string>(needed.Select(i => STEM.Sys.IO.Path.GetFileName(i).ToUpper()).Distinct());

                List<string> existing = Directory.GetFiles(STEM.Sys.Serialization.VersionManager.VersionCache).Select(i => STEM.Sys.IO.Path.GetFileName(i)).ToList();

                unusedListBox1.Items.Clear();

                existing.Where(i => !needed.Contains(i.ToUpper())).ToList().ForEach(i => unusedListBox1.Items.Add(i, true));
                //needed.ToList().ForEach(i => unusedListBox1.Items.Add(i, true));

                countLabel.Text = "Count: " + unusedListBox1.Items.Count;

                moveToArchive.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK);
            }
        }
        
        private void moveToArchive_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (int i in unusedListBox1.CheckedIndices)
                    {
                        try
                        {
                            string file = unusedListBox1.Items[i] as string;
                            _UIActor.SendToAll(new ArchiveAssembly { Name = file });
                            unusedListBox1.Items.Remove(i);
                        }
                        catch { }
                    }

                moveToArchive.Enabled = false;
            }
            catch (Exception ex)
            {
                ExceptionViewer ev = new ExceptionViewer(ex);
                ev.ShowDialog(this);
            }
        }

        private void selectAll_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < unusedListBox1.Items.Count; x++)
                unusedListBox1.SetItemCheckState(x, CheckState.Checked);
        }

        private void deselectAll_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < unusedListBox1.Items.Count; x++)
                unusedListBox1.SetItemCheckState(x, CheckState.Unchecked);
        }

        private void deleteExtensionsFolder_Click(object sender, EventArgs e)
        {
            string s = extensionsFolders.SelectedText;

            if (!String.IsNullOrEmpty(s))
            {
                _UIActor.SendToAll(new DeleteExtensionsSubdirectory(s));

                extensionsFolders.Items.Remove(s);

                if (extensionsFolders.Items.Count == 0)
                {
                    extensionsFolders.SelectedText = null;
                    extensionsFolders.SelectedIndex = -1;
                }
                else
                {
                    extensionsFolders.SelectedIndex = 0;
                }
            }
        }
    }
}
