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

namespace STEM.Sys.IO.Listing
{
    public class ListingEntry : IDisposable
    {
        public ItemType ItemType { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public DateTime CreationTimeUtc { get; set; }
        public DateTime LastWriteTimeUtc { get; set; }
        public DateTime LastAccessTimeUtc { get; set; }

        public ListingEntry()
        {
            ItemType = ItemType.File;
            Name = "";
            Size = -1;
            CreationTimeUtc = DateTime.MinValue;
            LastWriteTimeUtc = DateTime.MinValue;
            LastAccessTimeUtc = DateTime.MinValue;
        }

        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("IListingEntry.Dispose", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose)
        {
        }
    }
}
