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
using Microsoft.Azure.Storage.Blob;
using STEM.Listing.Azure;

namespace STEM.Surge.Azure
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Delete")]
    [Description("Delete file(s) from 'Source Path' which can not be an expandable path.")]
    public class Delete : Instruction
    {

        [Category("Azure")]
        [DisplayName("Authentication"), DescriptionAttribute("The authentication configuration to be used.")]
        public Authentication Authentication { get; set; }

        [Category("Retry")]
        [DisplayName("Number of retries"), DescriptionAttribute("How many times should each operation be attempted?")]
        public int Retry { get; set; }

        [Category("Retry")]
        [DisplayName("Seconds between retries"), DescriptionAttribute("How many seconds should we wait between retries?")]
        public int RetryDelaySeconds { get; set; }

        [Category("Source")]
        [DisplayName("Source Path"), DescriptionAttribute("The location of the file(s) to be deleted.")]
        public string SourcePath { get; set; }

        [Category("Source")]
        [DisplayName("Expand Source Path"), DescriptionAttribute("When searching for files to delete, should the 'Source Path' be treated like an expandable?.")]
        public bool ExpandSource { get; set; }

        [Category("Source")]
        [DisplayName("File Filter"), DescriptionAttribute("The filter used on files in 'Source Path' to select files to delete. This can be a compound filter.")]
        public string FileFilter { get; set; }

        [Category("Source")]
        [DisplayName("Directory Filter"), DescriptionAttribute("The filter used on directories in 'Source Path' when 'Recurse Source' is specified. This can be a compound filter.")]
        public string DirectoryFilter { get; set; }

        [Category("Source")]
        [DisplayName("Recurse Source"), DescriptionAttribute("Recurse the source for files to delete?")]
        public bool RecurseSource { get; set; }
        
        [Category("Flow")]
        [DisplayName("Execution Mode"), Description("Should this be executed on forward InstructionSet execution or on Rollback? Consider the use case where you want to " +
            "move a file out of the flow to an error folder on Rollback.")]
        public ExecuteOn ExecutionMode { get; set; }

        public Delete()
            : base()
        {
            Authentication = new Authentication();

            Retry = 1;
            RetryDelaySeconds = 2;
            ExpandSource = false;
            SourcePath = "[TargetPath]";
            FileFilter = "[TargetName]";
            DirectoryFilter = "!TEMP";
            RecurseSource = false;
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
                string container = Authentication.ContainerFromPath(SourcePath);
                string prefix = Authentication.PrefixFromPath(SourcePath);

                List<IListBlobItem> items = Authentication.ListObjects(container, prefix, Sys.IO.Listing.ListingType.File, RecurseSource, DirectoryFilter, FileFilter);
                                
                foreach (IListBlobItem i in items)
                {
                    if (i is CloudBlockBlob)
                    {
                        try
                        {
                            string filename = Authentication.ToString(i);

                            Authentication.DeleteFile(filename);
                            AppendToMessage(filename + " deleted");
                        }
                        catch (Exception ex)
                        {
                            AppendToMessage(ex.ToString());
                            Exceptions.Add(ex);
                        }
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

