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
using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;
using System.Threading;

namespace STEM.Surge.GCS
{
    public enum GcsListType { File, Directory, All }

    public class Authentication : IAuthentication
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
                        foreach (string gcFile in STEM.Sys.IO.Path.OrderPathsWithSubnet(GoogleCredentialFile, STEM.Sys.IO.Net.MachineIP()))
                        {
                            if (File.Exists(gcFile))
                            {
                                GoogleCredential cred = GoogleCredential.FromJson(File.ReadAllText(gcFile));
                                _Client = StorageClient.Create(cred);
                                break;
                            }
                        }
                    }

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

        public static string ToString(Google.Apis.Storage.v1.Data.Object blob)
        {
            if (blob == null)
                return "";

            string ret = blob.Bucket + "/" + blob.Name;

            return Path.DirectorySeparatorChar.ToString() + Path.DirectorySeparatorChar.ToString() + ret.Replace('/', Path.DirectorySeparatorChar);
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

        public List<Google.Apis.Storage.v1.Data.Object> ListObjects(string containerName, string directory, GcsListType listType, bool recurse, string directoryFilter, string fileFilter)
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

                if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
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
                    if (listType == GcsListType.Directory || listType == GcsListType.All)
                        ret.Add(blob);

                    if (recurse)
                        ret.AddRange(ListObjects(containerName, PrefixFromPath(path), listType, recurse, directoryFilter, fileFilter));
                }
                else if (!isDir && (listType == GcsListType.File || listType == GcsListType.All))
                {
                    ret.Add(blob);
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
            prefix = prefix + '/';

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

                Google.Apis.Storage.v1.Data.Object b = Client.GetObject(containerName, prefix);

                if (b != null)
                    return new FDCFileInfo
                    {
                        CreationTimeUtc = b.TimeCreated.Value.ToUniversalTime(),
                        LastAccessTimeUtc = DateTime.UtcNow,
                        LastWriteTimeUtc = b.Updated.Value.ToUniversalTime(),
                        Size = (long)b.Size
                    };
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
                string containerName = ContainerFromPath(directory);
                string dir = PrefixFromPath(directory);

                // Google throws if the object does not exist
                try
                {
                    if (dir == "")
                        Client.GetBucket(containerName);
                    else
                        Client.GetObject(containerName, dir + "/");

                    return true;
                }
                catch { return false; }
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

                if (String.IsNullOrEmpty(dir))
                {
                    Google.Apis.Storage.v1.Data.Bucket b = Client.GetBucket(containerName);

                    return new FDCDirectoryInfo
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
                        return new FDCDirectoryInfo
                        {
                            CreationTimeUtc = b.TimeCreated.Value.ToUniversalTime(),
                            LastAccessTimeUtc = DateTime.UtcNow,
                            LastWriteTimeUtc = b.Updated.Value.ToUniversalTime(),
                        };
                }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("Authentication.GetDirectoryInfo", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            return null;
        }

        public void CreateDirectory(string directory)
        {
            try
            {
                string containerName = ContainerFromPath(directory);
                string dir = PrefixFromPath(directory);

                Google.Apis.Storage.v1.Data.Object o = new Google.Apis.Storage.v1.Data.Object();

                o.Bucket = containerName;
                o.Name = dir + "/";

                Client.UploadObject(o, new MemoryStream());
            }
            catch { }
        }

        public void DeleteDirectory(string directory)
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

        public void DeleteFile(string filename)
        {
            string containerName = ContainerFromPath(filename);
            string prefix = PrefixFromPath(filename);

            Client.DeleteObject(containerName, prefix);
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
