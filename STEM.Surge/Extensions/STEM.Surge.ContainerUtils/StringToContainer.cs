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
using System.ComponentModel;

namespace STEM.Surge.ContainerUtils
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("String To Container")]
    [Description("Plant a string value in a container.")]
    public class StringToContainer : STEM.Surge.Instruction
    {
        [DisplayName("Container Data")]
        [Description("The data to load into the container.")]
        public string ContainerData { get; set; }

        [DisplayName("Container Data Key")]
        [Description("The key in the container where the data is loaded.")]
        public string ContainerDataKey { get; set; }

        [DisplayName("Target Container")]
        [Description("The container in which to put the value.")]
        public ContainerType TargetContainer { get; set; }

        public StringToContainer()
        {
            ContainerData = "[TargetName]";
            ContainerDataKey = "[TargetNameWithoutExt]";
            TargetContainer = ContainerType.InstructionSetContainer;
        }

        protected override void _Rollback()
        {
            try
            {
                switch (TargetContainer)
                {
                    case ContainerType.InstructionSetContainer:
                        InstructionSet.InstructionSetContainer.Remove(ContainerDataKey);
                        break;

                    case ContainerType.Session:
                        STEM.Sys.State.Containers.Session[ContainerDataKey] = null;
                        break;

                    case ContainerType.Cache:
                        STEM.Sys.State.Containers.Cache[ContainerDataKey] = null;
                        break;
                }
            }
            catch { }
        }

        protected override bool _Run()
        {
            try
            {
                switch (TargetContainer)
                {
                    case ContainerType.InstructionSetContainer:
                        InstructionSet.InstructionSetContainer[ContainerDataKey] = ContainerData;
                        break;

                    case ContainerType.Session:
                        STEM.Sys.State.Containers.Session[ContainerDataKey] = ContainerData;
                        break;

                    case ContainerType.Cache:
                        STEM.Sys.State.Containers.Cache[ContainerDataKey] = ContainerData;
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
