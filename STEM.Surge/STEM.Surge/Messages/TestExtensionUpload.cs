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
using System.Linq;
using STEM.Sys.Messaging;

namespace STEM.Surge.Messages
{
    public class TestExtensionUpload : STEM.Sys.Messaging.Message
    {
        public class Entry
        {
            public string TransformedFilename { get; set; }
            public DateTime LastModified { get; set; }
            
            public Entry()
            {
            }

            public Entry(string file)
            {
                TransformedFilename = STEM.Sys.Serialization.VersionManager.TransformFilename(file);
                LastModified = System.IO.File.GetLastWriteTimeUtc(file);
            }

            public void CopyFrom(Entry e)
            {
                TransformedFilename = e.TransformedFilename;
                LastModified = e.LastModified;
            }
        }

        public List<Entry> CandidateEntries { get; set; }
        public List<Entry> ExistingEntries { get; set; }

        public TestExtensionUpload()
        {
            CandidateEntries = new List<Entry>();
            ExistingEntries = new List<Entry>();
        }

        public TestExtensionUpload(List<string> files)
        {
            CandidateEntries = new List<Entry>();
            ExistingEntries = new List<Entry>();

            foreach (string e in files)
            {
                CandidateEntries.Add(new Entry(e));
            }
        }
    }
}