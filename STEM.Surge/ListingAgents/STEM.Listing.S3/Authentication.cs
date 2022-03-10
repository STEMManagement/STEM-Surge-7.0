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
using Amazon.S3;
using Amazon.S3.Model;
using STEM.Sys.Security;
using STEM.Sys.IO.Listing;

namespace STEM.Listing.S3
{
    public class Authentication : STEM.Sys.IO.Listing.IAuthentication
    {
        [Category("S3")]
        [DisplayName("Allow Bucket Control"), DescriptionAttribute("Is the user allowed to list, create, and delete Buckets?")]
        public bool AllowBucketControl { get; set; }

        [Category("S3")]
        [DisplayName("Region"), DescriptionAttribute("What is the S3 Region?")]
        public string Region { get; set; }

        [Category("S3")]
        [DisplayName("Service URL"), DescriptionAttribute("What is the S3 Service URL (blank to use the Region lookup)?")]
        public string ServiceURL { get; set; }

        [Category("S3")]
        [DisplayName("Access Key"), DescriptionAttribute("What is the S3 Access Key?")]
        public string AccessKey { get; set; }

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

        [Category("S3")]
        [DisplayName("Limit List Results"), DescriptionAttribute("What is the maximum list result length permitted?")]
        public int LimitListResults { get; set; }

        [Category("S3")]
        [DisplayName("Role Name"), DescriptionAttribute("Is this session to assume an AWS Role?")]
        public string RoleName { get; set; }

        [Category("S3")]
        [DisplayName("Role Session Name"), DescriptionAttribute("When this session assumes an AWS Role, what is the session name to be?")]
        public string RoleSessionName { get; set; }

        public Authentication()
        {
            AllowBucketControl = false;
            Region = "us-east-1";
            ServiceURL = "";
            AccessKey = "";
            SecretKey = "";
            LimitListResults = Int32.MaxValue;
            RoleName = "";
            RoleSessionName = "";
        }

        public override void PopulateFrom(Sys.Security.IAuthentication source)
        {
            if (source.VersionDescriptor.TypeName == VersionDescriptor.TypeName)
            {
                PropertyInfo i = source.GetType().GetProperties().FirstOrDefault(p => p.Name == "Region");
                if (i != null)
                {
                    string k = i.GetValue(source) as string;
                    if (!String.IsNullOrEmpty(k) && String.IsNullOrEmpty(Region))
                        Region = k;
                }

                i = source.GetType().GetProperties().FirstOrDefault(p => p.Name == "ServiceURL");
                if (i != null)
                {
                    string k = i.GetValue(source) as string;
                    if (!String.IsNullOrEmpty(k) && String.IsNullOrEmpty(ServiceURL))
                        ServiceURL = k;
                }

                i = source.GetType().GetProperties().FirstOrDefault(p => p.Name == "SecretKey");
                if (i != null)
                {
                    string k = i.GetValue(source) as string;
                    if (!String.IsNullOrEmpty(k) && String.IsNullOrEmpty(SecretKey))
                    {
                        SecretKey = k;

                        i = source.GetType().GetProperties().FirstOrDefault(p => p.Name == "AccessKey");
                        if (i != null)
                            AccessKey = i.GetValue(source) as string;

                        i = source.GetType().GetProperties().FirstOrDefault(p => p.Name == "RoleName");
                        if (i != null)
                            RoleName = i.GetValue(source) as string;

                        i = source.GetType().GetProperties().FirstOrDefault(p => p.Name == "RoleSessionName");
                        if (i != null)
                            RoleSessionName = i.GetValue(source) as string;

                        i = source.GetType().GetProperties().FirstOrDefault(p => p.Name == "AllowBucketControl");
                        if (i != null)
                            AllowBucketControl = (bool)i.GetValue(source);
                    }
                }

                i = source.GetType().GetProperties().FirstOrDefault(p => p.Name == "LimitListResults");
                if (i != null)
                    LimitListResults = (int)i.GetValue(source);
            }
            else
            {
                throw new Exception("IAuthentication Type mismatch.");
            }
        }

        IAmazonS3 _Client = null;

