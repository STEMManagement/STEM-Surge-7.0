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
    [DisplayName("Container To File")]
    [Description("Write data from a container to a file.")]
    public class ContainerToFile : Instruction
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

        public ContainerToFile()
        {
            DestinationFile = "[DestinationPath]\\[TargetName]";
            ContainerDataKey = "[TargetNameWithoutExt]";
            TargetContainer = ContainerType.InstructionSetContainer;
            FileExistsAction = STEM.Sys.IO.FileExistsAction.MakeUnique;
            CreateEmptyFiles = false;
        }

        protected override void _Rollback()
        {
            if (_SavedFile != null)
                try
                {
                    STEM.Sys.IO.File.STEM_Delete(_SavedFile, false);
                }
                catch (Exception ex)
                {
                    AppendToMessage(ex.Message);
                    Exceptions.Add(ex);
                }
        }

        string _SavedFile = null;
        protected override bool _Run()
        {
            DestinationFile = STEM.Sys.IO.Path.AdjustPath(DestinationFile);
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

                if (!Directory.Exists(STEM.Sys.IO.Path.GetDirectoryName(DestinationFile)))
                    Directory.CreateDirectory(STEM.Sys.IO.Path.GetDirectoryName(DestinationFile));

                string file = DestinationFile;

                if (File.Exists(file))
                    switch (FileExistsAction)
                    {
                        case STEM.Sys.IO.FileExistsAction.Skip:
                            return true;

                        case STEM.Sys.IO.FileExistsAction.Throw:
                            throw new IOException("File already exists.");

                        case STEM.Sys.IO.FileExistsAction.Overwrite:
                            break;

                        case STEM.Sys.IO.FileExistsAction.MakeUnique:
                            file = STEM.Sys.IO.File.UniqueFilename(file);
                            break;
                    }
                
                byte[] data = null;
                
                if (bData != null && bData.Length > 0)
                    data = bData;

                if (data == null)
                    if (sData != null && sData.Length > 0)
                        data = System.Text.Encoding.UTF8.GetBytes(sData);

                if (data != null)
                {
                    File.WriteAllBytes(file, data);
                    _SavedFile = file;
                }
                else if (CreateEmptyFiles)
                {
                    File.WriteAllText(file, "");
                    _SavedFile = file;
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
