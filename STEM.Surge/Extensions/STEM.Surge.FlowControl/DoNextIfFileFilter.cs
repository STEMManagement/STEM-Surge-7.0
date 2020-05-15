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
using STEM.Surge;

namespace STEM.Surge.FlowControl
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("DoNextIfFileFilter")]
    [Description("Do the next Instruction in the InstructionSet if the file specified in Filename matches the file filter specified in FileFilter.")]
    public class DoNextIfFileFilter : Instruction
    {
        [Category("Flow")]
        [DisplayName("Filename")]
        [Description("The name of the file to be evaluated")]
        public string FileName { get; set; }

        [Category("Flow")]
        [DisplayName("File Filter"), DescriptionAttribute("The filter used against Filename. This can be a compound filter.")]
        public string FileFilter { get; set; }

        [Category("Flow")]
        [DisplayName("Execution Mode"), Description("Should this be executed on forward InstructionSet execution or on Rollback?")]
        public ExecuteOn ExecutionMode { get; set; }

        public DoNextIfFileFilter() : base()
        {
            FileName = "[TargetName]";
            FileFilter = "*";
            ExecutionMode = ExecuteOn.ForwardExecution;
        }

        protected override bool _Run()
        {
            if (ExecutionMode == ExecuteOn.ForwardExecution)
            {
                try
                {
                    if (!STEM.Sys.IO.Path.StringMatches(FileName, FileFilter))
                        SkipNext();
                }
                catch (Exception ex)
                {
                    AppendToMessage(ex.Message);
                    Exceptions.Add(ex);
                }

                return Exceptions.Count == 0;
            }

            return true;
        }

        protected override void _Rollback()
        {
            if (ExecutionMode == ExecuteOn.Rollback)
            {
                try
                {
                    if (!STEM.Sys.IO.Path.StringMatches(FileName, FileFilter))
                        SkipPrevious();
                }
                catch (Exception ex)
                {
                    AppendToMessage(ex.Message);
                    Exceptions.Add(ex);
                }
            }
        }
    }
}
