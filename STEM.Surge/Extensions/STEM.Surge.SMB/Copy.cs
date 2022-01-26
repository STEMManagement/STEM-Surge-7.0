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

namespace STEM.Surge.SMB
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Copy")]
    [Description("Copy a file or folder from 'Source Path' to 'Destination Path' where either or both can be expandable paths.")]
    public class Copy : CopyBase
    {
        [Category("Source")]
        [DisplayName("Source Path"), DescriptionAttribute("The location of the file(s) to be actioned.")]
        public string SourcePath { get; set; }

        [Category("Source")]
        [DisplayName("Expand Source Path"), DescriptionAttribute("When searching for files to action, should the 'Source Path' be treated like an expandable?.")]
        public bool ExpandSource { get; set; }

        [Category("Source")]
        [DisplayName("File Filter"), DescriptionAttribute("The filter used on files in 'Source Path' to select files to action. This can be a compound filter.")]
        public string FileFilter { get; set; }

        [Category("Source")]
        [DisplayName("Directory Filter"), DescriptionAttribute("The filter used on directories in 'Source Path' when 'Recurse Source' is specified. This can be a compound filter.")]
        public string DirectoryFilter { get; set; }

        [Category("Source")]
        [DisplayName("Recurse Source"), DescriptionAttribute("Recurse the source for files to action?")]
        public bool RecurseSource { get; set; }

        public Copy()
            : base()
        {
            ExpandSource = false;
            SourcePath = "[TargetPath]";
            FileFilter = "[TargetName]";
            DirectoryFilter = "!TEMP";
            RecurseSource = false;
            Action = ActionType.Copy;
        }

        protected override void _Rollback()
        {
            CB_SourcePath = SourcePath;
            CB_ExpandSource = ExpandSource;
            CB_FileFilter = FileFilter;
            CB_DirectoryFilter = DirectoryFilter;
            CB_RecurseSource = RecurseSource;

            base._Rollback();
        }

        protected override bool _Run()
        {
            CB_SourcePath = SourcePath;
            CB_ExpandSource = ExpandSource;
            CB_FileFilter = FileFilter;
            CB_DirectoryFilter = DirectoryFilter;
            CB_RecurseSource = RecurseSource;

            return base._Run();
        }
    }
}