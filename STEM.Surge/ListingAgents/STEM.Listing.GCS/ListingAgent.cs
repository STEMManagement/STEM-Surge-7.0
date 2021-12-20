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

namespace STEM.Listing.GCS
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

            string container = "";

            try
            {
                container = GCS.Authentication.ContainerFromPath(Path);
                string directory = GCS.Authentication.PrefixFromPath(Path);

                if (ListingType == ListingType.File || ListingType == ListingType.All)
                {
                    List<Google.Apis.Storage.v1.Data.Object> files = _Auth.ListObjects(container, directory, ListingType.File, Recurse, SubpathFilter, FileFilter);

                    foreach (Google.Apis.Storage.v1.Data.Object file in files)
                    {
                        if (elements == ListingElements.None)
                        {
                            ret.Add(new ListingEntry { Name = GCS.Authentication.ToString(file), ItemType = ItemType.File });
                        }
                        else
                        {
                            FileInfo i = _Auth.GetFileInfo(GCS.Authentication.ToString(file));

                            if (i != null)
                            {

                                if (elements == ListingElements.LastWriteTimeUtc)
                                {
                                    try
                                    {
                                        ret.Add(new ListingEntry { Name = GCS.Authentication.ToString(file), ItemType = ItemType.File, LastWriteTimeUtc = i.LastWriteTimeUtc });
                                    }
                                    catch { }
                                }
                                else if (elements == ListingElements.CreationTimeUtc)
                                {
                                    try
                                    {
                                        ret.Add(new ListingEntry { Name = GCS.Authentication.ToString(file), ItemType = ItemType.File, CreationTimeUtc = i.CreationTimeUtc });
                                    }
                                    catch { }
                                }
                                else if (elements == ListingElements.LastAccessTimeUtc)
                                {
                                    try
                                    {
                                        ret.Add(new ListingEntry { Name = GCS.Authentication.ToString(file), ItemType = ItemType.File, LastAccessTimeUtc = i.LastAccessTimeUtc });
                                    }
                                    catch { }
                                }
                                else
                                {
                                    ret.Add(new ListingEntry { CreationTimeUtc = i.CreationTimeUtc, LastAccessTimeUtc = i.LastAccessTimeUtc, LastWriteTimeUtc = i.LastWriteTimeUtc, Size = i.Size, Name = GCS.Authentication.ToString(file), ItemType = ItemType.File });
                                }
                            }
                        }
                    }
                }

                if (ListingType == ListingType.Directory || ListingType == ListingType.All)
                {
                    List<Google.Apis.Storage.v1.Data.Object> directories = _Auth.ListObjects(container, directory, ListingType.Directory, Recurse, SubpathFilter, "*");

                    foreach (Google.Apis.Storage.v1.Data.Object dir in directories)
                    {
                        if (elements == ListingElements.None)
                        {
                            ret.Add(new ListingEntry { Name = GCS.Authentication.ToString(dir), ItemType = ItemType.Directory });
                        }
                        else
                        {
                            DirectoryInfo i = _Auth.GetDirectoryInfo(GCS.Authentication.ToString(dir));

                            if (i != null)
                            {

                                if (elements == ListingElements.LastWriteTimeUtc)
                                {
                                    try
                                    {
                                        ret.Add(new ListingEntry { Name = GCS.Authentication.ToString(dir), ItemType = ItemType.Directory, LastWriteTimeUtc = i.LastWriteTimeUtc });
                                    }
                                    catch { }
                                }
                                else if (elements == ListingElements.CreationTimeUtc)
                                {
                                    try
                                    {
                                        ret.Add(new ListingEntry { Name = GCS.Authentication.ToString(dir), ItemType = ItemType.Directory, CreationTimeUtc = i.CreationTimeUtc });
                                    }
                                    catch { }
                                }
                                else if (elements == ListingElements.LastAccessTimeUtc)
                                {
                                    try
                                    {
                                        ret.Add(new ListingEntry { Name = GCS.Authentication.ToString(dir), ItemType = ItemType.Directory, LastAccessTimeUtc = i.LastAccessTimeUtc });
                                    }
                                    catch { }
                                }
                                else
                                {
                                    ret.Add(new ListingEntry { CreationTimeUtc = i.CreationTimeUtc, LastAccessTimeUtc = i.LastAccessTimeUtc, LastWriteTimeUtc = i.LastWriteTimeUtc, Name = GCS.Authentication.ToString(dir), ItemType = ItemType.Directory });
                                }
                            }
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
            }

            return ret;
        }
    }
}
