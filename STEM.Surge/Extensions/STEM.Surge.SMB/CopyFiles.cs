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
using System.Collections.Generic;
using System.IO;

namespace STEM.Surge.SMB
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Copy Files")]
    [Description("Copy a set of files to 'Destination Path' where Destination Path can be an expandable path.")]
    public class CopyFiles : CopyBase
    {
        [Category("Source")]
        [DisplayName("File List (Property Name: FileList)"), DescriptionAttribute("List<string> Property for the GroupingController to populate.")]
        public List<string> FileList { get; set; }

        public CopyFiles()
            : base()
        {
            Action = ActionType.Copy;
            FileList = new List<string>();
        }

        protected override bool _Run()
        {
            List<Exception> exceptions = new List<Exception>();
            STEM.Sys.Serialization.Dictionary<string, string> fileSizes = new Sys.Serialization.Dictionary<string, string>();
            List<string> filesActioned = new List<string>();

            if (PopulatePostMortemMeta)
            {
                PostMortemMetaData["FilesRequested"] = STEM.Sys.Serializable.Serialize(FileList);
            }

            foreach (string file in FileList)
            {
                if (!File.Exists(file))
                    continue;

                CB_SourcePath = Path.GetDirectoryName(file);
                CB_ExpandSource = false;
                CB_FileFilter = Path.GetFileName(file);
                CB_DirectoryFilter = "*";
                CB_RecurseSource = false;

                base._Run();

                if (PopulatePostMortemMeta)
                    if (Exceptions.Count == 0)
                    {
                        if (PostMortemMetaData.ContainsKey("FileSize"))
                        {
                            string fs = PostMortemMetaData["FileSize"];

                            fileSizes[file] = fs;
                        }

                        filesActioned.Add(file);
                    }

                if (PostMortemMetaData.ContainsKey("FileSize"))
                    PostMortemMetaData.Remove("FileSize");

                if (PostMortemMetaData.ContainsKey("FilesActioned"))
                    PostMortemMetaData.Remove("FilesActioned");

                exceptions.AddRange(Exceptions);
                Exceptions.Clear();
            }

            if (PopulatePostMortemMeta)
            {
                PostMortemMetaData["FilesActioned"] = STEM.Sys.Serializable.Serialize(filesActioned);
                PostMortemMetaData["FileSizes"] = STEM.Sys.Serializable.Serialize(fileSizes);
            }

            if (exceptions.Count > 0)
            {
                Exceptions.AddRange(exceptions);
            }

            return Exceptions.Count == 0;
        }
    }
}