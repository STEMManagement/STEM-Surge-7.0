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
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.ComponentModel;
using STEM.Surge;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace STEM.Surge.SSH
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("File Fisher")]
    [Description("Intended to run in a static InstructionSet in continuous mode with a generous interval (> 5 min), this Instruction watches a folder for orphaned files.")]
    public class FileFisher : Instruction
    {
        [Category("SSH Server")]
        [DisplayName("Authentication"), DescriptionAttribute("The authentication configuration to be used.")]
        public Authentication Authentication { get; set; }

        [Category("SSH Server")]
        [DisplayName("SSH Server Address"), DescriptionAttribute("What is the SSH Server Address?")]
        public string ServerAddress { get; set; }

        [Category("SSH Server")]
        [DisplayName("SSH Port"), DescriptionAttribute("What is the SSH Port?")]
        public string Port { get; set; }

        [Category("File Fisher")]
        [DisplayName("File Presence (Minutes)")]
        [DescriptionAttribute("The time (in minutes) that a file has been present in the folder before it should be fished into errors.")]
        public int FilePresenceMinutes { get; set; }

        [Category("File Fisher")]
        [DisplayName("Target Path")]
        [DescriptionAttribute("The location of the file(s) to be watched.")]
        public string TargetPath { get; set; }

        [Category("File Fisher")]
        [DisplayName("File Filter")]
        [DescriptionAttribute("The file filter to apply to the Target Path.")]
        public string FileFilter { get; set; }

        [Category("File Fisher")]
        [DisplayName("Directory Filter")]
        [DescriptionAttribute("The directory filter to apply to the Target Path (applies if recurse is true).")]
        public string DirectoryFilter { get; set; }

        [Category("File Fisher")]
        [DisplayName("Recurse Target Path")]
        [DescriptionAttribute("If recurse is true, the subdirectories will be recreated under the error folder as files are fished.")]
        public bool Recurse { get; set; }

        [Category("File Fisher")]
        [DisplayName("Error Path")]
        [DescriptionAttribute("The error folder in which to move fished files.")]
        public string ErrorPath { get; set; }
        
        public FileFisher() : base()
        {
            Authentication = new Authentication();
            ServerAddress = "[SshServerAddress]";
            Port = "[SshServerPort]";

            TargetPath = "[TargetPath]";
            FileFilter = "*";
            DirectoryFilter = "*";
            Recurse = false;
            ErrorPath = "[Errors]";
            FilePresenceMinutes = 1440;
        }

        protected override bool _Run()
        {
            try
            {
                STEM.Sys.Serialization.Dictionary<string, DateTime> dict = null;

                string key = TargetPath + ":" + FileFilter + ":" + DirectoryFilter + ".fisher";

                if (!STEM.Sys.Global.Cache.ContainsKey(key))
                    STEM.Sys.Global.Cache[key] = new STEM.Sys.Serialization.Dictionary<string, DateTime>();

                dict = STEM.Sys.Global.Cache[key] as STEM.Sys.Serialization.Dictionary<string, DateTime>;

                if (dict == null)
                {
                    STEM.Sys.Global.Cache[key] = new STEM.Sys.Serialization.Dictionary<string, DateTime>();
                    dict = STEM.Sys.Global.Cache[key] as STEM.Sys.Serialization.Dictionary<string, DateTime>;
                }

                if (dict == null)
                    throw new Exception("There is an issue with the fisher cache for " + key + ". Fisher will not run.");

                TargetPath = STEM.Sys.IO.Path.AdjustPath(TargetPath);
                ErrorPath = STEM.Sys.IO.Path.AdjustPath(ErrorPath);

                bool modified = false;

                DateTime epoch = DateTime.UtcNow;

                List<SftpFile> files = Authentication.ListDirectory(ServerAddress, Int32.Parse(Port), TargetPath, SSHListType.File, Recurse, DirectoryFilter, FileFilter);

                lock (dict)
                {
                    foreach (string file in dict.Keys)
                    {
                        SftpFile f = files.FirstOrDefault(i => i.FullName == file);

                        if (f != null)
                            files.Remove(f);
                    }

                    foreach (string file in dict.Keys.ToList())
                    {
                        try
                        {
                            if (!Authentication.FileExists(ServerAddress, Int32.Parse(Port), file))
                            {
                                dict.Remove(file);
                                modified = true;
                                continue;
                            }

                            if ((epoch - dict[file]).TotalMinutes > FilePresenceMinutes)
                            {
                                string errDir = ErrorPath;

                                if (Recurse)
                                    errDir = Path.Combine(ErrorPath, Path.GetDirectoryName(file.Replace(STEM.Sys.IO.Path.FirstTokenOfPath(file), STEM.Sys.IO.Path.FirstTokenOfPath(TargetPath))).Substring(TargetPath.Length).Trim(Path.DirectorySeparatorChar));

                                if (!Directory.Exists(errDir))
                                    Directory.CreateDirectory(errDir);

                                if (errDir.ToLower().Contains(STEM.Sys.IO.Path.AdjustPath("/dev/null")))
                                {
                                    Authentication.DeleteFile(ServerAddress, Int32.Parse(Port), Authentication.AdjustPath(ServerAddress, file));
                                }
                                else
                                {
                                    SftpClient client = Authentication.OpenSftpClient(ServerAddress, Int32.Parse(Port));

                                    if (client != null)
                                        try
                                        {
                                            string errFile = STEM.Sys.IO.Path.AdjustPath(Path.Combine(errDir, Path.GetFileName(file)));

                                            try
                                            {
                                                using (System.IO.Stream dStream = System.IO.File.Open(errFile, System.IO.FileMode.CreateNew, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None))
                                                {
                                                    client.DownloadFile(Authentication.AdjustPath(ServerAddress, file), dStream);
                                                }
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    if (File.Exists(errFile))
                                                        File.Delete(errFile);
                                                }
                                                catch { }

                                                throw;
                                            }

                                            Authentication.DeleteFile(ServerAddress, Int32.Parse(Port), Authentication.AdjustPath(ServerAddress, file));
                                        }
                                        finally
                                        {
                                            Authentication.RecycleClient(client);
                                        }
                                }
                             
                                dict.Remove(file);
                                modified = true;
                            }
                        }
                        catch { }
                    }

                    if (files.Count() > 0)
                    {
                        foreach (Renci.SshNet.Sftp.SftpFile file in files)
                            dict[file.FullName] = epoch;

                        modified = true;
                    }

                    if (modified)
                        STEM.Sys.Global.Cache[key] = dict;
                }
            }
            catch (Exception ex)
            {
                AppendToMessage(ex.ToString());
                Exceptions.Add(ex);
            }

            return Exceptions.Count == 0;
        }

        protected override void _Rollback()
        {
        }
    }
}
