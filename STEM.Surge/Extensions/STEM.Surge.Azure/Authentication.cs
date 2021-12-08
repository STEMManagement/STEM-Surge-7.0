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
using System.ComponentModel; 
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using STEM.Sys.Security;
using Microsoft.Azure.Storage.Blob;

namespace STEM.Surge.Azure
{
    public enum AzureListType { File, Directory, All }

    public class Authentication : STEM.Surge.Azure.IAuthentication
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
                    if (_Client == null)
                        _Client = new CloudBlobContainer(new Uri(StorageConnectionString)).ServiceClient;

                    return _Client;
                }
            }
        }
        
        protected override void Dispose(bool dispose)
        {
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
                
                return Path.DirectorySeparatorChar.ToString() + Path.DirectorySeparatorChar.ToString() + ret.Trim('/').Replace('/', Path.DirectorySeparatorChar);
            }
            else if (blob is CloudBlobDirectory)
            {
                string ret = blob.Container.Uri.AbsolutePath + "/" + ((CloudBlobDirectory)blob).Prefix;
                
                return Path.DirectorySeparatorChar.ToString() + Path.DirectorySeparatorChar.ToString() + ret.Trim('/').Replace('/', Path.DirectorySeparatorChar);
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

        public List<IListBlobItem> ListObjects(string containerName, string directory, AzureListType listType, bool recurse, string directoryFilter, string fileFilter)
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
                            if (listType == AzureListType.File || listType == AzureListType.All)
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
                                    if (listType == AzureListType.Directory || listType == AzureListType.All)
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
                    if (listType == AzureListType.File || listType == AzureListType.All)
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
                            if (listType == AzureListType.Directory || listType == AzureListType.All)
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

        public bool FileExists(string file)
        {
            try
            {
                FDCFileInfo fi = GetFileInfo(file);

                if (fi != null)
                    return true;
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("Authentication.FileExists", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            return false;
        }

        public FDCFileInfo GetFileInfo(string file)
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
                    return new FDCFileInfo
                    {
                        CreationTimeUtc = b.Properties.Created.Value.UtcDateTime,
                        LastAccessTimeUtc = DateTime.UtcNow,
                        LastWriteTimeUtc = b.Properties.LastModified.Value.UtcDateTime,
                        Size = b.Properties.Length
                    };
            }
            catch (AggregateException)
            {
                //foreach (Exception e in ex.InnerExceptions)
                //    STEM.Sys.EventLog.WriteEntry("Authentication.GetFileInfo", e.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("Authentication.GetFileInfo", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            return null;
        }

        public bool DirectoryExists(string directory)
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
                
                List<IListBlobItem> list = ListObjects(containerName, dir, AzureListType.File, true, "*", "*");

                if (list.Count() > 0)
                    return true;
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("Authentication.DirectoryExists", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            return false;
        }


        public FDCDirectoryInfo GetDirectoryInfo(string directory)
        {
            try
            {
                string containerName = ContainerFromPath(directory);
                string dir = PrefixFromPath(directory);
                
                List<IListBlobItem> list = ListObjects(containerName, dir, AzureListType.File, true, "*", "*");
                
                if (list.Count() == 0)
                    return null;

                DateTime ct = list.Select(i => i as CloudBlockBlob).Min(i => i.Properties.Created.Value.UtcDateTime);
                DateTime lwt = list.Select(i => i as CloudBlockBlob).Max(i => i.Properties.LastModified.Value.UtcDateTime);

                return new FDCDirectoryInfo
                {
                    CreationTimeUtc = ct,
                    LastAccessTimeUtc = DateTime.UtcNow,
                    LastWriteTimeUtc = lwt,
                };
            }
            catch (AggregateException)
            {
                //foreach (Exception e in ex.InnerExceptions)
                //    STEM.Sys.EventLog.WriteEntry("Authentication.GetDirectoryInfo", e.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("Authentication.GetDirectoryInfo", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            return null;
        }

        public void CreateDirectory(string directory)
        {
            // Directories exist only when blobs exist within them

            // Verify that the Container exists

            try
            {
                string containerName = ContainerFromPath(directory);

                CloudBlobContainer container = Client.GetContainerReference(containerName);

                container.CreateIfNotExists();
            }
            catch { }
        }

        public void DeleteDirectory(string directory)
        {
            // Directories exist only when blobs exist within them so delete all blobs, but not here

            string containerName = ContainerFromPath(directory);
            string prefix = PrefixFromPath(directory);

            if (prefix == "") // Delete Container
            {
                CloudBlobContainer container = Client.GetContainerReference(containerName);

                container.DeleteIfExists();
            }
        }

        public void DeleteFile(string filename)
        {
            string containerName = ContainerFromPath(filename);
            string prefix = PrefixFromPath(filename);

            CloudBlobContainer container = Client.GetContainerReference(containerName);
            CloudBlobDirectory b = container.GetDirectoryReference(STEM.Sys.IO.Path.GetDirectoryName(prefix));

            b.GetBlobReference(STEM.Sys.IO.Path.GetFileName(prefix)).DeleteIfExists();
        }

        public string UniqueFilename(string filename)
        {
            string unique = filename;

            int cnt = 1;

            while (FileExists(unique))
            {
                unique = string.Format("{0}" + System.IO.Path.DirectorySeparatorChar + "{1}_{2}{3}",
                    STEM.Sys.IO.Path.GetDirectoryName(filename),
                    STEM.Sys.IO.Path.GetFileNameWithoutExtension(filename),
                    (cnt++).ToString("0000"),
                    STEM.Sys.IO.Path.GetExtension(filename));
            }

            return unique;
        }
    }
}