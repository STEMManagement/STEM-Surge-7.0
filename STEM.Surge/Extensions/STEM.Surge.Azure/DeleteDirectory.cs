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
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Azure.Storage.Blob;

namespace STEM.Surge.Azure
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Delete Directory")]
    [Description("Delete directories from 'Source Path' which can not be an expandable path.")]
    public class DeleteDirectory : Instruction
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
        [DisplayName("Source Path"), DescriptionAttribute("The location of the directories to be deleted.")]
        public string SourcePath { get; set; }

        [Category("Source")]
        [DisplayName("Expand Source Path"), DescriptionAttribute("When searching for directories to delete, should the 'Source Path' be treated like an expandable?.")]
        public bool ExpandSource { get; set; }

        [Category("Source")]
        [DisplayName("Directory Filter"), DescriptionAttribute("The filter used on directories in 'Source Path' when 'Recurse Source' is specified. This can be a compound filter.")]
        public string DirectoryFilter { get; set; }

        [Category("Source")]
        [DisplayName("Recurse Source"), DescriptionAttribute("Recurse the source for directories to delete?")]
        public bool RecurseSource { get; set; }

        [Category("Source")]
        [DisplayName("Delete Empty Directories Only"), DescriptionAttribute("Should empty directories be deleted after filtered files are deleted?")]
        public bool DeleteEmptyDirectoriesOnly { get; set; }

        [Category("Flow")]
        [DisplayName("Execution Mode"), Description("Should this be executed on forward InstructionSet execution or on Rollback? Consider the use case where you want to " +
            "delete a working directory on Rollback.")]
        public ExecuteOn ExecutionMode { get; set; }

        public DeleteDirectory()
            : base()
        {
            Authentication = new Authentication();

            Retry = 1;
            RetryDelaySeconds = 2;
            ExpandSource = false;
            SourcePath = "[TargetPath]";
            DirectoryFilter = "[TargetName]";
            DeleteEmptyDirectoriesOnly = true;
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

                List<IListBlobItem> items = Authentication.ListObjects(container, prefix, AzureListType.Directory, RecurseSource, DirectoryFilter, "*");

                foreach (IListBlobItem i in items)
                {
                    try
                    {
                        string p = Authentication.ToString(i);

                        List<IListBlobItem> remaining = Authentication.ListObjects(Authentication.ContainerFromPath(p), Authentication.PrefixFromPath(p), AzureListType.All, true, "*", "*");

                        if (DeleteEmptyDirectoriesOnly)
                        {
                            if (remaining.Count == 0)
                            {
                                Authentication.DeleteDirectory(p);
                                AppendToMessage(p + " deleted");
                            }
                        }
                        else
                        {
                            foreach (IListBlobItem del in remaining)
                            {
                                if (del is CloudBlockBlob)
                                    Authentication.DeleteFile(Authentication.ToString(del));
                            }

                            Authentication.DeleteDirectory(p);
                            AppendToMessage(p + " deleted");
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

