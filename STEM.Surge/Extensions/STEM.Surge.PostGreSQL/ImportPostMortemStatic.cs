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
using System.Data;
using System.Linq;
using System.ComponentModel;

namespace STEM.Surge.PostGreSQL
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("ImportPostMortemStatic")]
    [Description("Surge uses a PostGres database for PostMortem statistics. This instruction can be configured in a static InstructionSet to " +
        "watch a directory and ingest files into the PostMortem database. Files are deleted once ingested.")]

    public class ImportPostMortemStatic : ImportPostMortem
    {
        [DisplayName("Source Path"), DescriptionAttribute("The path of source files."), Category("Source")]
        public string SourcePath { get; set; }

        [DisplayName("File Filter"), DescriptionAttribute("File Filter"), Category("Source")]
        public string FileFilter { get; set; }

        [DisplayName("Directory Filter"), DescriptionAttribute("Directory Filter."), Category("Source")]
        public string DirectoryFilter { get; set; }

        [DisplayName("Search Option"), DescriptionAttribute("Search Option."), Category("Source")]
        public System.IO.SearchOption SearchOption { get; set; }

        [DisplayName("Maximum number of files to ingest at one time."), DescriptionAttribute("Maximum number of files to ingest at one time."), Category("Source")]
        public int IngestMax { get; set; }

        public ImportPostMortemStatic()
        {
            SourcePath = @"\\[BranchName]\STEM.Workforce\PostMortem";
            FileFilter = "*.is|*.dc";
            DirectoryFilter = "!TEMP";
            SearchOption = System.IO.SearchOption.TopDirectoryOnly;
            IngestMax = 30;
        }

        protected override void _Rollback()
        {
        }

        protected override bool _Run()
        {
            SourcePath = STEM.Sys.IO.Path.AdjustPath(SourcePath);

            try
            {
                List<string> files = new List<string>();
                DataTable iSet = Build_ISetTable();
                DataTable instructions = Build_InstructionTable();

                Random rnd = new Random();

                foreach (string file in STEM.Sys.IO.Directory.STEM_GetFiles(SourcePath, FileFilter, DirectoryFilter, SearchOption, true).OrderBy(i => rnd.Next()))
                {
                    string xml = System.IO.File.ReadAllText(file);

                    if (file.EndsWith(".is", StringComparison.InvariantCultureIgnoreCase))
                        IngestInstructionSet(xml, iSet, instructions);

                    files.Add(file);

                    if (files.Count == IngestMax)
                        break;
                }

                ImportDataTable(iSet, iSet.TableName);
                ImportDataTable(instructions, instructions.TableName);

                foreach (string file in files)
                    System.IO.File.Delete(file);
            }
            catch (Exception ex)
            {
                AppendToMessage(ex.Message);
                Exceptions.Add(ex);
            }

            return Exceptions.Count == 0;
        }
    }
}
