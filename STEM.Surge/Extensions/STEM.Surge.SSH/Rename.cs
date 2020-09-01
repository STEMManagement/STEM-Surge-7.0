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
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace STEM.Surge.SSH
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Rename")]
    [Description("Rename a file on an SSH server.")]
    public class Rename : STEM.Surge.Instruction
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
            ServerAddress = "[SshServerAddress]";
            Port = "[SshServerPort]";

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
                    PostMortemMetaData["LastOperation"] = "NextAddress";

                    string address = Authentication.NextAddress(ServerAddress);

                    if (address == null)
                    {
                        Exception ex = new Exception("No valid address. (" + ServerAddress + ")");
                        Exceptions.Add(ex);
                        AppendToMessage(ex.Message);
                        return false;
                    }

                    PostMortemMetaData["LastOperation"] = "FileExists:SourceFile";

                    if (!Authentication.FileExists(address, Int32.Parse(Port), SourceFile))
                        throw new System.IO.IOException("The target file does not exist: (" + SourceFile + ")");

                    string dst = NewFile;

                    PostMortemMetaData["LastOperation"] = "FileExists:DestinationFile";

                    if (Authentication.FileExists(address, Int32.Parse(Port), dst))
                        switch (FileExistsAction)
                        {
                            case STEM.Sys.IO.FileExistsAction.Skip:
                                return true;

                            case STEM.Sys.IO.FileExistsAction.Throw:
                                r = -1;
                                throw new System.IO.IOException("Destination file exists. (" + dst + ")");

                            case STEM.Sys.IO.FileExistsAction.Overwrite:
                            case STEM.Sys.IO.FileExistsAction.OverwriteIfNewer:
                                Authentication.DeleteFile(address, Int32.Parse(Port), dst);
                                break;

                            case STEM.Sys.IO.FileExistsAction.MakeUnique:
                                dst = Authentication.UniqueFilename(address, Int32.Parse(Port), dst);
                                break;
                        }

                    string directory = STEM.Sys.IO.Path.GetDirectoryName(dst);
                    
                    PostMortemMetaData["LastOperation"] = "DirectoryExists:DestinationDirectory";

                    if (!Authentication.DirectoryExists(address, Int32.Parse(Port), directory))
                        Authentication.CreateDirectory(address, Int32.Parse(Port), directory);

                    PostMortemMetaData["LastOperation"] = "RenameFile";

                    Authentication.RenameFile(address, Int32.Parse(Port), SourceFile, dst);

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
