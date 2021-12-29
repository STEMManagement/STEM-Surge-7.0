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
using System.ComponentModel;

namespace STEM.Surge.SMB
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Delete Files")]
    [Description("Delete files in the FileList.")]
    public class DeleteFiles : Instruction
    {
        [Category("Retry")]
        [DisplayName("Number of retries"), DescriptionAttribute("How many times should each operation be attempted?")]
        public int Retry { get; set; }

        [Category("Retry")]
        [DisplayName("Seconds between retries"), DescriptionAttribute("How many seconds should we wait between retries?")]
        public int RetryDelaySeconds { get; set; }

        [Category("Source")]
        [DisplayName("File List (Property Name: FileList)"), DescriptionAttribute("List<string> Property for the GroupingController to populate.")]
        public List<string> FileList { get; set; }

        [Category("Flow")]
        [DisplayName("Execution Mode"), Description("Should this be executed on forward InstructionSet execution or on Rollback? Consider the use case where you want to " +
            "delete a file out of the flow on Rollback.")]
        public ExecuteOn ExecutionMode { get; set; }

        public DeleteFiles()
            : base()
        {
            Retry = 1;
            RetryDelaySeconds = 2;
            FileList = new List<string>();
            ExecutionMode = ExecuteOn.ForwardExecution;
        }

        protected override bool _Run()
        {
            if (ExecutionMode == ExecuteOn.ForwardExecution)
                return Execute();

            return true;
        }

        protected override void _Rollback()
        {
            if (ExecutionMode == ExecuteOn.Rollback)
                Execute();
        }

        bool Execute()
        {
            try
            {
                foreach (string file in FileList)
                {
                    try
                    {
                        if (System.IO.File.Exists(file))
                        {
                            STEM.Sys.IO.File.STEM_Delete(file, false, Retry, RetryDelaySeconds);
                            AppendToMessage(file + " deleted");
                        }
                    }
                    catch (Exception ex)
                    {
                        AppendToMessage(ex.ToString());
                        Exceptions.Add(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                AppendToMessage(ex.ToString());
                Exceptions.Add(ex);
            }

            return (Exceptions.Count == 0);
        }
    }
}

