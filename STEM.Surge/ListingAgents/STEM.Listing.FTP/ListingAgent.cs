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
using STEM.Sys.Security;
using STEM.Sys.IO.Listing;

namespace STEM.Listing.FTP
{
    public class ListingAgent : STEM.Sys.IO.Listing.IListingAgent
    {
        Authentication _Auth = null;

        public ListingAgent(STEM.Sys.IO.Listing.IAuthentication authentication, ListingType listingType, string path, string fileFilter, string subpathFilter, bool recurse) : base(authentication, listingType, path, fileFilter, subpathFilter, recurse)
        {
            _Auth = authentication as Authentication;
        }

        protected override List<ListingEntry> GetListingEntries(ListingElements elements, out string message)
        {
            List<ListingEntry> ret = new List<ListingEntry>();

            Status = ListingAgentStatus.Listing;

            try
            {
                List<FluentFTP.FtpListItem> items = _Auth.ListDirectory(Path, ListingType, Recurse, SubpathFilter, FileFilter);

                foreach (FluentFTP.FtpListItem item in items)
                {
                    ItemType it = item.Type == FluentFTP.FtpFileSystemObjectType.File ? ItemType.File : ItemType.Directory;

                    if (elements == ListingElements.None)
                    {
                        ret.Add(new ListingEntry { Name = _Auth.ToString(item), ItemType = it });
                    }
                    else if (elements == ListingElements.LastWriteTimeUtc)
                    {
                        ret.Add(new ListingEntry { LastWriteTimeUtc = item.Modified, Name = _Auth.ToString(item), ItemType = it });
                    }
                    else if (elements == ListingElements.CreationTimeUtc)
                    {
                        ret.Add(new ListingEntry { CreationTimeUtc = item.Created, Name = _Auth.ToString(item), ItemType = it });
                    }
                    else if (elements == ListingElements.LastAccessTimeUtc)
                    {
                        ret.Add(new ListingEntry { LastAccessTimeUtc = item.Modified, Name = _Auth.ToString(item), ItemType = it });
                    }
                    else
                    {
                        ret.Add(new ListingEntry { CreationTimeUtc = item.Created, LastAccessTimeUtc = item.Modified, LastWriteTimeUtc = item.Modified, Size = item.Size, Name = _Auth.ToString(item), ItemType = it });
                    }
                }

                message = "";
            }
            catch (Exception ex)
            {
                Status = ListingAgentStatus.Error;
                message = ex.Message;
            }
            finally
            {
                if (Status == ListingAgentStatus.Listing)
                    Status = ListingAgentStatus.Waiting;
            }

            return ret;
        }
    }
}
