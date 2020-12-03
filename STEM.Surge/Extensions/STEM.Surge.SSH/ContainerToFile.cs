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
    [DisplayName("Container To File")]
    [Description("Write data from a container to a file on the SSH server.")]
    public class ContainerToFile : STEM.Surge.Instruction
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
            ServerAddress = "[SshServerAddress]";
            Port = "[SshServerPort]";

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

            while (r-- >= 0 && !Stop)
            {
                if (_SavedFile != null)
                    try
                    {
                        PostMortemMetaData["LastOperation"] = "DeleteFile";
                        Authentication.DeleteFile(_Address, Int32.Parse(Port), _SavedFile);
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
        }

        string _SavedFile = null;

        string _Address = null;

        protected override bool _Run()
        {
            int r = Retry;

            while (r-- >= 0 && !Stop)
            {
                PostMortemMetaData["LastOperation"] = "NextAddress";
                _Address = Authentication.NextAddress(ServerAddress);

                if (_Address == null)
                {
                    Exception ex = new Exception("No valid address. (" + ServerAddress + ")");
                    Exceptions.Add(ex);
                    AppendToMessage(ex.Message);
                    return false;
                }

                try
                {
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

                    string dFile = DestinationFile;

                    PostMortemMetaData["LastOperation"] = "FileExists";
                    if (Authentication.FileExists(_Address, Int32.Parse(Port), DestinationFile))
                        switch (FileExistsAction)
                        {
                            case STEM.Sys.IO.FileExistsAction.Skip:
                                return true;

                            case STEM.Sys.IO.FileExistsAction.Throw:
                                r = -1;
                                throw new System.IO.IOException("Destination file exists. (" + DestinationFile + ")");

                            case STEM.Sys.IO.FileExistsAction.Overwrite:
                            case STEM.Sys.IO.FileExistsAction.OverwriteIfNewer:
                                Authentication.DeleteFile(_Address, Int32.Parse(Port), DestinationFile);
                                break;

                            case STEM.Sys.IO.FileExistsAction.MakeUnique:
                                DestinationFile = Authentication.UniqueFilename(_Address, Int32.Parse(Port), DestinationFile);
                                break;
                        }

                    dFile = Authentication.AdjustPath(_Address, dFile);
                    string directory = Authentication.AdjustPath(_Address, STEM.Sys.IO.Path.GetDirectoryName(dFile));
                    
                    PostMortemMetaData["LastOperation"] = "DirectoryExists";
                    if (!Authentication.DirectoryExists(_Address, Int32.Parse(Port), directory))
                    {
                        PostMortemMetaData["LastOperation"] = "CreateDirectory";
                        Authentication.CreateDirectory(_Address, Int32.Parse(Port), directory);
                    }

                    byte[] data = null;

                    if (bData != null && bData.Length > 0)
                        data = bData;

                    if (data == null)
                        if (sData != null && sData.Length > 0)
                            data = System.Text.Encoding.UTF8.GetBytes(sData);

                    if (data == null && CreateEmptyFiles)
                        data = new byte[1];

                    if (data != null)
                    {
                        PostMortemMetaData["LastOperation"] = "OpenSftpClient";
                        SftpClient client = null;

                        try
                        {
                            client = Authentication.OpenSftpClient(_Address, Int32.Parse(Port));

                            try
                            {
                                PostMortemMetaData["LastOperation"] = "UploadFile";
                                using (System.IO.MemoryStream s = new System.IO.MemoryStream(data))
                                {
                                    client.UploadFile(s, dFile);

                                    _SavedFile = dFile;
                                }
                            }
                            finally
                            {
                                PostMortemMetaData["LastOperation"] = "RecycleClient";
                                Authentication.RecycleClient(client);
                                client = null;
                            }
                        }
                        catch
                        {
                            try
                            {
                                client.Disconnect();
                            }
                            catch { }
                            try
                            {
                                client.Dispose();
                            }
                            catch { }

                            throw;
                        }
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
            }

            return Exceptions.Count == 0;
        }
    }
}
