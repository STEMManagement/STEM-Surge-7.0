/*
 * Copyright 2019 STEM Management
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace STEM.Surge.SSH
{
    [Serializable()]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("SFTPController")]
    [Description("Derived from Basic File Controller... Poll a source on an SFTP server. The Switchboard configuration for Source should look like: " +
        "'\\\\10.0.0.80\\SftpRoot\\SftpSubdir' where the address can be expandable and identifies the SFTP server(s) to poll, " +
        "'SftpRoot' is the name of the root folder, and 'SftpSubdir(s)' can be specified. The 'File Filter', 'Directory Filter', and 'Recurse' " +
        "settings in the Switchboard are also applied to the poll, and 'Pingable Source' is honored such that a successful ping would " +
        "be required before a connect. This controller provides '[SshServerAddress]' and '[SshServerPort]' for template use.")]
    public class SftpController : STEM.Surge.BasicControllers.BasicFileController
    {
        [Category("SFTP Server")]
        [DisplayName("Authentication"), DescriptionAttribute("The authentication configuration to be used.")]
        public Authentication Authentication { get; set; }
        
        [Category("SFTP Server")]
        [DisplayName("SFTP Port"), DescriptionAttribute("What is the SFTP Port?")]
        public int Port { get; set; }

        [Category("SFTP Server")]
        [DisplayName("List Type"), DescriptionAttribute("Are you assigning files or directories?")]
        public SSHListType ListType { get; set; }
        
        public SftpController()
        {
            Authentication = new Authentication();

            PreprocessPerformsDiscovery = true;

            Port = 21;

            TemplateKVP["[SshServerAddress]"] = "Reserved";
            TemplateKVP["[SshServerPort]"] = "Reserved";
        }

        Dictionary<string, SftpFile> _ListItems = new Dictionary<string, SftpFile>();
                
        public override List<string> ListPreprocess(IReadOnlyList<string> list)
        {
            string server = "";
            List<string> returnList = new List<string>();
            try
            {
                server = STEM.Sys.IO.Path.IPFromPath(PollerSourceString);

                if (PingableSourceSetting)
                {
                    if (!STEM.Sys.IO.Net.PingHost(server))
                    {
                        PollError = "(" + STEM.Sys.IO.Net.MachineIP() + ") Failed to ping " + server;
                        return returnList;
                    }
                }

                string directory = Authentication.AdjustPath(server, PollerSourceString);

                List<SftpFile> items = Authentication.ListDirectory(server, Port, directory, ListType, PollerRecurseSetting, PollerDirectoryFilter, PollerFileFilter);

                _ListItems = items.ToDictionary(i => System.IO.Path.Combine(PollerSourceString, i.FullName.Substring(directory.Length + i.FullName.IndexOf(directory, StringComparison.InvariantCultureIgnoreCase)).Trim(System.IO.Path.DirectorySeparatorChar).Trim(System.IO.Path.AltDirectorySeparatorChar)).Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar), j => j);
                returnList = _ListItems.Keys.ToList();

                if (RandomizeList)
                {
                    Random rnd = new Random();
                    returnList = returnList.OrderBy(i => rnd.Next()).ToList();
                }
                else
                {
                    returnList.Sort();
                }

                if (HonorPriorityFilters)
                    returnList = ApplyPriorityFilterOrdering(returnList);

                PollError = "";
            }
            catch (Exception ex)
            {
                PollError = "(" + STEM.Sys.IO.Net.MachineIP() + ") encountered an error connecting to " + server + ": " + ex.Message;
                STEM.Sys.EventLog.WriteEntry("SftpController.ListPreprocess", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            return returnList;
        }

        public override DeploymentDetails GenerateDeploymentDetails(IReadOnlyList<string> listPreprocessResult, string initiationSource, string recommendedBranchIP, IReadOnlyList<string> limitedToBranches)
        {
            try
            {
                TemplateKVP["[SshServerAddress]"] = STEM.Sys.IO.Path.IPFromPath(PollerSourceString);
                TemplateKVP["[SshServerPort]"] = Port.ToString();

                DeploymentDetails dd = base.GenerateDeploymentDetails(listPreprocessResult, initiationSource, recommendedBranchIP, limitedToBranches);

                if (dd != null)
                {
                    foreach (Instruction ins in dd.ISet.Instructions)
                    {
                        foreach (PropertyInfo prop in ins.GetType().GetProperties().Where(p => p.PropertyType.IsSubclassOf(typeof(IAuthentication))))
                        {
                            IAuthentication a = prop.GetValue(ins) as IAuthentication;

                            if (a.VersionDescriptor.TypeName == "STEM.Surge.SSH.Authentication")
                            {
                                PropertyInfo i = a.GetType().GetProperties().FirstOrDefault(p => p.Name == "Password");
                                if (i != null)
                                {
                                    string k = i.GetValue(a) as string;
                                    if (String.IsNullOrEmpty(k))
                                    {
                                        i.SetValue(a, Authentication.Password);

                                        i = a.GetType().GetProperties().FirstOrDefault(p => p.Name == "User");
                                        if (i != null)
                                            i.SetValue(a, Authentication.User);

                                        i = a.GetType().GetProperties().FirstOrDefault(p => p.Name == "KeyFile");
                                        if (i != null)
                                            i.SetValue(a, Authentication.KeyFile);
                                    }
                                }
                            }
                        }
                    }
                }

                return dd;
            }
            finally
            {
                TemplateKVP["[SshServerAddress]"] = "Reserved";
                TemplateKVP["[SshServerPort]"] = "Reserved";
            }
        }

        public override bool FileExists(string file, string user, string password, bool isLocal)
        {
            return FileExists(file);
        }

        public override bool FileExists(string file)
        {
            string server = STEM.Sys.IO.Path.IPFromPath(file);

            if (server == System.Net.IPAddress.None.ToString())
                server = STEM.Sys.IO.Path.IPFromPath(PollerSourceString);
            
            return Authentication.FileExists(server, Port, file);
        }

        public override DateTime GetAgeBasis(string initiationSource)
        {
            if (_ListItems.ContainsKey(initiationSource))
            {
                switch (SelectedOrigin)
                {
                    case STEM.Surge.AgeOrigin.LastWriteTime:
                        return _ListItems[initiationSource].LastWriteTimeUtc;

                    case STEM.Surge.AgeOrigin.LastAccessTime:
                        return _ListItems[initiationSource].LastAccessTimeUtc;

                    case STEM.Surge.AgeOrigin.CreationTime:
                        return _ListItems[initiationSource].LastWriteTimeUtc;
                }
            }

            return DateTime.MinValue;
        }

        public override FDCFileInfo GetFileInfo(string file, string user, string password, bool isLocal)
        {
            return GetFileInfo(file);
        }

        public override FDCFileInfo GetFileInfo(string file)
        {
            if (_ListItems.ContainsKey(file))
                return new FDCFileInfo
                {
                    CreationTimeUtc = _ListItems[file].LastWriteTimeUtc,
                    LastAccessTimeUtc = _ListItems[file].LastAccessTimeUtc,
                    LastWriteTimeUtc = _ListItems[file].LastWriteTimeUtc,
                    Size = _ListItems[file].Attributes.Size
                };

            return null;
        }

        public override bool DirectoryExists(string directory, string user, string password, bool isLocal)
        {
            return DirectoryExists(directory);
        }

        public override bool DirectoryExists(string directory)
        {
            string server = STEM.Sys.IO.Path.IPFromPath(directory);

            if (server == System.Net.IPAddress.None.ToString())
                server = STEM.Sys.IO.Path.IPFromPath(PollerSourceString);
            
            return Authentication.DirectoryExists(server, Port, directory);
        }

        public override FDCDirectoryInfo GetDirectoryInfo(string directory, string user, string password, bool isLocal)
        {
            return GetDirectoryInfo(directory);
        }

        public override FDCDirectoryInfo GetDirectoryInfo(string directory)
        {
            if (_ListItems.ContainsKey(directory))
                return new FDCDirectoryInfo
                {
                    CreationTimeUtc = _ListItems[directory].LastWriteTimeUtc,
                    LastAccessTimeUtc = _ListItems[directory].LastAccessTimeUtc,
                    LastWriteTimeUtc = _ListItems[directory].LastWriteTimeUtc
                };

            return null;
        }

        public override void CreateDirectory(string directory, string user, string password, bool isLocal)
        {
            CreateDirectory(directory);
        }

        public override void CreateDirectory(string directory)
        {
            try
            {
                string server = STEM.Sys.IO.Path.IPFromPath(directory);

                if (server == System.Net.IPAddress.None.ToString())
                    server = STEM.Sys.IO.Path.IPFromPath(PollerSourceString);
                                
                Authentication.DirectoryExists(server, Port, directory);
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("SftpController.CreateDirectory", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                throw ex;
            }
        }

        protected override void Dispose(bool dispose)
        {
            base.Dispose(dispose);
            Authentication.Dispose();
        }
    }
}
