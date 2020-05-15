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
using STEM.Surge;

namespace STEM.Surge.FlowControl
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("DoNextIfRegex")]
    [Description("Do the next Instruction in the InstructionSet if the file specified in Filename matches the RegEx filter specified in RegEx Filter.")]
    public class DoNextIfRegex : Instruction
    {
        [Category("Flow")]
        [DisplayName("Test against")]
        [Description("The string to be evaluated")]
        public string TestString { get; set; }

        [Category("Flow")]
        [DisplayName("RegEx Filter"), DescriptionAttribute("The filter used against Filename.")]
        public string RegexFilter { get; set; }

        [Category("Flow")]
        [DisplayName("Execution Mode"), Description("Should this be executed on forward InstructionSet execution or on Rollback?")]
        public ExecuteOn ExecutionMode { get; set; }

        public DoNextIfRegex() : base()
        {
            TestString = "[TargetPath]\\[TargetName]";
            RegexFilter = "*";
            ExecutionMode = ExecuteOn.ForwardExecution;
        }

        protected override bool _Run()
        {
            if (ExecutionMode == ExecuteOn.ForwardExecution)
            {
                try
                {
                    if (!STEM.Sys.IO.Path.StringMatches(TestString, new System.Text.RegularExpressions.Regex(RegexFilter, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled)))
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
                    if (!STEM.Sys.IO.Path.StringMatches(TestString, new System.Text.RegularExpressions.Regex(RegexFilter, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled)))
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
