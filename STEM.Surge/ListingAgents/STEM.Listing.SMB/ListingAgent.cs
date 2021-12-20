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

namespace STEM.Listing.SMB
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
            Impersonation token = null;

            Status = ListingAgentStatus.Listing;

            try
            {
                if (_Auth != null)
                    _Auth.Impersonate(out token);

                if (ListingType == ListingType.File || ListingType == ListingType.All)
                {
                    List<string> files = STEM.Sys.IO.Directory.STEM_GetFiles(Path, FileFilter, SubpathFilter, Recurse ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly, false);

                    foreach (string file in files)
                    {
                        if (elements == ListingElements.None)
                        {
                            ret.Add(new ListingEntry { Name = file, ItemType = ItemType.File });
                        }
                        else if (elements == ListingElements.LastWriteTimeUtc)
                        {
                            try
                            {
                                ret.Add(new ListingEntry { Name = file, ItemType = ItemType.File, LastWriteTimeUtc = System.IO.File.GetLastWriteTimeUtc(file) });
                            }
                            catch { }
                        }
                        else if (elements == ListingElements.CreationTimeUtc)
                        {
                            try
                            {
                                ret.Add(new ListingEntry { Name = file, ItemType = ItemType.File, CreationTimeUtc = System.IO.File.GetCreationTimeUtc(file) });
                            }
                            catch { }
                        }
                        else if (elements == ListingElements.LastAccessTimeUtc)
                        {
                            try
                            {
                                ret.Add(new ListingEntry { Name = file, ItemType = ItemType.File, LastAccessTimeUtc = System.IO.File.GetLastAccessTimeUtc(file) });
                            }
                            catch { }
                        }
                        else
                        {
                            try
                            {
                                System.IO.FileInfo i = new System.IO.FileInfo(file);
                                ret.Add(new ListingEntry { CreationTimeUtc = i.CreationTimeUtc, LastAccessTimeUtc = i.LastAccessTimeUtc, LastWriteTimeUtc = i.LastWriteTimeUtc, Size = i.Length, Name = file, ItemType = ItemType.File });
                            }
                            catch { }
                        }
                    }
                }

                if (ListingType == ListingType.Directory || ListingType == ListingType.All)
                {
                    List<string> directories = STEM.Sys.IO.Directory.STEM_GetDirectories(Path, SubpathFilter, Recurse ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly, false);

                    foreach (string dir in directories)
                    {
                        if (elements == ListingElements.None)
                        {
                            ret.Add(new ListingEntry { Name = dir });
                        }
                        else if (elements == ListingElements.LastWriteTimeUtc)
                        {
                            try
                            {
                                ret.Add(new ListingEntry { Name = dir, ItemType = ItemType.Directory, LastWriteTimeUtc = System.IO.Directory.GetLastWriteTimeUtc(dir) });
                            }
                            catch { }
                        }
                        else if (elements == ListingElements.CreationTimeUtc)
                        {
                            try
                            {
                                ret.Add(new ListingEntry { Name = dir, ItemType = ItemType.Directory, CreationTimeUtc = System.IO.Directory.GetCreationTimeUtc(dir) });
                            }
                            catch { }
                        }
                        else if (elements == ListingElements.LastAccessTimeUtc)
                        {
                            try
                            {
                                ret.Add(new ListingEntry { Name = dir, ItemType = ItemType.Directory, LastAccessTimeUtc = System.IO.Directory.GetLastAccessTimeUtc(dir) });
                            }
                            catch { }
                        }
                        else
                        {
                            try
                            {
                                System.IO.DirectoryInfo i = new System.IO.DirectoryInfo(dir);
                                ret.Add(new ListingEntry { CreationTimeUtc = i.CreationTimeUtc, LastAccessTimeUtc = i.LastAccessTimeUtc, LastWriteTimeUtc = i.LastWriteTimeUtc, Name = dir, ItemType = ItemType.Directory });
                            }
                            catch { }
                        }
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

                if (_Auth != null)
                    _Auth.Unimpersonate(token);
            }

            return ret;
        }
    }
}
