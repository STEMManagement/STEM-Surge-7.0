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
using System.Linq;
using System.Xml.Linq;
using System.ComponentModel;
using STEM.Surge;

namespace STEM.Surge.ErrorCleaner
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Error Cleaner")]
    [Description("Intended to run in a static InstructionSet, this Instruction looks in the InstructionSet Errors folder of the Branch Manager for error files matching " +
        "the configured criteria and deletes the files.")]
    public class ErrorCleaner : Instruction
    {
        [Category("Criteria")]
        [DisplayName("Error Age (Minutes)")]
        public int MinutesToAge { get; set; }

        [Category("Criteria")]
        [DisplayName("Process Name Contains...")]
        public string ProcessNameContains { get; set; }

        [Category("Criteria")]
        [DisplayName("Message Contains...")]
        public string MessageContains { get; set; }
        
        public ErrorCleaner() : base()
        {
            ProcessNameContains = "";
            MessageContains = "";
            MinutesToAge = 1;
        }

        protected override bool _Run()
        {
            try
            {
                string errDir = Path.Combine(Environment.CurrentDirectory, "Errors");

                if (Directory.Exists(errDir))
                {
                    foreach (string file in Directory.GetFiles(errDir, "*.is"))
                    {
                        if ((DateTime.UtcNow - File.GetLastWriteTimeUtc(file)).TotalMinutes > MinutesToAge)
                            try
                            {
                                XDocument doc = XDocument.Parse(File.ReadAllText(file));

                                if (!String.IsNullOrEmpty(ProcessNameContains))
                                {
                                    XElement pn = doc.Root.Descendants().Where(i => i.Name.LocalName == "ProcessName").FirstOrDefault();

                                    if (pn == null || !pn.Value.ToUpper().Contains(ProcessNameContains.ToUpper()))
                                        continue;
                                }

                                foreach (XElement e in doc.Root.Descendants().Where(i => i.Name.LocalName == "Message"))
                                {
                                    if (e.Value.ToUpper().Contains(MessageContains.ToUpper()))
                                    {
                                        File.Delete(file);
                                        break;
                                    }
                                }
                            }
                            catch { }
                    }
                }
            }
            catch { }

            return true;
        }

        protected override void _Rollback()
        {
        }
    }
}
