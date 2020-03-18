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
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.GZip;

namespace STEM.Surge.Compression
{
    [Serializable()]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("TarGz In Container")]
    [Description("TarGZ data in a container where the data is a Dictionary<string, byte[]>.")]
    public class TarGzInContainer : Instruction
    {
        [DisplayName("Container Data Key")]
        [Description("The key in the container where the data is loaded.")]
        public string ContainerDataKey { get; set; }

        [DisplayName("Target Container")]
        [Description("The container with the data to be TarGZ'ed.")]
        public ContainerType TargetContainer { get; set; }

        public TarGzInContainer()
        {
            ContainerDataKey = "[TargetNameWithoutExt]";
            TargetContainer = ContainerType.InstructionSetContainer;
        }

        protected override void _Rollback()
        {
            switch (TargetContainer)
            {
                case ContainerType.InstructionSetContainer:
                    InstructionSet.InstructionSetContainer[ContainerDataKey] = _TData;
                    break;

                case ContainerType.Session:
                    STEM.Sys.State.Containers.Session[ContainerDataKey] = _TData;
                    break;

                case ContainerType.Cache:
                    STEM.Sys.State.Containers.Cache[ContainerDataKey] = _TData;
                    break;
            }
        }

        Dictionary<string, byte[]> _TData = null;

        protected override bool _Run()
        {
            try
            {
                switch (TargetContainer)
                {
                    case ContainerType.InstructionSetContainer:

                        if (!InstructionSet.InstructionSetContainer.ContainsKey(ContainerDataKey))
                            throw new Exception("ContainerDataKey (" + ContainerDataKey + ") does not exist.");

                        _TData = InstructionSet.InstructionSetContainer[ContainerDataKey] as Dictionary<string, byte[]>;

                        break;

                    case ContainerType.Session:

                        if (!STEM.Sys.State.Containers.Session.ContainsKey(ContainerDataKey))
                            throw new Exception("ContainerDataKey (" + ContainerDataKey + ") does not exist.");

                        _TData = STEM.Sys.State.Containers.Session[ContainerDataKey] as Dictionary<string, byte[]>;

                        break;

                    case ContainerType.Cache:

                        if (!STEM.Sys.State.Containers.Cache.ContainsKey(ContainerDataKey))
                            throw new Exception("ContainerDataKey (" + ContainerDataKey + ") does not exist.");

                        _TData = STEM.Sys.State.Containers.Cache[ContainerDataKey] as Dictionary<string, byte[]>;

                        break;
                }

                byte[] bData = null;

                if (_TData == null)
                    throw new Exception("ContainerDataKey (" + ContainerDataKey + ") has no data.");

                using (MemoryStream s = new MemoryStream())
                {
                    using (GZipOutputStream zStream = new GZipOutputStream(s))
                    {
                        zStream.IsStreamOwner = false;

                        using (TarOutputStream tStream = new TarOutputStream(zStream))
                        {
                            tStream.IsStreamOwner = false;

                            foreach (string name in _TData.Keys)
                            {
                                TarEntry e = TarEntry.CreateTarEntry(name);

                                if (_TData[name] == null)
                                {
                                    e.Size = 0;
                                    tStream.PutNextEntry(e);
                                }
                                else
                                {
                                    e.Size = _TData[name].Length;
                                    tStream.PutNextEntry(e);

                                    tStream.Write(_TData[name], 0, _TData[name].Length);
                                }

                                tStream.CloseEntry();
                            }
                        }
                    }

                    int len = (int)s.Position;
                    s.Position = 0;
                    bData = s.GetBuffer().Take(len).ToArray();
                }

                switch (TargetContainer)
                {
                    case ContainerType.InstructionSetContainer:
                        InstructionSet.InstructionSetContainer[ContainerDataKey] = bData;
                        break;

                    case ContainerType.Session:
                        STEM.Sys.State.Containers.Session[ContainerDataKey] = bData;
                        break;

                    case ContainerType.Cache:
                        STEM.Sys.State.Containers.Cache[ContainerDataKey] = bData;
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
