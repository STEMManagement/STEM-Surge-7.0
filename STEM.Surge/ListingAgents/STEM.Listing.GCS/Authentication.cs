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
using System.Reflection;
using System.Text.RegularExpressions;
using STEM.Sys.Security;
using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;
using STEM.Sys.IO.Listing;

namespace STEM.Listing.GCS
{
    public class Authentication : STEM.Sys.IO.Listing.IAuthentication
    {

        [Category("GCS")]
        [DisplayName("Storage Connection GoogleCredential JSON File"), DescriptionAttribute("Where is the GoogleCredential JSON file for authentication?")]
        public string GoogleCredentialFile { get; set; }

        [Category("GCS")]
        [DisplayName("Storage Project Name"), DescriptionAttribute("What is the GCS project name?")]
        public string ProjectName { get; set; }

        StorageClient _Client = null;

        [XmlIgnore]
        [Browsable(false)]
        public StorageClient Client
        {
            get
            {
                lock (this)
                {
                    if (_Client == null)
                    {
                        try
                        {
                            foreach (string gcFile in STEM.Sys.IO.Path.OrderPathsWithSubnet(GoogleCredentialFile, STEM.Sys.IO.Net.MachineIP()))
                            {
                                if (System.IO.File.Exists(gcFile))
                                {
                                    GoogleCredential cred = GoogleCredential.FromJson(System.IO.File.ReadAllText(gcFile));
                                    _Client = StorageClient.Create(cred);
                                    break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            STEM.Sys.EventLog.WriteEntry("GCS.Client", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                            throw ex;
                        }
                    }

                    return _Client;
                }
            }
        }

        public override void PopulateFrom(Sys.Security.IAuthentication source)
        {
            if (source.VersionDescriptor.TypeName == VersionDescriptor.TypeName)
            {
                PropertyInfo i = source.GetType().GetProperties().FirstOrDefault(p => p.Name == "GoogleCredentialFile");
                if (i != null)
                {
                    string k = i.GetValue(source) as string;
                    if (!String.IsNullOrEmpty(k) && String.IsNullOrEmpty(GoogleCredentialFile))
                        GoogleCredentialFile = k;
                }

                i = source.GetType().GetProperties().FirstOrDefault(p => p.Name == "ProjectName");
                if (i != null)
                {
                    string k = i.GetValue(source) as string;
                    if (!String.IsNullOrEmpty(k) && String.IsNullOrEmpty(ProjectName))
                        ProjectName = k;
                }
            }
            else
            {
                throw new Exception("IAuthentication Type mismatch.");
            }
        }

        protected override void Dispose(bool dispose)
        {
            lock (this)
            {
                if (_Client != null)
                    try
                    {
                        _Client.Dispose();
                    }
                    catch { }

                _Client = null;
            }
        }

        public static string ToString(Google.Apis.Storage.v1.Data.Object blob)
        {
            if (blob == null)
                return "";

            string ret = blob.Bucket + "/" + blob.Name;

            return System.IO.Path.DirectorySeparatorChar.ToString() + System.IO.Path.DirectorySeparatorChar.ToString() + ret.Replace('/', System.IO.Path.DirectorySeparatorChar);
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

        public Google.Apis.Storage.v1.Data.Object GetCloudBlockBlob(string filename)
        {
            try
            {
                string containerName = ContainerFromPath(filename);
                string prefix = PrefixFromPath(filename);

                Google.Apis.Storage.v1.Data.Object b = Client.GetObject(containerName, prefix);

                return b;
            }
            catch
            {
            }

            return null;
        }

        public List<Google.Apis.Storage.v1.Data.Object> ListObjects(string containerName, string directory, ListingType listType, bool recurse, string directoryFilter, string fileFilter)
        {
            List<Google.Apis.Storage.v1.Data.Object> ret = new List<Google.Apis.Storage.v1.Data.Object>();

            Regex inclusiveDirFilter = STEM.Sys.IO.Path.BuildInclusiveFilter(directoryFilter);
            Regex exclusiveDirFilter = STEM.Sys.IO.Path.BuildExclusiveFilter(directoryFilter);

            Regex inclusiveFileFilter = STEM.Sys.IO.Path.BuildInclusiveFilter(fileFilter);
            Regex exclusiveFileFilter = STEM.Sys.IO.Path.BuildExclusiveFilter(fileFilter);

            if (String.IsNullOrEmpty(containerName))
            {
                foreach (Google.Apis.Storage.v1.Data.Bucket cc in ListBuckets())
                    ret.AddRange(ListObjects(cc.Name, directory, listType, recurse, directoryFilter, fileFilter));

                return ret;
            }

            foreach (Google.Apis.Storage.v1.Data.Object blob in ListObjects(containerName, directory))
            {
                bool isDir = true;

                string path = ToString(blob);
                string dir = path;

                if (!path.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
                {
                    dir = STEM.Sys.IO.Path.GetDirectoryName(path);
                    isDir = false;
                }

                bool good = false;
                if (isDir)
                {
                    if (inclusiveDirFilter == null || inclusiveDirFilter.IsMatch(STEM.Sys.IO.Path.GetFileName(dir.TrimEnd('/')).ToUpper()))
                        if (exclusiveDirFilter == null || !exclusiveDirFilter.IsMatch(STEM.Sys.IO.Path.GetFileName(dir.TrimEnd('/')).ToUpper()))
                            good = true;
                }
                else
                {
                    if (inclusiveDirFilter == null || inclusiveDirFilter.IsMatch(STEM.Sys.IO.Path.GetFileName(dir.TrimEnd('/')).ToUpper()))
                        if (exclusiveDirFilter == null || !exclusiveDirFilter.IsMatch(STEM.Sys.IO.Path.GetFileName(dir.TrimEnd('/')).ToUpper()))
                            if (inclusiveFileFilter == null || inclusiveFileFilter.IsMatch(STEM.Sys.IO.Path.GetFileName(path).ToUpper()))
                                if (exclusiveFileFilter == null || !exclusiveFileFilter.IsMatch(STEM.Sys.IO.Path.GetFileName(path).ToUpper()))
                                    good = true;
                }

                if (!good)
                    continue;

                if (isDir)
                {
                    if (PrefixFromPath(path) != directory)
                    {
                        if (listType == ListingType.Directory || listType == ListingType.All)
                            ret.Add(blob);

                        if (recurse)
                            ret.AddRange(ListObjects(containerName, PrefixFromPath(path), listType, recurse, directoryFilter, fileFilter));
                    }
                }
                else if (!isDir && (listType == ListingType.File || listType == ListingType.All))
                {
                    if (directory == "" && !blob.Name.Contains("/"))
                    {
                        ret.Add(blob);
                    }
                    else if (directory != "")
                    {
                        string prefix = directory.Trim('/').Trim('\\');
                        prefix = prefix.Replace('\\', '/');
                        prefix = prefix.Trim('/') + '/';

                        if (!blob.Name.Replace(prefix, "").Contains("/"))
                            ret.Add(blob);
                    }
                }
            }

            return ret;
        }

        public List<Google.Apis.Storage.v1.Data.Bucket> ListBuckets()
        {
            List<Google.Apis.Storage.v1.Data.Bucket> containers = new List<Google.Apis.Storage.v1.Data.Bucket>();

            foreach (Google.Apis.Storage.v1.Data.Bucket o in Client.ListBuckets(ProjectName))
                containers.Add(o);

            return containers;
        }

        public List<Google.Apis.Storage.v1.Data.Object> ListObjects(Google.Apis.Storage.v1.Data.Bucket container)
        {
            return ListObjects(container.Name);
        }

        public List<Google.Apis.Storage.v1.Data.Object> ListObjects(string containerName, string directory)
        {
            List<Google.Apis.Storage.v1.Data.Object> blobs = new List<Google.Apis.Storage.v1.Data.Object>();

            string prefix = directory.Trim('/').Trim('\\');
            prefix = prefix.Replace('\\', '/');
            prefix = prefix.Trim('/');

            foreach (Google.Apis.Storage.v1.Data.Object o in Client.ListObjects(containerName, prefix))
                blobs.Add(o);

            return blobs;
        }

        public List<Google.Apis.Storage.v1.Data.Object> ListObjects(string containerName)
        {
            List<Google.Apis.Storage.v1.Data.Object> blobs = new List<Google.Apis.Storage.v1.Data.Object>();

            foreach (Google.Apis.Storage.v1.Data.Object o in Client.ListObjects(containerName))
                blobs.Add(o);

            return blobs;
        }

        public override IListingAgent ConstructListingAgent(ListingType listingType, string path, string fileFilter, string subpathFilter, bool recurse)
        {
            return new ListingAgent(this, listingType, path, fileFilter, subpathFilter, recurse);
        }

        public override void CreateDirectory(string directory)
        {
            try
            {
                string containerName = ContainerFromPath(directory);
                string dir = PrefixFromPath(directory);

                Google.Apis.Storage.v1.Data.Object o = new Google.Apis.Storage.v1.Data.Object();

                o.Bucket = containerName;
                o.Name = dir + "/";

                Client.UploadObject(o, new System.IO.MemoryStream());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void DeleteDirectory(string directory, bool recurse, bool deleteFiles)
        {
            try
            {
                if (DirectoryExists(directory))
                {
                    string containerName = ContainerFromPath(directory);
                    string prefix = PrefixFromPath(directory);

                    if (prefix == "")
                    {
                        Client.DeleteBucket(containerName);
                    }
                    else
                    {
                        Client.DeleteObject(containerName, prefix + "/");
                    }
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

                    Client.DeleteObject(containerName, prefix);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override bool DirectoryExists(string directory)
        {
            try
            {
                DirectoryInfo di = GetDirectoryInfo(directory);

                if (di != null)
                    return true;
            }
            catch
            {
            }

            return false;
        }

        public override bool FileExists(string file)
        {
            try
            {
                FileInfo fi = GetFileInfo(file);

                if (fi != null)
                    return true;
            }
            catch
            {
            }

            return false;
        }

        public override DirectoryInfo GetDirectoryInfo(string directory)
        {
            try
            {
                string containerName = ContainerFromPath(directory);
                string dir = PrefixFromPath(directory);

                if (String.IsNullOrEmpty(dir))
                {
                    Google.Apis.Storage.v1.Data.Bucket b = Client.GetBucket(containerName);

                    return new DirectoryInfo
                    {
                        CreationTimeUtc = b.TimeCreated.Value.ToUniversalTime(),
                        LastAccessTimeUtc = DateTime.UtcNow,
                        LastWriteTimeUtc = b.Updated.Value.ToUniversalTime(),
                    };
                }
                else
                {
                    Google.Apis.Storage.v1.Data.Object b = Client.GetObject(containerName, dir + "/");

                    if (b != null)
                        return new DirectoryInfo
                        {
                            CreationTimeUtc = b.TimeCreated.Value.ToUniversalTime(),
                            LastAccessTimeUtc = DateTime.UtcNow,
                            LastWriteTimeUtc = b.Updated.Value.ToUniversalTime(),
                        };
                }
            }
            catch
            {
            }

            return null;
        }

        public override FileInfo GetFileInfo(string file)
        {
            try
            {
                string containerName = ContainerFromPath(file);
                string prefix = PrefixFromPath(file);

                if (String.IsNullOrEmpty(prefix))
                    return null;

                Google.Apis.Storage.v1.Data.Object b = Client.GetObject(containerName, prefix);

                if (b != null)
                    return new FileInfo
                    {
                        CreationTimeUtc = b.TimeCreated.Value.ToUniversalTime(),
                        LastAccessTimeUtc = DateTime.UtcNow,
                        LastWriteTimeUtc = b.Updated.Value.ToUniversalTime(),
                        Size = (long)b.Size
                    };
            }
            catch
            {
            }

            return null;
        }
    }
}
