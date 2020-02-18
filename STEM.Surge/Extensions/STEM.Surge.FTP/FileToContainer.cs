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

namespace STEM.Surge.FTP
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("File To Container")]
    [Description("Write data to a container from a file on the FTP server.")]
    public class FileToContainer : STEM.Surge.Instruction
    {
        [Category("FTP Server")]
        [DisplayName("Authentication"), DescriptionAttribute("The authentication configuration to be used.")]
        public Authentication Authentication { get; set; }

        [Category("FTP Server")]
        [DisplayName("FTP Server Address"), DescriptionAttribute("What is the FTP Server Address?")]
        public string ServerAddress { get; set; }

        [Category("FTP Server")]
        [DisplayName("FTP Port"), DescriptionAttribute("What is the FTP Port?")]
        public string Port { get; set; }

        [DisplayName("Source File")]
        [Description("The file data to be loaded into the container.")]
        public string SourceFile { get; set; }

        [DisplayName("Container Data Key")]
        [Description("The key in the container where the file data is to be loaded.")]
        public string ContainerDataKey { get; set; }

        [DisplayName("Target Container")]
        [Description("The container with the data to be saved.")]
        public ContainerType TargetContainer { get; set; }

        [DisplayName("Data Type")]
        [Description("Whether the data in the file is a string or a byte array.")]
        public DataType FileType { get; set; }

        [Category("Retry")]
        [DisplayName("Number of retries"), DescriptionAttribute("How many times should each operation be attempted?")]
        public int Retry { get; set; }

        [Category("Retry")]
        [DisplayName("Seconds between retries"), DescriptionAttribute("How many seconds should we wait between retries?")]
        public int RetryDelaySeconds { get; set; }

        public FileToContainer()
        {
            Authentication = new Authentication();
            ServerAddress = "[FtpServerAddress]";
            Port = "[FtpServerPort]";

            SourceFile = "[TargetPath]\\[TargetName]";
            ContainerDataKey = "[TargetNameWithoutExt]";
            TargetContainer = ContainerType.InstructionSetContainer;
            FileType = DataType.Binary;

            Retry = 1;
            RetryDelaySeconds = 2;
        }

        protected override void _Rollback()
        {
            switch (TargetContainer)
            {
                case ContainerType.InstructionSetContainer:

                    InstructionSet.InstructionSetContainer[ContainerDataKey] = null;
                    break;

                case ContainerType.Session:

                    STEM.Sys.State.Containers.Session[ContainerDataKey] = null;
                    break;

                case ContainerType.Cache:

                    STEM.Sys.State.Containers.Cache[ContainerDataKey] = null;
                    break;
            }
        }

        protected override bool _Run()
        {
            int r = Retry;

            while (r-- >= 0)
                try
                {
                    string address = Authentication.NextAddress(ServerAddress);

                    if (address == null)
                    {
                        Exception ex = new Exception("No valid address. (" + ServerAddress + ")");
                        Exceptions.Add(ex);
                        AppendToMessage(ex.Message);
                        return false;
                    }

                    FtpClient conn = Authentication.OpenClient(address, Int32.Parse(Port));

                    try
                    {
                        string file = Authentication.AdjustPath(address, SourceFile);

                        if (!conn.FileExists(file))
                            throw new System.IO.IOException("File does not exist.");
                        
                        string sData = null;
                        byte[] bData = null;
                        FtpReply reply;

                        switch (FileType)
                        {
                            case DataType.Binary:
                                using (System.IO.Stream s = conn.OpenRead(file, FtpDataType.Binary))
                                {
                                    bData = new byte[s.Length];
                                    s.Read(bData, 0, bData.Length);
                                }

                                reply = conn.GetReply();

                                if (!reply.Success)
                                    throw new Exception("There was an error reading from the FTP server: " + reply.Message);

                                break;

                            case DataType.String:
                                using (System.IO.Stream s = conn.OpenRead(file, FtpDataType.Binary))
                                {
                                    bData = new byte[s.Length];
                                    s.Read(bData, 0, bData.Length);
                                }

                                reply = conn.GetReply();

                                if (!reply.Success)
                                    throw new Exception("There was an error reading from the FTP server: " + reply.Message);

                                sData = System.Text.Encoding.Unicode.GetString(bData, 0, bData.Length);
                                bData = null;
                                break;
                        }

                        switch (TargetContainer)
                        {
                            case ContainerType.InstructionSetContainer:

                                if (bData != null)
                                    InstructionSet.InstructionSetContainer[ContainerDataKey] = bData;
                                else
                                    InstructionSet.InstructionSetContainer[ContainerDataKey] = sData;

                                break;

                            case ContainerType.Session:

                                if (bData != null)
                                    STEM.Sys.State.Containers.Session[ContainerDataKey] = bData;
                                else
                                    STEM.Sys.State.Containers.Session[ContainerDataKey] = sData;

                                break;

                            case ContainerType.Cache:

                                if (bData != null)
                                    STEM.Sys.State.Containers.Cache[ContainerDataKey] = bData;
                                else
                                    STEM.Sys.State.Containers.Cache[ContainerDataKey] = sData;

                                break;
                        }
                    }
                    finally
                    {
                        Authentication.RecycleClient(conn);
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
