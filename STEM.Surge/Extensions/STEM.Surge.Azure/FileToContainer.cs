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
using Microsoft.Azure.Storage.Blob;

namespace STEM.Surge.Azure
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("File To Container")]
    [Description("Write data to a container from a file in Azure.")]
    public class FileToContainer : STEM.Surge.Instruction
    {
        public enum ContainerType
        {
            InstructionSetContainer,
            Session,
            Cache,
        }

        public enum DataType
        {
            String,
            Binary
        }

        [Category("Azure")]
        [DisplayName("Authentication"), DescriptionAttribute("The authentication configuration to be used.")]
        public Authentication Authentication { get; set; }
        
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

        public FileToContainer()
        {
            Authentication = new Authentication();

            SourceFile = "[TargetPath]\\[TargetName]";
            ContainerDataKey = "[TargetNameWithoutExt]";
            TargetContainer = ContainerType.InstructionSetContainer;
            FileType = DataType.Binary;
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
            try
            {
                if (!Authentication.FileExists(SourceFile))
                    throw new System.IO.IOException("File does not exist.");
                
                CloudBlockBlob blob = Authentication.GetCloudBlockBlob(SourceFile, false);

                if (blob == null)
                    throw new Exception("File does not exist or can not be reached: " + SourceFile);

                string sData = null;
                byte[] bData = null;

                System.Threading.Tasks.Task<System.IO.Stream> streamResult = blob.OpenReadAsync();
                streamResult.Wait();

                switch (FileType)
                {
                    case DataType.Binary:
                        using (System.IO.Stream s = streamResult.Result)
                        {
                            bData = new byte[s.Length];
                            s.Read(bData, 0, bData.Length);
                        }

                        break;

                    case DataType.String:
                        using (System.IO.Stream s = streamResult.Result)
                        {
                            bData = new byte[s.Length];
                            s.Read(bData, 0, bData.Length);
                        }

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
            catch (AggregateException ex)
            {
                foreach (Exception e in ex.InnerExceptions)
                {
                    AppendToMessage(e.Message);
                    Exceptions.Add(e);
                }
            }
            catch (Exception ex)
            {
                AppendToMessage(ex.Message);
                Exceptions.Add(ex);
            }

            return Exceptions.Count == 0;
        }
    }
}
