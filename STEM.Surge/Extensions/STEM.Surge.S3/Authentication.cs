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
using Amazon.S3;
using Amazon.S3.Model;
using STEM.Sys.Security;

namespace STEM.Surge.S3
{
    public enum S3ListType { File, Directory, All }

    public class Authentication : IAuthentication
    {
        [Category("S3")]
        [DisplayName("Region"), DescriptionAttribute("What is the S3 Region?")]
        public string Region { get; set; }

        [Category("S3")]
        [DisplayName("Service URL"), DescriptionAttribute("What is the S3 Service URL (blank to use the Region lookup)?")]
        public string ServiceURL { get; set; }

        [Category("S3")]
        [DisplayName("Access Key"), DescriptionAttribute("What is the S3 Access Key?")]
        [XmlIgnore]
        [PasswordPropertyText(true)]
        public string AccessKey { get; set; }
        [Browsable(false)]
        public string AccessKeyEncoded
        {
            get
            {
                return this.Entangle(AccessKey);
            }

            set
            {
                AccessKey = this.Detangle(value);
            }
        }

        [Category("S3")]
        [DisplayName("Secret Key"), DescriptionAttribute("What is the S3 Secret Key?")]
        [XmlIgnore]
        [PasswordPropertyText(true)]
        public string SecretKey { get; set; }
        [Browsable(false)]
        public string SecretKeyEncoded
        {
            get
            {
                return this.Entangle(SecretKey);
            }

            set
            {
                SecretKey = this.Detangle(value);
            }
        }

        public Authentication()
        {
            Region = "us-east-1";
            ServiceURL = "";
            AccessKey = "";
            SecretKey = "";
        }

        IAmazonS3 _Client = null;

