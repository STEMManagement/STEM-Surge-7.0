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
using FluentFTP;
using STEM.Listing.FTP;

namespace STEM.Surge.FTP
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Rename")]
    [Description("Rename a file on an FTP server.")]
    public class Rename : STEM.Surge.Instruction
    {
        [Category("FTP Server")]
        [DisplayName("Authentication"), DescriptionAttribute("The authentication configuration to be used.")]
        public Authentication Authentication { get; set; }

        [Category("Target")]
        [DisplayName("Source File"), DescriptionAttribute("The full path of the file to be renamed.")]
        public string SourceFile { get; set; }

        [Category("Target")]
        [DisplayName("New File"), DescriptionAttribute("The full path of the new file.")]
        public string NewFile { get; set; }

        [DisplayName("File Exists Action")]
        [Description("What action should be taken if the Destination File already exists?")]
        public STEM.Sys.IO.FileExistsAction FileExistsAction { get; set; }

        [Category("Retry")]
        [DisplayName("Number of retries"), DescriptionAttribute("How many times should each operation be attempted?")]
        public int Retry { get; set; }

        [Category("Retry")]
        [DisplayName("Seconds between retries"), DescriptionAttribute("How many seconds should we wait between retries?")]
        public int RetryDelaySeconds { get; set; }

        public Rename()
        {
            Authentication = new Authentication();

            SourceFile = "[TargetPath]\\[TargetName]";
            NewFile = "[TargetPath]\\NewFileName.txt";

            FileExistsAction = STEM.Sys.IO.FileExistsAction.MakeUnique;

            Retry = 1;
            RetryDelaySeconds = 2;
        }

        protected override void _Rollback()
        {
        }

        protected override bool _Run()
        {
            int r = Retry;

            while (r-- >= 0 && !Stop)
                try
                {
                    if (InstructionSet.InstructionSetContainer.ContainsKey(Authentication.ConfigurationName + ".FtpClientAddress"))
                        InstructionSet.InstructionSetContainer[Authentication.ConfigurationName + ".FtpClientAddress"] = Authentication.TargetAddress((string)InstructionSet.InstructionSetContainer[Authentication.ConfigurationName + ".FtpClientAddress"]);
                    else
                        InstructionSet.InstructionSetContainer[Authentication.ConfigurationName + ".FtpClientAddress"] = Authentication.TargetAddress(null);

                    if (!Authentication.FileExists(SourceFile))
                        throw new System.IO.IOException("The target file does not exist: (" + SourceFile + ")");

                    string dst = NewFile;

                    if (Authentication.FileExists(dst))
                        switch (FileExistsAction)
                        {
                            case STEM.Sys.IO.FileExistsAction.Skip:
                                return true;

                            case STEM.Sys.IO.FileExistsAction.Throw:
                                r = -1;
                                throw new System.IO.IOException("Destination file exists. (" + dst + ")");

                            case STEM.Sys.IO.FileExistsAction.Overwrite:
                            case STEM.Sys.IO.FileExistsAction.OverwriteIfNewer:
                                Authentication.DeleteFile(dst);
                                break;

                            case STEM.Sys.IO.FileExistsAction.MakeUnique:
                                dst = Authentication.UniqueFilename(dst);
                                break;
                        }

                    string directory = STEM.Sys.IO.Path.GetDirectoryName(dst);

                    if (!Authentication.DirectoryExists(directory))
                        Authentication.CreateDirectory(directory);

                    Authentication.RenameFile(SourceFile, dst);

                    AppendToMessage(SourceFile + " renamed to " + dst);

                    break;
                }
                catch (Exception ex)
                {
                    if (r < 0)
                    {
                        AppendToMessage(ex.Message);
                        Exceptions.Add(ex);
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(RetryDelaySeconds * 1000);
                    }
                }

            return Exceptions.Count == 0;
        }
    }
}
