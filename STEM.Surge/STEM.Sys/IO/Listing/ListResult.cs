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

namespace STEM.Sys.IO.Listing
{
    public class ListResult : STEM.Sys.Serializable
    {
        public string ListingAgentType { get; set; }

        public ListingAgentStatus Status { get; set; }

        public string ListingAgentMessage { get; set; }

        public ListingType ListingType { get; private set; }

        public string Path { get; set; }

        public string FileFilter { get; set; }

        public string SubpathFilter { get; set; }

        public bool Recurse { get; set; }

        public List<ListingEntry> Entries { get; set; }

        public IAuthentication Authentication { get; set; }

        IListingAgent _Owner = null;

        public ListResult()
        {
        }

        public ListResult(IListingAgent owner)
        {
            _Owner = owner;

            ListingAgentType = _Owner.GetType().FullName;

            ListingType = _Owner.ListingType;
            Path = _Owner.Path;
            FileFilter = _Owner.FileFilter;
            SubpathFilter = _Owner.SubpathFilter;
            Recurse = _Owner.Recurse;

            Authentication = _Owner.Authentication;

            Entries = new List<ListingEntry>();
        }

        public void TrimPaths()
        {
            foreach (ListingEntry entry in Entries)
            {
                if (entry.Name.StartsWith(Path))
                    entry.Name = entry.Name.Substring(Path.Length);
            }
        }

        public void FullPaths()
        {
            foreach (ListingEntry entry in Entries)
            {
                if (!entry.Name.StartsWith(Path))
                    entry.Name = System.IO.Path.Combine(Path, entry.Name);
            }
        }
    }
}