        [XmlIgnore]
        [Browsable(false)]
        internal IAmazonS3 Client
        {
            get
            {
                lock (this)
                {
                    if (_Client == null)
                    {
                        if (String.IsNullOrEmpty(ServiceURL))
                        {
                            _Client = new AmazonS3Client(AccessKey, SecretKey, Amazon.RegionEndpoint.GetBySystemName(Region));
                        }
                        else
                        {
                            AmazonS3Config cfg = new AmazonS3Config
                            {
                                RegionEndpoint = Amazon.RegionEndpoint.USEast1,
                                ServiceURL = this.ServiceURL,
                                ForcePathStyle = true
                            };

                            _Client = new AmazonS3Client(AccessKey, SecretKey, cfg);
                        }
                    }
                }

                return _Client;
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

        public static string ToString(S3Object s)
        {
            if (s == null)
                return "";

            return Path.Combine(Path.DirectorySeparatorChar.ToString() + Path.DirectorySeparatorChar.ToString() + s.BucketName, s.Key.Replace('/', Path.DirectorySeparatorChar)).TrimEnd(Path.DirectorySeparatorChar);
        }

        public static string BucketFromPath(string path)
        {
            return STEM.Sys.IO.Path.FirstTokenOfPath(path).ToLower();
        }

        public static string PrefixFromPath(string path)
        {
            string bucket = BucketFromPath(path);

            string prefix = path.Trim('/').Trim('\\');

            prefix = prefix.Substring(bucket.Length);
            prefix = prefix.Replace('\\', '/');
            return prefix.Trim('/');
        }

        public List<S3Bucket> ListBuckets()
        {            
            System.Threading.Tasks.Task<ListBucketsResponse> listResponse = Client.ListBucketsAsync();
            listResponse.Wait();
            return listResponse.Result.Buckets;
        }

        public List<S3Object> ListObjects(string bucketName)
        {
            List<S3Object> ret = new List<S3Object>();

            ListObjectsRequest listRequest = new ListObjectsRequest { BucketName = bucketName };

            System.Threading.Tasks.Task<ListObjectsResponse> listResponse;
            do
            {
                // Get a list of objects
                listResponse = Client.ListObjectsAsync(listRequest);
                listResponse.Wait();

                ret.AddRange(listResponse.Result.S3Objects);

                // Set the marker property
                listRequest.Marker = listResponse.Result.NextMarker;
            } while (listResponse.Result.IsTruncated);

            return ret;
        }

        public List<S3Object> ListObjects(string bucketName, string prefix)
        {
            List<S3Object> ret = new List<S3Object>();

            ListObjectsRequest listRequest = new ListObjectsRequest { BucketName = bucketName, Prefix = prefix };

            System.Threading.Tasks.Task<ListObjectsResponse> listResponse;
            do
            {
                // Get a list of objects
                listResponse = Client.ListObjectsAsync(listRequest);
                listResponse.Wait();

                ret.AddRange(listResponse.Result.S3Objects);

                // Set the marker property
                listRequest.Marker = listResponse.Result.NextMarker;
            } while (listResponse.Result.IsTruncated);

            return ret;
        }

        public List<S3Object> ListObjects(string bucketName, string prefix, S3ListType listType, bool recurse, string directoryFilter, string fileFilter)
        {
            List<S3Object> ret = new List<S3Object>();

            prefix = prefix.Replace('\\', '/');
            prefix = prefix.TrimEnd('/');

            Regex inclusiveDirFilter = STEM.Sys.IO.Path.BuildInclusiveFilter(directoryFilter);
            Regex exclusiveDirFilter = STEM.Sys.IO.Path.BuildExclusiveFilter(directoryFilter);

            Regex inclusiveFileFilter = STEM.Sys.IO.Path.BuildInclusiveFilter(fileFilter);
            Regex exclusiveFileFilter = STEM.Sys.IO.Path.BuildExclusiveFilter(fileFilter);
                                                                              
            if (String.IsNullOrEmpty(bucketName))
            {
                List<S3Object> buckets = new List<S3Object>();
                foreach (S3Bucket i in ListBuckets())
                {
                    S3Object o = new S3Object();
                    o.BucketName = i.BucketName;
                    o.Key = "";
                    o.LastModified = i.CreationDate;
                    o.Size = -1;

                    buckets.Add(o);
                }

                if (listType != S3ListType.File)
                {
                    if (inclusiveDirFilter != null)
                        buckets = buckets.Where(i => inclusiveDirFilter.IsMatch(STEM.Sys.IO.Path.GetFileName(i.BucketName.TrimEnd('/')).ToUpper())).ToList();

                    if (exclusiveDirFilter != null)
                        buckets = buckets.Where(i => !exclusiveDirFilter.IsMatch(STEM.Sys.IO.Path.GetFileName(i.BucketName.TrimEnd('/')).ToUpper())).ToList();

                    ret.AddRange(buckets);
                }

                if (recurse)
                {
                    foreach (S3Object i in buckets)
                        ret.AddRange(ListObjects(i.BucketName, "", listType, recurse, directoryFilter, fileFilter));
                }

                return ret;
            }

            List<S3Object> fullList = ListObjects(bucketName, prefix);

            ret = fullList.Where(i => i.Key.EndsWith("/") && i.Key.TrimEnd('/') != prefix).ToList();

            if (inclusiveDirFilter != null)
                ret = ret.Where(i => inclusiveDirFilter.IsMatch(STEM.Sys.IO.Path.GetFileName(i.Key.TrimEnd('/')).ToUpper())).ToList();

            if (exclusiveDirFilter != null)
                ret = ret.Where(i => !exclusiveDirFilter.IsMatch(STEM.Sys.IO.Path.GetFileName(i.Key.TrimEnd('/')).ToUpper())).ToList();

            if (!recurse)
            {
                ret = ret.Where(i => STEM.Sys.IO.Path.GetDirectoryName(i.Key.TrimEnd('/')).Replace("\\", "/").Equals(prefix, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }

            List<S3Object> ret2 = fullList.Where(i => !i.Key.EndsWith("/")).ToList();

            if (inclusiveFileFilter != null)
                ret2 = ret2.Where(i => inclusiveFileFilter.IsMatch(STEM.Sys.IO.Path.GetFileName(i.Key).ToUpper())).ToList();

            if (exclusiveFileFilter != null)
                ret2 = ret2.Where(i => !exclusiveFileFilter.IsMatch(STEM.Sys.IO.Path.GetFileName(i.Key).ToUpper())).ToList();

            foreach (S3Object o in ret2)
            {
                if (ret.Exists(i => i.Key.Equals(STEM.Sys.IO.Path.GetDirectoryName(o.Key).Replace("\\", "/") + "/", StringComparison.InvariantCultureIgnoreCase)))
                    ret.Add(o);
                else if (prefix.Equals(STEM.Sys.IO.Path.GetDirectoryName(o.Key).Replace("\\", "/"), StringComparison.InvariantCultureIgnoreCase))
                    ret.Add(o);
            }
            
            if (listType == S3ListType.Directory)
            {
                ret = ret.Where(i => i.Key.EndsWith("/") && i.Key.TrimEnd('/') != prefix).ToList();
            }
            else if (listType == S3ListType.File)
            {
                ret = ret.Where(i => !i.Key.EndsWith("/")).ToList();
            }

            return ret;
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
                string bucket = BucketFromPath(file);
                string prefix = PrefixFromPath(file);

                System.Threading.Tasks.Task<GetObjectMetadataResponse> r = Client.GetObjectMetadataAsync(bucket, prefix);
                r.Wait();

                return new FDCFileInfo
                {
                    CreationTimeUtc = r.Result.LastModified,
                    LastAccessTimeUtc = DateTime.UtcNow,
                    LastWriteTimeUtc = r.Result.LastModified,
                    Size = r.Result.ContentLength
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
                FDCDirectoryInfo di = GetDirectoryInfo(directory);

                if (di != null)
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
                string bucket = BucketFromPath(directory);
                string prefix = PrefixFromPath(directory);

                System.Threading.Tasks.Task<GetObjectMetadataResponse> r = Client.GetObjectMetadataAsync(bucket, prefix + "/");
                r.Wait();

                return new FDCDirectoryInfo
                {
                    CreationTimeUtc = r.Result.LastModified,
                    LastAccessTimeUtc = DateTime.UtcNow,
                    LastWriteTimeUtc = r.Result.LastModified,
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
            if (!DirectoryExists(directory))
            {
                string bucket = BucketFromPath(directory);
                string prefix = PrefixFromPath(directory);
                prefix = prefix + "/";

                System.Threading.Tasks.Task<PutObjectResponse> r = Client.PutObjectAsync(new PutObjectRequest { BucketName = bucket, Key = prefix });
                r.Wait();
            }
        }

        public void DeleteDirectory(string directory)
        {
            if (DirectoryExists(directory))
            {
                string bucket = BucketFromPath(directory);
                string prefix = PrefixFromPath(directory);
                prefix = prefix + "/";

                System.Threading.Tasks.Task<ListVersionsResponse> r = Client.ListVersionsAsync(new ListVersionsRequest { BucketName = bucket, Prefix = prefix });
                r.Wait();

                if (r.Result.Versions.Count > 0)
                {
                    foreach (S3ObjectVersion v in r.Result.Versions)
                    {
                        DeleteObjectRequest deleteObjectRequest = new DeleteObjectRequest
                        {
                            BucketName = v.BucketName,
                            Key = v.Key,
                            VersionId = v.VersionId
                        };

                        System.Threading.Tasks.Task t = Client.DeleteObjectAsync(deleteObjectRequest);
                        t.Wait();
                    }
                }
                else
                {
                    DeleteObjectRequest deleteObjectRequest = new DeleteObjectRequest
                    {
                        BucketName = bucket,
                        Key = prefix,
                        VersionId = null
                    };

                    System.Threading.Tasks.Task t = Client.DeleteObjectAsync(deleteObjectRequest);
                    t.Wait();
                }
            }
        }

        public void DeleteFile(string filename)
        {
            if (FileExists(filename))
            {
                string bucket = BucketFromPath(filename);
                string prefix = PrefixFromPath(filename);

                System.Threading.Tasks.Task<ListVersionsResponse> r = Client.ListVersionsAsync(new ListVersionsRequest { BucketName = bucket, Prefix = prefix });
                r.Wait();

                if (r.Result.Versions.Count > 0)
                {
                    foreach (S3ObjectVersion v in r.Result.Versions)
                    {
                        DeleteObjectRequest deleteObjectRequest = new DeleteObjectRequest
                        {
                            BucketName = v.BucketName,
                            Key = v.Key,
                            VersionId = v.VersionId
                        };

                        System.Threading.Tasks.Task t = Client.DeleteObjectAsync(deleteObjectRequest);
                        t.Wait();
                    }
                }
                else
                {
                    DeleteObjectRequest deleteObjectRequest = new DeleteObjectRequest
                    {
                        BucketName = bucket,
                        Key = prefix,
                        VersionId = null
                    };

                    System.Threading.Tasks.Task t = Client.DeleteObjectAsync(deleteObjectRequest);
                    t.Wait();
                }
            }
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

            return unique.Replace("\\", "/");
        }
    }
}