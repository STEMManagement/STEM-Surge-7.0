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
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using STEM.Sys.Security;
using Microsoft.Azure.Storage.Blob;

namespace STEM.Listing.Azure
{
    public class Authentication : STEM.Sys.IO.Listing.IAuthentication
    {
        [Category("Azure")]
        [DisplayName("Storage Connection String"), DescriptionAttribute("What is the Azure connection string?")]
        [XmlIgnore]
        [PasswordPropertyText(true)]
        public string StorageConnectionString { get; set; }
        [Browsable(false)]
        public string StorageConnectionStringEncoded
        {
            get
            {
                return this.Entangle(StorageConnectionString);
            }

            set
            {
                StorageConnectionString = this.Detangle(value);
            }
        }

        CloudBlobClient _Client = null;

        [XmlIgnore]
        [Browsable(false)]
        public CloudBlobClient Client
        {
            get
            {
                lock (this)
                {
                    try
                    {
                        if (_Client == null)
                            _Client = new CloudBlobContainer(new Uri(StorageConnectionString)).ServiceClient;
                    }
                    catch (Exception ex)
                    {
                        STEM.Sys.EventLog.WriteEntry("Azure.Authentication.Client", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                        throw ex;
                    }

                    return _Client;
                }
            }
        }

        public Authentication()
        {
            StorageConnectionString = "";
        }

        public override void PopulateFrom(Sys.Security.IAuthentication source)
        {
            if (source.VersionDescriptor.TypeName == VersionDescriptor.TypeName)
            {
                PropertyInfo i = source.GetType().GetProperties().FirstOrDefault(p => p.Name == "StorageConnectionString");
                if (i != null)
                {
                    string k = i.GetValue(source) as string;
                    if (!String.IsNullOrEmpty(k) && String.IsNullOrEmpty(StorageConnectionString))
                        StorageConnectionString = k;
                }
            }
            else
            {
                throw new Exception("IAuthentication Type mismatch.");
            }
        }
        public override STEM.Sys.IO.Listing.IListingAgent ConstructListingAgent(STEM.Sys.IO.Listing.ListingType listingType, string path, string fileFilter, string subpathFilter, bool recurse)
        {
            return new ListingAgent(this, listingType, path, fileFilter, subpathFilter, recurse);
        }

        protected override void Dispose(bool dispose)
        {
            base.Dispose(dispose);

            lock (this)
            {
                _Client = null;
            }
        }

        public static string ToString(IListBlobItem blob)
        {
            if (blob == null)
                return "";

            if (blob is CloudBlockBlob)
            {
                string ret = blob.Container.Uri.AbsolutePath + "/" + ((CloudBlockBlob)blob).Name;

                return System.IO.Path.DirectorySeparatorChar.ToString() + System.IO.Path.DirectorySeparatorChar.ToString() + ret.Trim('/').Replace('/', System.IO.Path.DirectorySeparatorChar);
            }
            else if (blob is CloudBlobDirectory)
            {
                string ret = blob.Container.Uri.AbsolutePath + "/" + ((CloudBlobDirectory)blob).Prefix;

                return System.IO.Path.DirectorySeparatorChar.ToString() + System.IO.Path.DirectorySeparatorChar.ToString() + ret.Trim('/').Replace('/', System.IO.Path.DirectorySeparatorChar);
            }

            return "";
        }

        public static string ContainerFromPath(string path)
        {
            return STEM.Sys.IO.Path.FirstTokenOfPath(path).ToLower();
        }

        public static string PrefixFromPath(string path)
        {
            string container = ContainerFromPath(path);

            string prefix = path.Trim('/').Trim('\\');

            prefix = prefix.Substring(container.Length);
            prefix = prefix.Replace('\\', '/');
            return prefix.Trim('/');
        }

        public CloudBlockBlob GetCloudBlockBlob(string filename, bool createIfNotExists)
        {
            try
            {
                string containerName = ContainerFromPath(filename);
                string prefix = PrefixFromPath(filename);

                CloudBlobContainer container = Client.GetContainerReference(containerName);

                CloudBlockBlob b = container.GetDirectoryReference(STEM.Sys.IO.Path.GetDirectoryName(prefix)).GetBlockBlobReference(STEM.Sys.IO.Path.GetFileName(prefix));

                try
                {
                    System.Threading.Tasks.Task t = b.FetchAttributesAsync();
                    t.Wait();
                }
                catch
                {
                    if (!createIfNotExists)
                        return null;
                }

                return b;
            }
            catch
            {
            }

            return null;
        }

        public List<IListBlobItem> ListObjects(string containerName, string directory, Sys.IO.Listing.ListingType listType, bool recurse, string directoryFilter, string fileFilter)
        {
            List<IListBlobItem> ret = new List<IListBlobItem>();

            Regex inclusiveDirFilter = STEM.Sys.IO.Path.BuildInclusiveFilter(directoryFilter);
            Regex exclusiveDirFilter = STEM.Sys.IO.Path.BuildExclusiveFilter(directoryFilter);

            Regex inclusiveFileFilter = STEM.Sys.IO.Path.BuildInclusiveFilter(fileFilter);
            Regex exclusiveFileFilter = STEM.Sys.IO.Path.BuildExclusiveFilter(fileFilter);

            if (String.IsNullOrEmpty(containerName))
            {
                foreach (CloudBlobContainer cc in ListContainers())
                {
                    foreach (IListBlobItem blob in ListBlobs(cc))
                    {
                        if (blob is CloudBlockBlob)
                        {
                            if (listType == Sys.IO.Listing.ListingType.File || listType == Sys.IO.Listing.ListingType.All)
                            {
                                string path = ToString(blob);
                                string dir = STEM.Sys.IO.Path.GetDirectoryName(path);

                                if (inclusiveDirFilter == null || inclusiveDirFilter.IsMatch(STEM.Sys.IO.Path.GetFileName(dir.TrimEnd('/')).ToUpper()))
                                    if (exclusiveDirFilter == null || !exclusiveDirFilter.IsMatch(STEM.Sys.IO.Path.GetFileName(dir.TrimEnd('/')).ToUpper()))
                                        if (inclusiveFileFilter == null || inclusiveFileFilter.IsMatch(STEM.Sys.IO.Path.GetFileName(path).ToUpper()))
                                            if (exclusiveFileFilter == null || !exclusiveFileFilter.IsMatch(STEM.Sys.IO.Path.GetFileName(path).ToUpper()))
                                            {
                                                ret.Add(blob);
                                            }
                            }
                        }
                        else if (blob is CloudBlobDirectory)
                        {
                            string dir = ToString(blob);

                            if (inclusiveDirFilter == null || inclusiveDirFilter.IsMatch(STEM.Sys.IO.Path.GetFileName(dir.TrimEnd('/')).ToUpper()))
                                if (exclusiveDirFilter == null || !exclusiveDirFilter.IsMatch(STEM.Sys.IO.Path.GetFileName(dir.TrimEnd('/')).ToUpper()))
                                {
                                    if (listType == Sys.IO.Listing.ListingType.Directory || listType == Sys.IO.Listing.ListingType.All)
                                        ret.Add(blob);

                                    if (recurse)
                                    {
                                        string p = ToString(blob);
                                        ret.AddRange(ListObjects(ContainerFromPath(p), PrefixFromPath(p), listType, recurse, directoryFilter, fileFilter));
                                    }
                                }
                        }
                    }
                }

                return ret;
            }

            foreach (IListBlobItem blob in ListBlobs(containerName, directory))
            {
                if (blob is CloudBlockBlob)
                {
                    if (listType == Sys.IO.Listing.ListingType.File || listType == Sys.IO.Listing.ListingType.All)
                    {
                        string path = ToString(blob);
                        string dir = STEM.Sys.IO.Path.GetDirectoryName(path);

                        if (inclusiveDirFilter == null || inclusiveDirFilter.IsMatch(STEM.Sys.IO.Path.GetFileName(dir.TrimEnd('/')).ToUpper()))
                            if (exclusiveDirFilter == null || !exclusiveDirFilter.IsMatch(STEM.Sys.IO.Path.GetFileName(dir.TrimEnd('/')).ToUpper()))
                                if (inclusiveFileFilter == null || inclusiveFileFilter.IsMatch(STEM.Sys.IO.Path.GetFileName(path).ToUpper()))
                                    if (exclusiveFileFilter == null || !exclusiveFileFilter.IsMatch(STEM.Sys.IO.Path.GetFileName(path).ToUpper()))
                                    {
                                        ret.Add(blob);
                                    }
                    }
                }
                else if (blob is CloudBlobDirectory)
                {
                    string dir = ToString(blob);

                    if (inclusiveDirFilter == null || inclusiveDirFilter.IsMatch(STEM.Sys.IO.Path.GetFileName(dir.TrimEnd('/')).ToUpper()))
                        if (exclusiveDirFilter == null || !exclusiveDirFilter.IsMatch(STEM.Sys.IO.Path.GetFileName(dir.TrimEnd('/')).ToUpper()))
                        {
                            if (listType == Sys.IO.Listing.ListingType.Directory || listType == Sys.IO.Listing.ListingType.All)
                                ret.Add(blob);

                            if (recurse)
                            {
                                string p = ToString(blob);
                                ret.AddRange(ListObjects(ContainerFromPath(p), PrefixFromPath(p), listType, recurse, directoryFilter, fileFilter));
                            }
                        }
                }
            }

            return ret;
        }

        public List<CloudBlobContainer> ListContainers()
        {
            BlobContinuationToken continuationToken = null;
            List<CloudBlobContainer> containers = new List<CloudBlobContainer>();
            do
            {
                System.Threading.Tasks.Task<ContainerResultSegment> response = Client.ListContainersSegmentedAsync(continuationToken);
                response.Wait();

                continuationToken = response.Result.ContinuationToken;
                containers.AddRange(response.Result.Results);
            }
            while (continuationToken != null);

            return containers;
        }

        public List<IListBlobItem> ListBlobs(CloudBlobContainer container)
        {
            List<IListBlobItem> blobs = new List<IListBlobItem>();

            BlobContinuationToken continuationToken = null;
            do
            {
                System.Threading.Tasks.Task<BlobResultSegment> r = container.ListBlobsSegmentedAsync(continuationToken);
                r.Wait();

                continuationToken = r.Result.ContinuationToken;
                blobs.AddRange(r.Result.Results);
            }
            while (continuationToken != null);

            return blobs;
        }

        public List<IListBlobItem> ListBlobs(string containerName, string directory)
        {
            CloudBlobContainer container = Client.GetContainerReference(containerName);
            CloudBlobDirectory dir = container.GetDirectoryReference(directory);

            List<IListBlobItem> blobs = new List<IListBlobItem>();

            BlobContinuationToken continuationToken = null;
            do
            {
                System.Threading.Tasks.Task<BlobResultSegment> r = dir.ListBlobsSegmentedAsync(continuationToken);
                r.Wait();

                continuationToken = r.Result.ContinuationToken;
                blobs.AddRange(r.Result.Results);
            }
            while (continuationToken != null);

            return blobs;
        }

        public List<IListBlobItem> ListBlobs(string containerName)
        {
            CloudBlobContainer container = Client.GetContainerReference(containerName);

            List<IListBlobItem> blobs = new List<IListBlobItem>();

            BlobContinuationToken continuationToken = null;
            do
            {
                System.Threading.Tasks.Task<BlobResultSegment> r = container.ListBlobsSegmentedAsync(continuationToken);
                r.Wait();

                continuationToken = r.Result.ContinuationToken;
                blobs.AddRange(r.Result.Results);
            }
            while (continuationToken != null);

            return blobs;
        }

        public override bool FileExists(string file)
        {
            try
            {
                Sys.IO.Listing.FileInfo fi = GetFileInfo(file);

                if (fi != null)
                    return true;
            }
            catch
            {
            }

            return false;
        }

        public override Sys.IO.Listing.FileInfo GetFileInfo(string file)
        {
            try
            {
                string containerName = ContainerFromPath(file);
                string prefix = PrefixFromPath(file);

                if (String.IsNullOrEmpty(prefix))
                    return null;

                CloudBlobContainer container = Client.GetContainerReference(containerName);

                CloudBlockBlob b = container.GetDirectoryReference(STEM.Sys.IO.Path.GetDirectoryName(prefix)).GetBlockBlobReference(STEM.Sys.IO.Path.GetFileName(prefix));

                try
                {
                    System.Threading.Tasks.Task t = b.FetchAttributesAsync();
                    t.Wait();
                }
                catch
                {
                    b = null;
                }

                if (b != null)
                    return new Sys.IO.Listing.FileInfo
                    {
                        CreationTimeUtc = b.Properties.Created.Value.UtcDateTime,
                        LastAccessTimeUtc = DateTime.UtcNow,
                        LastWriteTimeUtc = b.Properties.LastModified.Value.UtcDateTime,
                        Size = b.Properties.Length
                    };
            }
            catch 
            {
            }

            return null;
        }

        public override bool DirectoryExists(string directory)
        {
            try
            {
                // In Azure a directory can exist when a blob exists within it
                // so directories always exist
                // Check to be sure this isn't a blob and if it's not just return true


                string containerName = ContainerFromPath(directory);

                string dir = PrefixFromPath(directory);

                if (dir == "")
                    return true;

                List<IListBlobItem> list = ListObjects(containerName, dir, Sys.IO.Listing.ListingType.File, true, "*", "*");

                if (list.Count > 0)
                    return true;
            }
            catch
            {
            }

            return false;
        }


        public override Sys.IO.Listing.DirectoryInfo GetDirectoryInfo(string directory)
        {
            try
            {
                string containerName = ContainerFromPath(directory);
                string dir = PrefixFromPath(directory);

                List<IListBlobItem> list = ListObjects(containerName, dir, Sys.IO.Listing.ListingType.File, true, "*", "*");

                if (list.Count == 0)
                    return null;

                DateTime ct = list.Select(i => i as CloudBlockBlob).Min(i => i.Properties.Created.Value.UtcDateTime);
                DateTime lwt = list.Select(i => i as CloudBlockBlob).Max(i => i.Properties.LastModified.Value.UtcDateTime);

                return new Sys.IO.Listing.DirectoryInfo
                {
                    CreationTimeUtc = ct,
                    LastAccessTimeUtc = DateTime.UtcNow,
                    LastWriteTimeUtc = lwt,
                };
            }
            catch
            {
            }

            return null;
        }

        public override void CreateDirectory(string directory)
        {
            // Directories exist only when blobs exist within them

            // Verify that the Container exists?

            try
            {
                //string containerName = ContainerFromPath(directory);

                //CloudBlobContainer container = Client.GetContainerReference(containerName);

                //container.CreateIfNotExists();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void DeleteDirectory(string directory, bool recurse, bool deleteFiles)
        {
            // Directories exist only when blobs exist within them so delete all blobs, but not here

            try
            {
                string containerName = ContainerFromPath(directory);
                string prefix = PrefixFromPath(directory);

                if (prefix == "") // Delete Container
                {
                    CloudBlobContainer container = Client.GetContainerReference(containerName);

                    container.DeleteIfExists();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void DeleteFile(string file)
        {
            try
            {
                if (FileExists(file))
                {
                    string containerName = ContainerFromPath(file);
                    string prefix = PrefixFromPath(file);

                    CloudBlobContainer container = Client.GetContainerReference(containerName);
                    CloudBlobDirectory b = container.GetDirectoryReference(STEM.Sys.IO.Path.GetDirectoryName(prefix));

                    b.GetBlobReference(STEM.Sys.IO.Path.GetFileName(prefix)).DeleteIfExists();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}