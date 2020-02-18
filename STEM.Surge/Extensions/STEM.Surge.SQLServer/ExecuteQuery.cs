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
using System.Data;

namespace STEM.Surge.SQLServer
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("ExecuteQuery")]
    [Description("Execute a query and put the resulting DataSet in a container for others to use.")]
    public class ExecuteQuery : SQLBaseInstruction
    {
        [DisplayName("Sql to be executed"), DescriptionAttribute("This is the Sql that will be executed.")]
        public List<string> Sql { get; set; }

        [DisplayName("Retry Attempts")]
        public int Retry { get; set; }

        [DisplayName("Container Data Key")]
        [Description("The key in the container where the query results DataSet is to be stored.")]
        public string ContainerDataKey { get; set; }

        [DisplayName("Target Container")]
        [Description("The container where the query results DataSet is to be stored.")]
        public ContainerType TargetContainer { get; set; }

        public ExecuteQuery()
        {
            Retry = 3;
            Sql = new List<string>();
            ContainerDataKey = "[InitiationSource]";
            TargetContainer = ContainerType.InstructionSetContainer;
        }

        protected override void _Rollback()
        {
            // No rollback
        }

        protected override bool _Run()
        {
            try
            {
                DataSet ds = base.ExecuteQuery(String.Join("\r\n", Sql), Retry);

                switch (TargetContainer)
                {
                    case ContainerType.InstructionSetContainer:

                        InstructionSet.InstructionSetContainer[ContainerDataKey] = ds;

                        break;

                    case ContainerType.Session:

                        STEM.Sys.State.Containers.Session[ContainerDataKey] = ds;

                        break;

                    case ContainerType.Cache:
                        
                        STEM.Sys.State.Containers.Cache[ContainerDataKey] = ds;
                        
                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                Exceptions.Add(ex);
                return false;
            }
        }
    }
}
