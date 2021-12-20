using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.S3;
using Amazon.S3.Model;
using STEM.Sys.Security;
using STEM.Sys.IO.Listing;

namespace STEM.Listing.S3
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

            string bucket = "";

            try
            {
                bucket = S3.Authentication.BucketFromPath(Path);
                string prefix = S3.Authentication.PrefixFromPath(Path);

                List<S3Object> files = _Auth.ListObjects(bucket, prefix, ListingType.File, Recurse, SubpathFilter, FileFilter);

                List<string> folders = files.Select(i => STEM.Sys.IO.Path.GetDirectoryName(i.Key).Replace("\\", "/").TrimEnd('/') + '/').Distinct().ToList();

                folders = folders.Select(i => System.IO.Path.Combine(System.IO.Path.DirectorySeparatorChar.ToString() + System.IO.Path.DirectorySeparatorChar.ToString() + bucket, i.Replace('/', System.IO.Path.DirectorySeparatorChar)).TrimEnd(System.IO.Path.DirectorySeparatorChar)).ToList();

                folders.Add(System.IO.Path.Combine(System.IO.Path.DirectorySeparatorChar.ToString() + System.IO.Path.DirectorySeparatorChar.ToString() + bucket));

                if (ListingType == ListingType.File || ListingType == ListingType.All)
                {
                    foreach (S3Object file in files)
                    {
                        if (elements == ListingElements.None)
                        {
                            ret.Add(new ListingEntry { Name = S3.Authentication.ToString(file), ItemType = ItemType.File });
                        }
                        else
                        {
                            FileInfo i = _Auth.GetFileInfo(S3.Authentication.ToString(file));

                            if (i != null)
                            {

                                if (elements == ListingElements.LastWriteTimeUtc)
                                {
                                    try
                                    {
                                        ret.Add(new ListingEntry { Name = S3.Authentication.ToString(file), ItemType = ItemType.File, LastWriteTimeUtc = i.LastWriteTimeUtc });
                                    }
                                    catch { }
                                }
                                else if (elements == ListingElements.CreationTimeUtc)
                                {
                                    try
                                    {
                                        ret.Add(new ListingEntry { Name = S3.Authentication.ToString(file), ItemType = ItemType.File, CreationTimeUtc = i.CreationTimeUtc });
                                    }
                                    catch { }
                                }
                                else if (elements == ListingElements.LastAccessTimeUtc)
                                {
                                    try
                                    {
                                        ret.Add(new ListingEntry { Name = S3.Authentication.ToString(file), ItemType = ItemType.File, LastAccessTimeUtc = i.LastAccessTimeUtc });
                                    }
                                    catch { }
                                }
                                else
                                {
                                    ret.Add(new ListingEntry { CreationTimeUtc = i.CreationTimeUtc, LastAccessTimeUtc = i.LastAccessTimeUtc, LastWriteTimeUtc = i.LastWriteTimeUtc, Size = i.Size, Name = S3.Authentication.ToString(file), ItemType = ItemType.File });
                                }
                            }
                        }
                    }
                }

                if (ListingType == ListingType.Directory || ListingType == ListingType.All)
                {
                    foreach (string folder in folders)
                    {
                        if (elements == ListingElements.None)
                        {
                            ret.Add(new ListingEntry { Name = folder, ItemType = ItemType.Directory });
                        }
                        else
                        {
                            DirectoryInfo i = _Auth.GetDirectoryInfo(folder);

                            if (i != null)
                            {

                                if (elements == ListingElements.LastWriteTimeUtc)
                                {
                                    try
                                    {
                                        ret.Add(new ListingEntry { Name = folder, ItemType = ItemType.Directory, LastWriteTimeUtc = i.LastWriteTimeUtc });
                                    }
                                    catch { }
                                }
                                else if (elements == ListingElements.CreationTimeUtc)
                                {
                                    try
                                    {
                                        ret.Add(new ListingEntry { Name = folder, ItemType = ItemType.Directory, CreationTimeUtc = i.CreationTimeUtc });
                                    }
                                    catch { }
                                }
                                else if (elements == ListingElements.LastAccessTimeUtc)
                                {
                                    try
                                    {
                                        ret.Add(new ListingEntry { Name = folder, ItemType = ItemType.Directory, LastAccessTimeUtc = i.LastAccessTimeUtc });
                                    }
                                    catch { }
                                }
                                else
                                {
                                    ret.Add(new ListingEntry { CreationTimeUtc = i.CreationTimeUtc, LastAccessTimeUtc = i.LastAccessTimeUtc, LastWriteTimeUtc = i.LastWriteTimeUtc, Name = folder, ItemType = ItemType.Directory });
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
