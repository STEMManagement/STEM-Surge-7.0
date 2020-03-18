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
    [DisplayName("Zip In Container")]
    [Description("Zip data in a container where the data can be string, byte[], or Dictionary<string, byte[]>.")]
    public class ZipInContainer : Instruction
    {
        [DisplayName("Container Data Key")]
        [Description("The key in the container where the data is loaded.")]
        public string ContainerDataKey { get; set; }

        [DisplayName("Target Container")]
        [Description("The container with the data to be zipped.")]
        public ContainerType TargetContainer { get; set; }

        public ZipInContainer()
        {
            ContainerDataKey = "[TargetNameWithoutExt]";
            TargetContainer = ContainerType.InstructionSetContainer;
        }

        protected override void _Rollback()
        {
            switch (TargetContainer)
            {
                case ContainerType.InstructionSetContainer:
                    if (_SData != null)
                        InstructionSet.InstructionSetContainer[ContainerDataKey] = _SData;
                    else if (_BData != null)
                        InstructionSet.InstructionSetContainer[ContainerDataKey] = _BData;
                    else
                        InstructionSet.InstructionSetContainer[ContainerDataKey] = _ZData;
                    break;

                case ContainerType.Session:
                    if (_SData != null)
                        STEM.Sys.State.Containers.Session[ContainerDataKey] = _SData;
                    else if (_BData != null)
                        STEM.Sys.State.Containers.Session[ContainerDataKey] = _BData;
                    else
                        STEM.Sys.State.Containers.Session[ContainerDataKey] = _ZData;
                    break;

                case ContainerType.Cache:
                    if (_SData != null)
                        STEM.Sys.State.Containers.Cache[ContainerDataKey] = _SData;
                    else if (_BData != null)
                        STEM.Sys.State.Containers.Cache[ContainerDataKey] = _BData;
                    else
                        STEM.Sys.State.Containers.Cache[ContainerDataKey] = _ZData;
                    break;
            }
        }

        string _SData = null;
        byte[] _BData = null;
        Dictionary<string, byte[]> _ZData = null;

        protected override bool _Run()
        {
            try
            {
                switch (TargetContainer)
                {
                    case ContainerType.InstructionSetContainer:

                        if (!InstructionSet.InstructionSetContainer.ContainsKey(ContainerDataKey))
                            throw new Exception("ContainerDataKey (" + ContainerDataKey + ") does not exist.");

                        _SData = InstructionSet.InstructionSetContainer[ContainerDataKey] as string;
                        _BData = InstructionSet.InstructionSetContainer[ContainerDataKey] as byte[];
                        _ZData = InstructionSet.InstructionSetContainer[ContainerDataKey] as Dictionary<string, byte[]>;

                        break;

                    case ContainerType.Session:

                        if (!STEM.Sys.State.Containers.Session.ContainsKey(ContainerDataKey))
                            throw new Exception("ContainerDataKey (" + ContainerDataKey + ") does not exist.");

                        _SData = STEM.Sys.State.Containers.Session[ContainerDataKey] as string;
                        _BData = STEM.Sys.State.Containers.Session[ContainerDataKey] as byte[];
                        _ZData = STEM.Sys.State.Containers.Session[ContainerDataKey] as Dictionary<string, byte[]>;

                        break;

                    case ContainerType.Cache:

                        if (!STEM.Sys.State.Containers.Cache.ContainsKey(ContainerDataKey))
                            throw new Exception("ContainerDataKey (" + ContainerDataKey + ") does not exist.");

                        _SData = STEM.Sys.State.Containers.Cache[ContainerDataKey] as string;
                        _BData = STEM.Sys.State.Containers.Cache[ContainerDataKey] as byte[];
                        _ZData = STEM.Sys.State.Containers.Cache[ContainerDataKey] as Dictionary<string, byte[]>;

                        break;
                }
                                
                byte[] zData = null;

                if (_BData != null && _BData.Length > 0)
                    zData = _BData;
                else if (_SData != null && _SData.Length > 0)
                    zData = System.Text.Encoding.UTF8.GetBytes(_SData);

                if (zData == null && _ZData == null)
                    throw new Exception("ContainerDataKey (" + ContainerDataKey + ") has no data.");

                if (_ZData != null)
                {
                    using (MemoryStream s = new MemoryStream())
                    {
                        using (ZipOutputStream zStream = new ZipOutputStream(s))
                        {
                            zStream.IsStreamOwner = false;

                            foreach (string name in _ZData.Keys)
                            {
                                ZipEntry e = new ZipEntry(name);

                                if (_ZData[name] == null)
                                {
                                    e.Size = 0;
                                    zStream.PutNextEntry(e);
                                }
                                else
                                {
                                    e.Size = _ZData[name].Length;
                                    zStream.PutNextEntry(e);

                                    zStream.Write(_ZData[name], 0, _ZData[name].Length);
                                }

                                zStream.CloseEntry();
                            }
                        }

                        int len = (int)s.Position;
                        s.Position = 0;
                        zData = s.GetBuffer().Take(len).ToArray();
                    }
                }
                else
                {
                    using (MemoryStream s = new MemoryStream())
                    {
                        using (ZipOutputStream zStream = new ZipOutputStream(s))
                        {
                            zStream.IsStreamOwner = false;
                            
                            ZipEntry e = new ZipEntry(ContainerDataKey);
                            e.Size = zData.Length;

                            zStream.PutNextEntry(e);

                            zStream.Write(zData, 0, zData.Length);

                            zStream.CloseEntry();
                        }

                        int len = (int)s.Position;
                        s.Position = 0;
                        zData = s.GetBuffer().Take(len).ToArray();
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
