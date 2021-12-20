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
using STEM.Listing.FTP;
using FluentFTP;

namespace STEM.Surge.FTP
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Container To File")]
    [Description("Write data from a container to a file on the FTP server.")]
    public class ContainerToFile : STEM.Surge.Instruction
    {
        [Category("FTP Server")]
        [DisplayName("Authentication"), DescriptionAttribute("The authentication configuration to be used.")]
        public Authentication Authentication { get; set; }

        [DisplayName("Destination File")]
        [Description("The file to which the data is to be saved.")]
        public string DestinationFile { get; set; }

        [DisplayName("Container Data Key")]
        [Description("The key in the container where the file data is loaded.")]
        public string ContainerDataKey { get; set; }

        [DisplayName("Target Container")]
        [Description("The container with the data to be saved.")]
        public ContainerType TargetContainer { get; set; }
        
        [DisplayName("File Exists Action")]
        [Description("What action should be taken if the Destination File already exists?")]
        public STEM.Sys.IO.FileExistsAction FileExistsAction { get; set; }

        [DisplayName("Create Empty Files")]
        [Description("Should an empty file be created if the file data in the container is empty?")]
        public bool CreateEmptyFiles { get; set; }

        [Category("Retry")]
        [DisplayName("Number of retries"), DescriptionAttribute("How many times should each operation be attempted?")]
        public int Retry { get; set; }

        [Category("Retry")]
        [DisplayName("Seconds between retries"), DescriptionAttribute("How many seconds should we wait between retries?")]
        public int RetryDelaySeconds { get; set; }

        public ContainerToFile()
        {
            Authentication = new Authentication();

            DestinationFile = "[DestinationPath]\\[TargetName]";
            ContainerDataKey = "[TargetNameWithoutExt]";
            TargetContainer = ContainerType.InstructionSetContainer;
            FileExistsAction = STEM.Sys.IO.FileExistsAction.MakeUnique;
            CreateEmptyFiles = false;

            Retry = 1;
            RetryDelaySeconds = 2;
        }

        protected override void _Rollback()
        {
            int r = Retry;

            while (r-- >= 0)
                if (_SavedFile != null)
                    try
                    {
                        if (InstructionSet.InstructionSetContainer.ContainsKey(Authentication.ConfigurationName + ".FtpClientAddress"))
                            InstructionSet.InstructionSetContainer[Authentication.ConfigurationName + ".FtpClientAddress"] = Authentication.TargetAddress((string)InstructionSet.InstructionSetContainer[Authentication.ConfigurationName + ".FtpClientAddress"]);
                        else
                            InstructionSet.InstructionSetContainer[Authentication.ConfigurationName + ".FtpClientAddress"] = Authentication.TargetAddress(null);

                        Authentication.DeleteFile(_SavedFile);

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
        }

        string _SavedFile = null;

        protected override bool _Run()
        {
            int r = Retry;

            while (r-- >= 0)
                try
                {
                    if (InstructionSet.InstructionSetContainer.ContainsKey(Authentication.ConfigurationName + ".FtpClientAddress"))
                        InstructionSet.InstructionSetContainer[Authentication.ConfigurationName + ".FtpClientAddress"] = Authentication.TargetAddress((string)InstructionSet.InstructionSetContainer[Authentication.ConfigurationName + ".FtpClientAddress"]);
                    else
                        InstructionSet.InstructionSetContainer[Authentication.ConfigurationName + ".FtpClientAddress"] = Authentication.TargetAddress(null);

                    string sData = null;
                    byte[] bData = null;

                    switch (TargetContainer)
                    {
                        case ContainerType.InstructionSetContainer:

                            if (!InstructionSet.InstructionSetContainer.ContainsKey(ContainerDataKey))
                                throw new Exception("ContainerDataKey (" + ContainerDataKey + ") does not exist.");

                            sData = InstructionSet.InstructionSetContainer[ContainerDataKey] as string;
                            bData = InstructionSet.InstructionSetContainer[ContainerDataKey] as byte[];

                            break;

                        case ContainerType.Session:

                            if (!STEM.Sys.State.Containers.Session.ContainsKey(ContainerDataKey))
                                throw new Exception("ContainerDataKey (" + ContainerDataKey + ") does not exist.");

                            sData = STEM.Sys.State.Containers.Session[ContainerDataKey] as string;
                            bData = STEM.Sys.State.Containers.Session[ContainerDataKey] as byte[];

                            break;

                        case ContainerType.Cache:

                            if (!STEM.Sys.State.Containers.Cache.ContainsKey(ContainerDataKey))
                                throw new Exception("ContainerDataKey (" + ContainerDataKey + ") does not exist.");

                            sData = STEM.Sys.State.Containers.Cache[ContainerDataKey] as string;
                            bData = STEM.Sys.State.Containers.Cache[ContainerDataKey] as byte[];

                            break;
                    }

                    string file = DestinationFile;

                    if (!Authentication.DirectoryExists(STEM.Sys.IO.Path.GetDirectoryName(file)))
                        Authentication.CreateDirectory(STEM.Sys.IO.Path.GetDirectoryName(file));

                    if (Authentication.FileExists(file))
                        switch (FileExistsAction)
                        {
                            case STEM.Sys.IO.FileExistsAction.Skip:
                                return true;

                            case STEM.Sys.IO.FileExistsAction.Throw:
                                r = -1;
                                throw new System.IO.IOException("Destination file exists. (" + file + ")");

                            case STEM.Sys.IO.FileExistsAction.Overwrite:
                                Authentication.DeleteFile(file);
                                break;

                            case STEM.Sys.IO.FileExistsAction.OverwriteIfNewer:
                                // Assume newer
                                Authentication.DeleteFile(file);
                                break;

                            case STEM.Sys.IO.FileExistsAction.MakeUnique:
                                file = Authentication.UniqueFilename(file);
                                break;
                        }

                    byte[] data = null;

                    if (bData != null && bData.Length > 0)
                        data = bData;

                    if (data == null)
                        if (sData != null && sData.Length > 0)
                            data = System.Text.Encoding.UTF8.GetBytes(sData);

                    if (data == null && CreateEmptyFiles)
                        data = new byte[0];

                    if (data != null)
                    {
                        using (System.IO.Stream s = new System.IO.MemoryStream(data))
                        {
                            Authentication.UploadFile(s, file);
                        }
                            
                        _SavedFile = file;
                    }

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
