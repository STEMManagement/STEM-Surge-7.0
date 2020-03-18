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
using System.Linq;
using System.ComponentModel;
using ICSharpCode.SharpZipLib.Zip;

namespace STEM.Surge.Compression
{
    [Serializable()]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("UnZip In Container")]
    [Description("UnZip data in a container to a Dictionary<string, byte[]>.")]
    public class UnZipInContainer : Instruction
    {
        [DisplayName("Container Data Key")]
        [Description("The key in the container where the data is loaded.")]
        public string ContainerDataKey { get; set; }

        [DisplayName("Target Container")]
        [Description("The container with the data to be zipped.")]
        public ContainerType TargetContainer { get; set; }

        public UnZipInContainer()
        {
            ContainerDataKey = "[TargetNameWithoutExt]";
            TargetContainer = ContainerType.InstructionSetContainer;
        }

        protected override void _Rollback()
        {
            switch (TargetContainer)
            {
                case ContainerType.InstructionSetContainer:
                    InstructionSet.InstructionSetContainer[ContainerDataKey] = _BData;
                    break;

                case ContainerType.Session:
                    STEM.Sys.State.Containers.Session[ContainerDataKey] = _BData;
                    break;

                case ContainerType.Cache:
                    STEM.Sys.State.Containers.Cache[ContainerDataKey] = _BData;
                    break;
            }
        }

        byte[] _BData = null;

        protected override bool _Run()
        {
            try
            {
                switch (TargetContainer)
                {
                    case ContainerType.InstructionSetContainer:

                        if (!InstructionSet.InstructionSetContainer.ContainsKey(ContainerDataKey))
                            throw new Exception("ContainerDataKey (" + ContainerDataKey + ") does not exist.");

                        _BData = InstructionSet.InstructionSetContainer[ContainerDataKey] as byte[];

                        break;

                    case ContainerType.Session:

                        if (!STEM.Sys.State.Containers.Session.ContainsKey(ContainerDataKey))
                            throw new Exception("ContainerDataKey (" + ContainerDataKey + ") does not exist.");

                        _BData = STEM.Sys.State.Containers.Session[ContainerDataKey] as byte[];

                        break;

                    case ContainerType.Cache:

                        if (!STEM.Sys.State.Containers.Cache.ContainsKey(ContainerDataKey))
                            throw new Exception("ContainerDataKey (" + ContainerDataKey + ") does not exist.");

                        _BData = STEM.Sys.State.Containers.Cache[ContainerDataKey] as byte[];

                        break;
                }

                if (_BData == null || _BData.Length == 0)
                    throw new Exception("ContainerDataKey (" + ContainerDataKey + ") has no data.");

                Dictionary<string, byte[]> zData = new Dictionary<string, byte[]>();

                using (MemoryStream s = new MemoryStream(_BData))
                {
                    using (ZipInputStream zStream = new ZipInputStream(s))
                    {
                        zStream.IsStreamOwner = false;
                        ZipEntry e = null;

                        while ((e = zStream.GetNextEntry()) != null)
                        {
                            if (!e.IsDirectory && e.Size > 0)
                            {
                                using (MemoryStream o = new MemoryStream())
                                {
                                    zStream.CopyTo(o);
                                    o.Position = 0;
                                    zData[e.Name] = o.GetBuffer().Take((int)e.Size).ToArray();
                                }
                            }
                            else
                            {
                                zData[e.Name] = null;
                            }

                            break;
                        }
                    }
                }

                switch (TargetContainer)
                {
                    case ContainerType.InstructionSetContainer:
                        InstructionSet.InstructionSetContainer[ContainerDataKey] = zData;
                        break;

                    case ContainerType.Session:
                        STEM.Sys.State.Containers.Session[ContainerDataKey] = zData;
                        break;

                    case ContainerType.Cache:
                        STEM.Sys.State.Containers.Cache[ContainerDataKey] = zData;
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
