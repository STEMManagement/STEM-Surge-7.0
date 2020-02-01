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
    [DisplayName("SkipNextIfFileFilter")]
    [Description("Skip the next Instruction in the InstructionSet if the file specified in Filename matches the file filter specified in FileFilter.")]
    public class SkipNextIfFileFilter : Instruction
    {
        [Category("Flow")]
        [DisplayName("Filename")]
        [Description("The full path of the file to be evaluated")]
        public string FileName { get; set; }

        [Category("Flow")]
        [DisplayName("File Filter"), DescriptionAttribute("The filter used against Filename. This can be a compound filter.")]
        public string FileFilter { get; set; }

        public SkipNextIfFileFilter() : base()
        {
            FileName = "[TargetPath]\\[TargetName]";
            FileFilter = "*";
        }

        protected override bool _Run()
        {
            try
            {
                if (STEM.Sys.IO.Path.StringMatches(FileName, FileFilter))
                    SkipNext();
            }
            catch (Exception ex)
            {
                AppendToMessage(ex.Message);
                Exceptions.Add(ex);
            }

            return Exceptions.Count == 0;
        }

        protected override void _Rollback()
        {
            // does nothing
        }
    }
}
