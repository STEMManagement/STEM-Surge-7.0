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
using System.ComponentModel;

namespace STEM.Surge.SMB
{
    [Serializable()]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("File To Container")]
    [Description("Write data to a container from a file.")]
    public class FileToContainer : Instruction
    {
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
            SourceFile = STEM.Sys.IO.Path.AdjustPath(SourceFile);
            try
            {
                if (!File.Exists(SourceFile))
                    throw new IOException("File does not exist.");

                string sData = null;
                byte[] bData = null;

                switch (FileType)
                {
                    case DataType.Binary:
                        bData = File.ReadAllBytes(SourceFile);
                        break;

                    case DataType.String:
                        sData = File.ReadAllText(SourceFile);
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
            catch (Exception ex)
            {
                AppendToMessage(ex.Message);
                Exceptions.Add(ex);
            }

            return Exceptions.Count == 0;
        }
    }
}
