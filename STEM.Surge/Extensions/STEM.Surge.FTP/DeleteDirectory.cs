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
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using FluentFTP;

namespace STEM.Surge.FTP
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Delete Directory")]
    [Description("Delete directories from 'Source Path' which can not be an expandable path.")]
    public class DeleteDirectory : Instruction
    {
        public enum ExecutoOn { ForwardExecution, Rollback }

        [Category("FTP Server")]
        [DisplayName("Authentication"), DescriptionAttribute("The authentication configuration to be used.")]
        public Authentication Authentication { get; set; }

        [Category("FTP Server")]
        [DisplayName("FTP Server Address"), DescriptionAttribute("What is the FTP Server Address?")]
        public string ServerAddress { get; set; }

        [Category("FTP Server")]
        [DisplayName("FTP Port"), DescriptionAttribute("What is the FTP Port?")]
        public string Port { get; set; }

        [Category("Retry")]
        [DisplayName("Number of retries"), DescriptionAttribute("How many times should each operation be attempted?")]
        public int Retry { get; set; }

        [Category("Retry")]
        [DisplayName("Seconds between retries"), DescriptionAttribute("How many seconds should we wait between retries?")]
        public int RetryDelaySeconds { get; set; }

        [Category("Source")]
        [DisplayName("Source Path"), DescriptionAttribute("The location of the directories to be deleted.")]
        public string SourcePath { get; set; }

        [Category("Source")]
        [DisplayName("Expand Source Path"), DescriptionAttribute("When searching for directories to delete, should the 'Source Path' be treated like an expandable?.")]
        public bool ExpandSource { get; set; }

        [Category("Source")]
        [DisplayName("Directory Filter"), DescriptionAttribute("The filter used on directories in 'Source Path' when 'Recurse Source' is specified. This can be a compound filter.")]
        public string DirectoryFilter { get; set; }

        [Category("Source")]
        [DisplayName("Recurse Source"), DescriptionAttribute("Recurse the source for directories to delete?")]
        public bool RecurseSource { get; set; }

        [Category("Source")]
        [DisplayName("Delete Empty Directories Only"), DescriptionAttribute("Should empty directories be deleted after filtered files are deleted?")]
        public bool DeleteEmptyDirectoriesOnly { get; set; }

        [Category("Flow")]
        [DisplayName("Execution Mode"), Description("Should this be executed on forward InstructionSet execution or on Rollback? Consider the use case where you want to " +
            "delete a working directory on Rollback.")]
        public ExecutoOn ExecutionMode { get; set; }

        public DeleteDirectory()
            : base()
        {
            Authentication = new Authentication();
            ServerAddress = "[FtpServerAddress]";
            Port = "[FtpServerPort]";

            Retry = 1;
            RetryDelaySeconds = 2;
            ExpandSource = false;
            SourcePath = "[TargetPath]";
            DirectoryFilter = "[TargetName]";
            DeleteEmptyDirectoriesOnly = true;
            RecurseSource = false;
            ExecutionMode = ExecutoOn.ForwardExecution;
        }

        string _Address = null;

        protected override bool _Run()
        {
            if (ExecutionMode == ExecutoOn.ForwardExecution)
            {
                int r = Retry;
                while (r-- >= 0)
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
            if (ExecutionMode == ExecutoOn.Rollback)
            {
                int r = Retry;
                while (r-- >= 0)
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
                FtpClient conn = Authentication.OpenClient(_Address, Int32.Parse(Port));

                try
                {
                    List<FtpListItem> items = Authentication.ListDirectory(_Address, Int32.Parse(Port), SourcePath, FTPListType.Directory, RecurseSource, DirectoryFilter, "*");

                    foreach (FtpListItem i in items)
                    {
                        try
                        {
                            List<FtpListItem> remaining = Authentication.ListDirectory(_Address, Int32.Parse(Port), i.FullName, FTPListType.All, true, "*", "*");

                            if (DeleteEmptyDirectoriesOnly)
                            {
                                if (remaining.Count == 0)
                                {
                                    conn.DeleteDirectory(i.FullName);
                                    AppendToMessage(i.FullName + " deleted");
                                }
                            }
                            else
                            {
                                foreach (FtpListItem del in remaining)
                                {
                                    if (del.Type == FtpFileSystemObjectType.File)
                                        conn.DeleteFile(del.FullName);
                                }

                                foreach (FtpListItem del in remaining)
                                {
                                    if (del.Type == FtpFileSystemObjectType.Directory)
                                        if (conn.DirectoryExists(del.FullName))
                                            conn.DeleteDirectory(del.FullName);
                                }

                                conn.DeleteDirectory(i.FullName);
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
                finally
                {
                    Authentication.RecycleClient(conn);
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

