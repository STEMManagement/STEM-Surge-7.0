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
using System.Linq;
using Renci.SshNet.Sftp;
using STEM.Listing.SSH;

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

        protected override bool _Run()
        {
            if (ExecutionMode == ExecuteOn.ForwardExecution)
            {
                int r = Retry;
                while (r-- >= 0 && !Stop)
                {
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
                if (InstructionSet.InstructionSetContainer.ContainsKey(Authentication.ConfigurationName + ".SshClientAddress"))
                    InstructionSet.InstructionSetContainer[Authentication.ConfigurationName + ".SshClientAddress"] = Authentication.TargetAddress((string)InstructionSet.InstructionSetContainer[Authentication.ConfigurationName + ".SshClientAddress"]);
                else
                    InstructionSet.InstructionSetContainer[Authentication.ConfigurationName + ".SshClientAddress"] = Authentication.TargetAddress(null);

                List<string> files = null;

                if (!ExpandSource && !RecurseSource && !FileFilter.Contains("|") && !FileFilter.Contains("!") && !FileFilter.Contains("<>") && !FileFilter.Contains("?") && !FileFilter.Contains("*"))
                {
                    files = new List<string>();
                    files.Add(Path.Combine(SourcePath, FileFilter));
                }
                else
                {
                    files = Authentication.ListDirectory(SourcePath, Sys.IO.Listing.ListingType.File, RecurseSource, DirectoryFilter, FileFilter).Select(i => Authentication.ToString(i)).ToList();
                }

                foreach (string file in files)
                {
                    try
                    {
                        Authentication.DeleteFile(file);
                        AppendToMessage(file + " deleted");
                    }
                    catch (Exception ex)
                    {
                        AppendToMessage(ex.ToString());
                        Exceptions.Add(ex);
                    }
                }

                if (DeleteEmptyDirectories)
                {
                    if (RecurseSource)
                    {
                        foreach (SftpFile dir in Authentication.ListDirectory(SourcePath, Sys.IO.Listing.ListingType.Directory, true, DirectoryFilter, "*"))
                        {
                            try
                            {
                                Authentication.DeleteDirectory(Authentication.ToString(dir), true, false);
                            }
                            catch { }
                        }
                    }

                    try
                    {
                        Authentication.DeleteDirectory(SourcePath, true, false);
                    }
                    catch { }
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

