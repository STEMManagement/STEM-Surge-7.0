﻿/*
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
    [DisplayName("SkipToLabelIfFileFilter")]
    [Description("Skip the next labeled Instruction in the InstructionSet if the file specified in Filename matches the file filter specified in FileFilter.")]
    public class SkipToLabelIfFileFilter : Instruction
    {
        [Category("Flow")]
        [DisplayName("Filename")]
        [Description("The full path of the file to be evaluated")]
        public string FileName { get; set; }

        [Category("Flow")]
        [DisplayName("File Filter"), DescriptionAttribute("The filter used against Filename. This can be a compound filter.")]
        public string FileFilter { get; set; }

        [Category("Flow")]
        [DisplayName("Flow Control Label")]
        [Description("The Flow Control Label of the Instruction to skip to.")]
        public string SkipToLabel { get; set; }

        public SkipToLabelIfFileFilter() : base()
        {
            FileName = "[TargetPath]\\[TargetName]";
            FileFilter = "*";
            SkipToLabel = "End";
        }

        protected override bool _Run()
        {
            try
            {
                if (STEM.Sys.IO.Path.StringMatches(FileName, FileFilter))
                    SkipForwardToFlowControlLabel(SkipToLabel);
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