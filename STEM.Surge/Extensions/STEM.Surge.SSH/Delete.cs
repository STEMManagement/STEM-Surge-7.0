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
using System.ComponentModel;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace STEM.Surge.SSH
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Delete")]
    [Description("Delete file(s) from 'Source Path' which can not be an expandable path.")]
    public class Delete : Instruction
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

        [Category("Retry")]
        [DisplayName("Number of retries"), DescriptionAttribute("How many times should each operation be attempted?")]
        public int Retry { get; set; }

        [Category("Retry")]
        [DisplayName("Seconds between retries"), DescriptionAttribute("How many seconds should we wait between retries?")]
        public int RetryDelaySeconds { get; set; }

        [Category("Source")]
        [DisplayName("Source Path"), DescriptionAttribute("The location of the file(s) to be deleted.")]
        public string SourcePath { get; set; }

        [Category("Source")]
        [DisplayName("Expand Source Path"), DescriptionAttribute("When searching for files to delete, should the 'Source Path' be treated like an expandable?.")]
        public bool ExpandSource { get; set; }

        [Category("Source")]
        [DisplayName("File Filter"), DescriptionAttribute("The filter used on files in 'Source Path' to select files to delete. This can be a compound filter.")]
        public string FileFilter { get; set; }

        [Category("Source")]
        [DisplayName("Directory Filter"), DescriptionAttribute("The filter used on directories in 'Source Path' when 'Recurse Source' is specified. This can be a compound filter.")]
        public string DirectoryFilter { get; set; }

        [Category("Source")]
        [DisplayName("Recurse Source"), DescriptionAttribute("Recurse the source for files to delete?")]
        public bool RecurseSource { get; set; }

        [Category("Source")]
        [DisplayName("Delete Empty Directories"), DescriptionAttribute("Should empty directories be deleted after filtered files are deleted?")]
        public bool DeleteEmptyDirectories { get; set; }

        [Category("Flow")]
        [DisplayName("Execution Mode"), Description("Should this be executed on forward InstructionSet execution or on Rollback? Consider the use case where you want to " +
            "move a file out of the flow to an error folder on Rollback.")]
        public ExecuteOn ExecutionMode { get; set; }

        public Delete()
            : base()
        {
            Authentication = new Authentication();
            ServerAddress = "[SshServerAddress]";
            Port = "[SshServerPort]";

            Retry = 1;
            RetryDelaySeconds = 2;
            ExpandSource = false;
            SourcePath = "[TargetPath]";
            FileFilter = "[TargetName]";
            DirectoryFilter = "!TEMP";
            DeleteEmptyDirectories = false;
            RecurseSource = false;
            ExecutionMode = ExecuteOn.ForwardExecution;
        }

        string _Address = null;

        protected override bool _Run()
        {
            if (ExecutionMode == ExecuteOn.ForwardExecution)
            {
                int r = Retry;
                while (r-- >= 0 && !Stop)
                {
                    _Address = Authentication.NextAddress(ServerAddress);

                    if (_Address == null)
                    {
                        Exception ex = new Exception("No valid address. (" + ServerAddress + ")");
                        Exceptions.Add(ex);
                        AppendToMessage(ex.Message);
                        return false;
                    }

                    Exceptions.Clear();
                    Message = "";
                    bool success = Execute();
                    if (success)
                        return true;

                    System.Threading.Thread.Sleep(RetryDelaySeconds * 1000);
                }

                return false;
            }

            return true;
        }

        protected override void _Rollback()
        {
            if (ExecutionMode == ExecuteOn.Rollback)
            {
                int r = Retry;
                while (r-- >= 0 && !Stop)
                {
                    _Address = Authentication.NextAddress(ServerAddress);

                    if (_Address == null)
                    {
                        Exception ex = new Exception("No valid address. (" + ServerAddress + ")");
                        Exceptions.Add(ex);
                        AppendToMessage(ex.Message);
                        return;
                    }

                    Exceptions.Clear();
                    Message = "";
                    bool success = Execute();
                    if (success)
                        return;

                    System.Threading.Thread.Sleep(RetryDelaySeconds * 1000);
                }
            }
        }

        bool Execute()
        {
            try
            {
                PostMortemMetaData["LastOperation"] = "ListDirectory";

                List<SftpFile> items = Authentication.ListDirectory(_Address, Int32.Parse(Port), SourcePath, SSHListType.All, RecurseSource, DirectoryFilter, FileFilter);

                foreach (SftpFile i in items)
                {
                    if (i.IsRegularFile)
                    {
                        try
                        {
                            PostMortemMetaData["LastOperation"] = "DeleteFile";
                            Authentication.DeleteFile(_Address, Int32.Parse(Port), i.FullName);
                            AppendToMessage(i.FullName + " deleted");
                        }
                        catch (Exception ex)
                        {
                            AppendToMessage(ex.ToString());
                            Exceptions.Add(ex);
                        }
                    }
                }

                if (DeleteEmptyDirectories)
                {
                    List<SftpFile> remaining;

                    if (RecurseSource)
                    {
                        foreach (SftpFile i in items)
                        {
                            if (i.IsDirectory)
                            {
                                try
                                {
                                    PostMortemMetaData["LastOperation"] = "ListDirectory";
                                    remaining = Authentication.ListDirectory(_Address, Int32.Parse(Port),
                                                                       i.FullName, SSHListType.All, false, "*", "*");

                                    if (remaining.Count == 0)
                                    {
                                        PostMortemMetaData["LastOperation"] = "DeleteDirectory";
                                        Authentication.DeleteDirectory(_Address, Int32.Parse(Port), i.FullName);
                                        AppendToMessage(i.FullName + " deleted");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    AppendToMessage(ex.ToString());
                                    Exceptions.Add(ex);
                                }
                            }
                        }
                    }

                    PostMortemMetaData["LastOperation"] = "ListDirectory";
                    remaining = Authentication.ListDirectory(_Address, Int32.Parse(Port),
                                                       SourcePath, SSHListType.All, false, "*", "*");

                    if (remaining.Count == 0)
                    {
                        PostMortemMetaData["LastOperation"] = "DeleteDirectory";
                        Authentication.DeleteDirectory(_Address, Int32.Parse(Port), SourcePath);
                        AppendToMessage(SourcePath + " deleted");
                    }
                }
            }
            catch (Exception ex)
            {
                AppendToMessage(ex.ToString());
                Exceptions.Add(ex);
            }

            return (Exceptions.Count == 0);
        }
    }
}

