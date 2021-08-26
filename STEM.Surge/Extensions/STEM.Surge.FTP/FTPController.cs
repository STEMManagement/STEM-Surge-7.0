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
using FluentFTP;

namespace STEM.Surge.FTP
{
    [Serializable()]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("FTPController")]
    [Description("Derived from Basic File Controller... Poll a source on an FTP server. The Switchboard configuration for Source should look like: " +
        "'\\\\10.0.0.80\\FtpRoot\\FtpSubdir' where the address can be expandable and identifies the FTP server(s) to poll, " +
        "'FtpRoot' is the name of the root folder, and 'FtpSubdir(s)' can be specified. The 'File Filter', 'Directory Filter', and 'Recurse' " +
        "settings in the Switchboard are also applied to the poll, and 'Pingable Source' is honored such that a successful ping would " +
        "be required before a connect. This controller provides '[FtpServerAddress]' and '[FtpServerPort]' for template use.")]
    public class FTPController : STEM.Surge.BasicControllers.BasicFileController
    {
        [Category("FTP Server")]
        [DisplayName("Authentication"), DescriptionAttribute("The authentication configuration to be used.")]
        public Authentication Authentication { get; set; }
        
        [Category("FTP Server")]
        [DisplayName("FTP Port"), DescriptionAttribute("What is the FTP Port?")]
        public int Port { get; set; }

        [Category("FTP Server")]
        [DisplayName("List Type"), DescriptionAttribute("Are you assigning files or directories?")]
        public FTPListType ListType { get; set; }
        
        public FTPController()
        {
            Authentication = new Authentication();

            PreprocessPerformsDiscovery = true;

            Port = 21;

            TemplateKVP["[FtpServerAddress]"] = "Reserved";
            TemplateKVP["[FtpServerPort]"] = "Reserved";
        }

        Dictionary<string, FtpListItem> _ListItems = new Dictionary<string, FtpListItem>();
                
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

                List<FtpListItem> items = Authentication.ListDirectory(server, Port, directory, ListType, PollerRecurseSetting, PollerDirectoryFilter, PollerFileFilter);
                
                _ListItems = items.ToDictionary(i => System.IO.Path.Combine(PollerSourceString, i.FullName.Substring(directory.Length).Trim(System.IO.Path.DirectorySeparatorChar).Trim(System.IO.Path.AltDirectorySeparatorChar)).Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar), j => j);
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
                STEM.Sys.EventLog.WriteEntry("FTPController.ListPreprocess", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            return returnList;
        }

        public override DeploymentDetails GenerateDeploymentDetails(IReadOnlyList<string> listPreprocessResult, string initiationSource, string recommendedBranchIP, IReadOnlyList<string> limitedToBranches)
        {
            try
            {
                TemplateKVP["[FtpServerAddress]"] = STEM.Sys.IO.Path.IPFromPath(PollerSourceString);
                TemplateKVP["[FtpServerPort]"] = Port.ToString();

                DeploymentDetails dd = base.GenerateDeploymentDetails(listPreprocessResult, initiationSource, recommendedBranchIP, limitedToBranches);

                if (dd != null)
                {
                    foreach (Instruction ins in dd.ISet.Instructions)
                    {
                        foreach (PropertyInfo prop in ins.GetType().GetProperties().Where(p => p.PropertyType.IsSubclassOf(typeof(IAuthentication))))
                        {
                            IAuthentication a = prop.GetValue(ins) as IAuthentication;

                            if (a.VersionDescriptor.TypeName == "STEM.Surge.FTP.Authentication")
                            {
                                PropertyInfo i = a.GetType().GetProperties().FirstOrDefault(p => p.Name == "FTPPassword");
                                if (i != null)
                                {
                                    string k = i.GetValue(a) as string;
                                    if (String.IsNullOrEmpty(k))
                                    {
                                        i.SetValue(a, Authentication.FTPPassword);

                                        i = a.GetType().GetProperties().FirstOrDefault(p => p.Name == "FTPUser");
                                        if (i != null)
                                            i.SetValue(a, Authentication.FTPUser);

                                        i = a.GetType().GetProperties().FirstOrDefault(p => p.Name == "RequireCertificateValidation");
                                        if (i != null)
                                            i.SetValue(a, Authentication.RequireCertificateValidation);

                                        i = a.GetType().GetProperties().FirstOrDefault(p => p.Name == "UseTLS");
                                        if (i != null)
                                            i.SetValue(a, Authentication.UseTLS);
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
                TemplateKVP["[FtpServerAddress]"] = "Reserved";
                TemplateKVP["[FtpServerPort]"] = "Reserved";
            }
        }

        public override bool FileExists(string file, string user, string password, bool isLocal)
        {
            return FileExists(file);
        }

        public override bool FileExists(string file)
        {
            try
            {
                string server = STEM.Sys.IO.Path.IPFromPath(file);

                if (server == System.Net.IPAddress.None.ToString())
                    server = STEM.Sys.IO.Path.IPFromPath(PollerSourceString);

                file = Authentication.AdjustPath(server, file);

                FtpClient conn = Authentication.OpenClient(server, Port);

                try
                {
                    return conn.FileExists(file);
                }
                finally
                {
                    Authentication.RecycleClient(conn);
                }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("FTPController.FileExists", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            return false;
        }

        public override DateTime GetAgeBasis(string initiationSource)
        {
            if (_ListItems.ContainsKey(initiationSource))
            {
                switch (SelectedOrigin)
                {
                    case STEM.Surge.AgeOrigin.LastWriteTime:
                        return _ListItems[initiationSource].Modified;

                    case STEM.Surge.AgeOrigin.LastAccessTime:
                        return _ListItems[initiationSource].Modified;

                    case STEM.Surge.AgeOrigin.CreationTime:
                        return _ListItems[initiationSource].Created;
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
                    CreationTimeUtc = _ListItems[file].Created,
                    LastAccessTimeUtc = _ListItems[file].Modified,
                    LastWriteTimeUtc = _ListItems[file].Modified,
                    Size = _ListItems[file].Size
                };

            return null;
        }

        public override bool DirectoryExists(string directory, string user, string password, bool isLocal)
        {
            return DirectoryExists(directory);
        }

        public override bool DirectoryExists(string directory)
        {
            try
            {
                string server = STEM.Sys.IO.Path.IPFromPath(directory);

                if (server == System.Net.IPAddress.None.ToString())
                    server = STEM.Sys.IO.Path.IPFromPath(PollerSourceString);

                directory = Authentication.AdjustPath(server, directory);
                
                FtpClient conn = Authentication.OpenClient(server, Port);

                try
                {
                    return conn.DirectoryExists(directory);
                }
                finally
                {
                    Authentication.RecycleClient(conn);
                }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("FTPController.DirectoryExists", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            return false;
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
                    CreationTimeUtc = _ListItems[directory].Created,
                    LastAccessTimeUtc = _ListItems[directory].Modified,
                    LastWriteTimeUtc = _ListItems[directory].Modified
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

                directory = Authentication.AdjustPath(server, directory);

                FtpClient conn = Authentication.OpenClient(server, Port);

                try
                {
                    conn.CreateDirectory(directory, true);
                }
                finally
                {
                    Authentication.RecycleClient(conn);
                }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("FTPController.CreateDirectory", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
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
