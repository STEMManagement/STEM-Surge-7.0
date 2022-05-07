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

namespace STEM.Surge.FlowControl
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("SkipNextIfExceptionsContain")]
    [Description("Skip the next Instruction in the InstructionSet if the Exceptions Collection contains the 'Exception Like' string fragment.")]
    public class SkipNextIfExceptionsContain : Instruction
    {
        [Category("Flow")]
        [DisplayName("Exception Filter"), DescriptionAttribute("The filter used against Exceptions Collection. (One filter per line, all filters are 'OR' evaluations)")]
        public List<string> ExceptionFilter { get; set; }

        [Category("Flow")]
        [DisplayName("Execution Mode"), Description("Should this be executed on forward InstructionSet execution or on Rollback?")]
        public ExecuteOn ExecutionMode { get; set; }

        public SkipNextIfExceptionsContain() : base()
        {
            ExceptionFilter = new List<string>();
            ExecutionMode = ExecuteOn.ForwardExecution;
        }

        protected override bool _Run()
        {
            if (ExecutionMode == ExecuteOn.ForwardExecution)
            {
                try
                {
                    if (ExceptionFilter.Count == 0)
                        return true;

                    foreach (string exception in ExceptionFilter)
                        if (Exceptions.Exists(i => i.ToString().ToUpper().Contains(exception.ToUpper())))
                        {
                            SkipNext();
                            return true;
                        }
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
                    if (ExceptionFilter.Count == 0)
                        return;

                    foreach (string exception in ExceptionFilter)
                        if (Exceptions.Exists(i => i.ToString().ToUpper().Contains(exception.ToUpper())))
                        {
                            SkipPrevious();
                            return;
                        }
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

