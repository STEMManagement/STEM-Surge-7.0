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
using System.ComponentModel;
using System.IO;

namespace STEM.Surge.SMB
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("FileInUse")]
    [Description("Check to see if a file is in use (i.e. open by another process) and take a flow action.")]
    public class FileInUse : Instruction
    {
        [DisplayName("Filename")]
        [Description("The full path of the file to be evaluated")]
        public string FileName { get; set; }

        [DisplayName("If file is in use (i.e. open by another process)")]
        [Description("What action should be taken if the file is in use (i.e. open by another process)?")]
        public Surge.FailureAction FileInUseAction { get; set; }

        [DisplayName("Target Label")]
        [Description("The label to skip forward to when Action == SkipToLabel")]
        public string TargetLabel { get; set; }

        public FileInUse()
            : base()
        {
            FileName = "[TargetPath]\\[TargetName]";
            TargetLabel = "";
        }

        protected override bool _Run()
        {
            FileName = STEM.Sys.IO.Path.AdjustPath(FileName);
            try
            {
                if (File.Exists(FileName))
                    try
                    {
                        using (FileStream s = File.Open(FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                        {
                        }
                    }
                    catch
                    {
                        switch (FileInUseAction)
                        {
                            case Surge.FailureAction.SkipRemaining:

                                SkipRemaining();
                                break;

                            case Surge.FailureAction.SkipNext:

                                SkipNext();
                                break;

                            case Surge.FailureAction.Rollback:

                                RollbackAllPreceedingAndSkipRemaining();
                                break;

                            case Surge.FailureAction.Continue:

                                break;

                            case Surge.FailureAction.SkipToLabel:

                                SkipForwardToFlowControlLabel(TargetLabel);
                                break;
                        }
                    }
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
        }
    }
}