        [XmlIgnore]
        [Browsable(false)]
        public IAmazonS3 Client
        {
            get
            {
                lock (this)
                {
                    if (_Client == null)
                    {
                        try
                        {
                            if (String.IsNullOrEmpty(ServiceURL))
                            {
                                Amazon.Runtime.AWSCredentials creds = null;

                                creds = new Amazon.Runtime.BasicAWSCredentials(AccessKey, SecretKey);

                                if (!String.IsNullOrEmpty(RoleName))
                                    creds = new Amazon.Runtime.AssumeRoleAWSCredentials(creds, RoleName, RoleSessionName);

                                _Client = new AmazonS3Client(creds, Amazon.RegionEndpoint.GetBySystemName(Region));
                            }
                            else
                            {
                                Amazon.Runtime.AWSCredentials creds = null;

                                creds = new Amazon.Runtime.BasicAWSCredentials(AccessKey, SecretKey);

                                if (!String.IsNullOrEmpty(RoleName))
                                    creds = new Amazon.Runtime.AssumeRoleAWSCredentials(creds, RoleName, RoleSessionName);

                                AmazonS3Config cfg = new AmazonS3Config
                                {
                                    RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(Region),
                                    ServiceURL = this.ServiceURL,
                                    ForcePathStyle = true
                                };

                                _Client = new AmazonS3Client(creds, cfg);
                            }
                        }
                        catch (Exception ex)
                        {
                            STEM.Sys.EventLog.WriteEntry("S3.Authentication.Client", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                            throw ex;
                        }
                    }
                }

                return _Client;
            }
        }

        public static string ToString(S3Object s)
        {
            if (s == null)
                return "";

            return System.IO.Path.Combine(System.IO.Path.DirectorySeparatorChar.ToString() + System.IO.Path.DirectorySeparatorChar.ToString() + s.BucketName, s.Key.Replace('/', System.IO.Path.DirectorySeparatorChar)).TrimEnd(System.IO.Path.DirectorySeparatorChar);
        }

        public static string BucketFromPath(string path)
        {
            return STEM.Sys.IO.Path.FirstTokenOfPath(path).ToLower();
        }

        public static string PrefixFromPath(string path)
        {
            bool isDir = path.EndsWith("/") || path.EndsWith("\\");

            string bucket = BucketFromPath(path);

            string prefix = path.Trim('/').Trim('\\');

            prefix = prefix.Substring(bucket.Length);
            prefix = prefix.Replace('\\', '/');
            string r = prefix.Trim('/');

            if (isDir)
                r += "/";

            return r;
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

        public override IListingAgent ConstructListingAgent(ListingType listingType, string path, string fileFilter, string subpathFilter, bool recurse)
        {
            return new ListingAgent(this, listingType, path, fileFilter, subpathFilter, recurse);
        }

        public List<S3Bucket> ListBuckets()
        {
            if (!AllowBucketControl)
                return new List<S3Bucket>();

            System.Threading.Tasks.Task<ListBucketsResponse> listResponse = Client.ListBucketsAsync();
            listResponse.Wait();
            return listResponse.Result.Buckets;
        }

        public List<S3Object> ListObjects(string bucketName, int maxResults = Int32.MaxValue)
        {
            List<S3Object> ret = new List<S3Object>();

            ListObjectsRequest listRequest = new ListObjectsRequest { BucketName = bucketName, MaxKeys = maxResults };

            System.Threading.Tasks.Task<ListObjectsResponse> listResponse;
            do
            {
                // Get a list of objects
                listResponse = Client.ListObjectsAsync(listRequest);
                listResponse.Wait();

                ret.AddRange(listResponse.Result.S3Objects);

                if (ret.Count >= maxResults)
                    break;

                // Set the marker property
                listRequest.Marker = listResponse.Result.NextMarker;
            } while (listResponse.Result.IsTruncated);

            return ret;
        }

        public List<S3Object> ListObjects(string bucketName, string prefix, int maxResults = Int32.MaxValue)
        {
            List<S3Object> ret = new List<S3Object>();

            ListObjectsRequest listRequest = new ListObjectsRequest { BucketName = bucketName, Prefix = prefix, Delimiter = "/", MaxKeys = maxResults };

            System.Threading.Tasks.Task<ListObjectsResponse> listResponse;
            do
            {
                // Get a list of objects
                listResponse = Client.ListObjectsAsync(listRequest);
                listResponse.Wait();

                ret.AddRange(listResponse.Result.S3Objects);

                if (ret.Count >= maxResults)
                    break;

                // Set the marker property
                listRequest.Marker = listResponse.Result.NextMarker;
            } while (listResponse.Result.IsTruncated);

            return ret;
        }

        static Dictionary<string, Regex> _InclusiveDirFilter = new Dictionary<string, Regex>(StringComparer.InvariantCultureIgnoreCase);
        static Dictionary<string, Regex> _ExclusiveDirFilter = new Dictionary<string, Regex>(StringComparer.InvariantCultureIgnoreCase);
        static Dictionary<string, Regex> _InclusiveFileFilter = new Dictionary<string, Regex>(StringComparer.InvariantCultureIgnoreCase);
        static Dictionary<string, Regex> _ExclusiveFileFilter = new Dictionary<string, Regex>(StringComparer.InvariantCultureIgnoreCase);


        public List<S3Object> ListObjects(string bucketName, string prefix, ListingType listType, bool recurse, string directoryFilter, string fileFilter, int maxResults = Int32.MaxValue)
        {
            List<S3Object> ret = new List<S3Object>();

            prefix = prefix.Replace('\\', '/');
            prefix = prefix.TrimEnd('/');
            prefix = prefix + "/";

            Regex inclusiveDirFilter = null;
            if (_InclusiveDirFilter.ContainsKey(directoryFilter))
            {
                inclusiveDirFilter = _InclusiveDirFilter[directoryFilter];
            }
            else
            {
                inclusiveDirFilter = STEM.Sys.IO.Path.BuildInclusiveFilter(directoryFilter);
                if (inclusiveDirFilter != null)
                    _InclusiveDirFilter[directoryFilter] = inclusiveDirFilter;
            }

            Regex exclusiveDirFilter = null;
            if (_ExclusiveDirFilter.ContainsKey(directoryFilter))
            {
                exclusiveDirFilter = _ExclusiveDirFilter[directoryFilter];
            }
            else
            {
                exclusiveDirFilter = STEM.Sys.IO.Path.BuildExclusiveFilter(directoryFilter);
                if (exclusiveDirFilter != null)
                    _ExclusiveDirFilter[directoryFilter] = exclusiveDirFilter;
            }

            Regex inclusiveFileFilter = null;
            if (_InclusiveFileFilter.ContainsKey(fileFilter))
            {
                inclusiveFileFilter = _InclusiveFileFilter[fileFilter];
            }
            else
            {
                inclusiveFileFilter = STEM.Sys.IO.Path.BuildInclusiveFilter(fileFilter);
                if (inclusiveFileFilter != null)
                    _InclusiveFileFilter[fileFilter] = inclusiveFileFilter;
            }

            Regex exclusiveFileFilter = null;
            if (_ExclusiveFileFilter.ContainsKey(fileFilter))
            {
                exclusiveFileFilter = _ExclusiveFileFilter[fileFilter];
            }
            else
            {
                exclusiveFileFilter = STEM.Sys.IO.Path.BuildExclusiveFilter(fileFilter);
                if (exclusiveFileFilter != null)
                    _ExclusiveFileFilter[fileFilter] = exclusiveFileFilter;
            }

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

                if (listType != ListingType.File)
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
                        ret.AddRange(ListObjects(i.BucketName, "", listType, recurse, directoryFilter, fileFilter, maxResults));
                }

                return ret;
            }

            string fullPrefix = prefix;

            if (!fileFilter.Contains("|") &&
                !fileFilter.Contains("!") &&
                !fileFilter.Contains("<>") &&
                !fileFilter.Contains("*") &&
                !fileFilter.Contains("?"))
            {
                fullPrefix = PrefixFromPath(bucketName + "/" + prefix + fileFilter);
            }

            List<S3Object> fullList = ListObjects(bucketName, fullPrefix, maxResults);

            List<string> folders = fullList.Where(i => !i.Key.EndsWith("/") && STEM.Sys.IO.Path.GetDirectoryName(i.Key).Replace("\\", "/").TrimEnd('/') != prefix.TrimEnd('/')).Select(i => STEM.Sys.IO.Path.GetDirectoryName(i.Key).Replace("\\", "/").TrimEnd('/') + '/').ToList();
            
            folders.AddRange(fullList.Where(i => i.Key.EndsWith("/") && i.Key != prefix).Select(i => i.Key));

            folders = folders.Distinct().ToList();

            if (inclusiveDirFilter != null)
                folders = folders.Where(i => inclusiveDirFilter.IsMatch(i.TrimEnd('/').ToUpper())).ToList();

            if (exclusiveDirFilter != null)
                folders = folders.Where(i => !exclusiveDirFilter.IsMatch(i.TrimEnd('/').ToUpper())).ToList();

            if (!recurse)
            {
                folders = folders.Where(i => i.Equals(prefix, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }

            folders.Add("/");

            List<S3Object> ret2 = fullList.Where(i => !i.Key.EndsWith("/")).ToList();

            if (inclusiveFileFilter != null)
                ret2 = ret2.Where(i => inclusiveFileFilter.IsMatch(STEM.Sys.IO.Path.GetFileName(i.Key).ToUpper())).ToList();

            if (exclusiveFileFilter != null)
                ret2 = ret2.Where(i => !exclusiveFileFilter.IsMatch(STEM.Sys.IO.Path.GetFileName(i.Key).ToUpper())).ToList();

            foreach (S3Object o in ret2)
            {
                if (folders.Exists(i => i.Equals(STEM.Sys.IO.Path.GetDirectoryName(o.Key).Replace("\\", "/") + "/", StringComparison.InvariantCultureIgnoreCase)))
                    ret.Add(o);
                else if (prefix.Equals(STEM.Sys.IO.Path.GetDirectoryName(o.Key).Replace("\\", "/") + "/", StringComparison.InvariantCultureIgnoreCase))
                    ret.Add(o);
            }

            if (listType == ListingType.Directory)
            {
                ret = ret.Where(i => i.Key.EndsWith("/") && i.Key != prefix).ToList();
            }
            else if (listType == ListingType.File)
            {
                ret = ret.Where(i => !i.Key.EndsWith("/")).ToList();
            }

            return ret;
        }

        public override void CreateDirectory(string directory)
        {
            try
            {
                if (!DirectoryExists(directory))
                {
                    string bucket = BucketFromPath(directory);
                    string prefix = PrefixFromPath(directory);

                    if (prefix != "")
                    {
                        if (!DirectoryExists(bucket))
                            CreateDirectory(@"\\" + bucket);

                        prefix = prefix + "/";

                        System.Threading.Tasks.Task<PutObjectResponse> r = Client.PutObjectAsync(new PutObjectRequest { BucketName = bucket, Key = prefix });
                        r.Wait();
                    }
                    else if (AllowBucketControl)
                    {
                        System.Threading.Tasks.Task<PutBucketResponse> r = Client.PutBucketAsync(new PutBucketRequest { BucketName = bucket });
                        r.Wait();
                    }
                }
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
                    string bucket = BucketFromPath(directory);
                    string prefix = PrefixFromPath(directory);

                    if (prefix != "")
                    {
                        prefix = prefix + "/";

                        System.Threading.Tasks.Task<ListVersionsResponse> r = null;

                        try
                        {
                            r = Client.ListVersionsAsync(new ListVersionsRequest { BucketName = bucket, Prefix = prefix });
                            r.Wait();
                        }
                        catch { r = null; }

                        if (r != null && r.Result.Versions.Count > 0)
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
                    else if (AllowBucketControl)
                    {
                        System.Threading.Tasks.Task t = Client.DeleteBucketAsync(new DeleteBucketRequest { BucketName = bucket });
                        t.Wait();
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
                    string bucket = BucketFromPath(file);
                    string prefix = PrefixFromPath(file);

                    System.Threading.Tasks.Task<ListVersionsResponse> r = null;

                    try
                    {
                        r = Client.ListVersionsAsync(new ListVersionsRequest { BucketName = bucket, Prefix = prefix });
                        r.Wait();
                    }
                    catch { r = null; }

                    if (r != null && r.Result.Versions.Count > 0)
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override bool DirectoryExists(string directory)
        {
            try
            {
                string bucket = BucketFromPath(directory);
                string prefix = PrefixFromPath(directory);

                if (prefix != "")
                {
                    prefix = prefix + "/";

                    System.Threading.Tasks.Task<GetObjectResponse> r = Client.GetObjectAsync(bucket, prefix); 
                    r.Wait();

                    return r.Result.Key == prefix;
                }
                else
                {
                    if (!AllowBucketControl)
                        return true; // Assume it exists?

                    foreach (S3Bucket b in ListBuckets())
                    {
                        if (b.BucketName.Equals(bucket, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
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
                string bucket = BucketFromPath(directory);
                string prefix = PrefixFromPath(directory);

                if (prefix != "")
                {
                    prefix = prefix + "/";

                    System.Threading.Tasks.Task<GetObjectResponse> r = Client.GetObjectAsync(bucket, prefix);
                    r.Wait();

                    if (r.Result.Key == prefix)
                    {
                        return new DirectoryInfo
                        {
                            CreationTimeUtc = DateTime.MinValue,
                            LastAccessTimeUtc = DateTime.UtcNow,
                            LastWriteTimeUtc = r.Result.LastModified
                        };
                    }
                }
                else
                {
                    if (!AllowBucketControl)
                        return new DirectoryInfo
                        {
                            CreationTimeUtc = DateTime.UtcNow,
                            LastAccessTimeUtc = DateTime.UtcNow,
                            LastWriteTimeUtc = DateTime.UtcNow
                        }; // Assume it exists?

                    foreach (S3Bucket b in ListBuckets())
                    {
                        if (b.BucketName.Equals(bucket, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return new DirectoryInfo
                            {
                                CreationTimeUtc = b.CreationDate,
                                LastAccessTimeUtc = DateTime.UtcNow,
                                LastWriteTimeUtc = b.CreationDate
                            };
                        }
                    }
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
                string bucket = BucketFromPath(file);
                string prefix = PrefixFromPath(file);

                System.Threading.Tasks.Task<GetObjectMetadataResponse> r = Client.GetObjectMetadataAsync(bucket, prefix);
                r.Wait();

                return new FileInfo
                {
                    CreationTimeUtc = r.Result.LastModified,
                    LastAccessTimeUtc = DateTime.UtcNow,
                    LastWriteTimeUtc = r.Result.LastModified,
                    Size = r.Result.ContentLength
                };
            }
            catch
            {
            }

            return null;
        }
    }
}