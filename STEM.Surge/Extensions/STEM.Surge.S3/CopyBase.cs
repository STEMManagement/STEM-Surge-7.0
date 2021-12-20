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
using System.IO;
using System.Linq;
using STEM.Listing.S3;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace STEM.Surge.S3
{
    public enum S3ACL { NoACL, AuthenticatedRead, AWSExecRead, BucketOwnerFullControl, BucketOwnerRead, LogDeliveryWrite, Private, PublicRead, PublicReadWrite }
    public enum S3EncryptionMethod { None, AES256, AWSKMS }

    public abstract class CopyBase : STEM.Surge.Instruction
    {
        protected enum ActionType { Copy, Move }
        public enum DestinationRule { FirstSuccess, AllOrNone, OneOrMore }
        public enum S3Direction { ToS3Bucket, FromS3Bucket }

        protected ActionType Action { get; set; }

        [Category("S3")]
        [DisplayName("Authentication"), DescriptionAttribute("The authentication configuration to be used.")]
        public Authentication Authentication { get; set; }
        
        [Category("Source")]
        [DisplayName("Source Path"), DescriptionAttribute("The location of the file(s) to be actioned.")]
        public string SourcePath { get; set; }

        [Category("Source")]
        [DisplayName("Expand Source Path"), DescriptionAttribute("When searching for files to action, should the 'Source Path' be treated like an expandable?.")]
        public bool ExpandSource { get; set; }

        [Category("Source")]
        [DisplayName("File Filter"), DescriptionAttribute("The filter used on files in 'Source Path' to select files to action. This can be a compound filter.")]
        public string FileFilter { get; set; }

        [Category("Source")]
        [DisplayName("Directory Filter"), DescriptionAttribute("The filter used on directories in 'Source Path' when 'Recurse Source' is specified. This can be a compound filter.")]
        public string DirectoryFilter { get; set; }

        [Category("Source")]
        [DisplayName("Recurse Source"), DescriptionAttribute("Recurse the source for files to action?")]
        public bool RecurseSource { get; set; }

        [Category("Destination")]
        [DisplayName("Recreate Source Tree"), DescriptionAttribute("If the source folder is being recursively searched, should the subdirectory be recreated in the destination folder?")]
        public bool RecreateTree { get; set; }

        [Category("Destination")]
        [DisplayName("Destination Path"), DescriptionAttribute("The destination folder.")]
        public string DestinationPath { get; set; }

        [Category("Destination")]
        [DisplayName("Destination Filename"), DescriptionAttribute("The destination filename. Set this to '*.*' to maintain source filename(s), '*' to use the source filename(s) " +
            "without the extension, '*.*suffix' to append to the filename(s), and 'prefix*.*' to prepend. And get creative (e.g. " +
            "*.suffix to replace the extension, prefix*suffix to wrap the filename without its extension...)")]
        public string DestinationFilename { get; set; }

        [Category("Destination")]
        [DisplayName("Expand Destination"), DescriptionAttribute("Should the 'Destination Path' be treated like an expandable? If so, files will be put in every destination.")]
        public bool ExpandDestination { get; set; }

        [Category("Destination")]
        [DisplayName("Exists Action"), DescriptionAttribute("What to do if the destination exists.")]
        public STEM.Sys.IO.FileExistsAction ExistsAction { get; set; }

        [Category("Destination")]
        [DisplayName("Destination Action Rule"), DescriptionAttribute("Accept FirstSuccess and move on, require AllOrNone, take OneOrMore success as good enough.")]
        public DestinationRule DestinationActionRule { get; set; }

        [Category("Retry")]
        [DisplayName("Number of retries"), DescriptionAttribute("How many times should each operation be attempted?")]
        public int Retry { get; set; }

        [Category("Retry")]
        [DisplayName("Seconds between retries"), DescriptionAttribute("How many seconds should we wait between retries?")]
        public int RetryDelaySeconds { get; set; }

        [Category("Flow")]
        [DisplayName("Execution Mode"), Description("Should this be executed on forward InstructionSet execution or on Rollback? Consider the use case where you want to " +
            "move a file out of the flow to an error folder on Rollback.")]
        public ExecuteOn ExecutionMode { get; set; }

        [Category("S3")]
        [DisplayName("S3 Direction"), DescriptionAttribute("Is the action to or from an S3 bucket?")]
        public S3Direction Direction { get; set; }

        [Category("S3")]
        [DisplayName("ACL"), DescriptionAttribute("When writing to an S3 bucket, what ACL should be used?")]
        public S3ACL ACL { get; set; }

        [Category("S3")]
        [DisplayName("Server Side Encryption Method"), DescriptionAttribute("When writing to an S3 bucket, what EncryptionMethod should be used?")]
        public S3EncryptionMethod EncryptionMethod { get; set; }

        [Category("S3")]
        [DisplayName("Server Side Encryption KMS Key ID"), DescriptionAttribute("When writing to an S3 bucket and using AWSKMS Server Side Encryption, what KMS Key ID should be used?")]
        public string KMS_Key_ID { get; set; }

        [Category("Flow")]
        [DisplayName("Zero Files Action"), Description("What flow action should be taken if no files are found?")]
        public FailureAction ZeroFilesAction { get; set; }

        [Category("Large File Handling")]
        [DisplayName("Minimum Part Size (bytes)"), Description("When uploading a file, how large should uploadable chunks be?")]
        public int MinimumPartSize { get; set; }

        [Category("Large File Handling")]
        [DisplayName("Maximum Number Of Parts"), Description("When uploading a file, should 'Minimum Part Size' be modified to limit to a 'Maximum Number Of Parts'?")]
        public int MaximumNumberOfParts { get; set; }

        public CopyBase()
            : base()
        {
            Authentication = new Authentication();

            ExistsAction = STEM.Sys.IO.FileExistsAction.MakeUnique;
            DestinationActionRule = DestinationRule.AllOrNone;
            Retry = 1;
            RetryDelaySeconds = 2;

            ExpandSource = false;
            SourcePath = "[TargetPath]";
            FileFilter = "[TargetName]";
            DirectoryFilter = "!TEMP";
            RecurseSource = false;
            RecreateTree = false;

            ExpandDestination = false;
            DestinationPath = "[DestinationPath]\\[SubDir]";
            DestinationFilename = "*.*";

            ExecutionMode = ExecuteOn.ForwardExecution;
            ZeroFilesAction = FailureAction.SkipRemaining;

            ACL = S3ACL.NoACL;
            EncryptionMethod = S3EncryptionMethod.None;
            KMS_Key_ID = "";

            MinimumPartSize = 10485760;
            MaximumNumberOfParts = 1000;
        }

        Dictionary<string, int> _PartSize = new Dictionary<string, int>();
        Dictionary<string, string> _FilesActioned = new Dictionary<string, string>();

        protected override void _Rollback()
        {
            if (ExecutionMode == ExecuteOn.ForwardExecution)
            {
                foreach (string d in _FilesActioned.Keys)
                {
                    try
                    {
                        string s = _FilesActioned[d];

                        if (Action == ActionType.Move)
                        {
                            if (Direction == S3Direction.ToS3Bucket)
                            {
                                string bucket = Authentication.BucketFromPath(s);
                                string prefix = Authentication.PrefixFromPath(s);

                                STEM.Sys.IO.Listing.FileInfo fi = Authentication.GetFileInfo(s);

                                if (fi != null)
                                {
                                    string tmp = "";

                                    try
                                    {
                                        tmp = Path.Combine(STEM.Sys.IO.Path.GetDirectoryName(d), "TEMP");

                                        if (!Directory.Exists(tmp))
                                            Directory.CreateDirectory(tmp);

                                        tmp = Path.Combine(tmp, STEM.Sys.IO.Path.GetFileName(d));
										
                                        TransferUtilityDownload(bucket, prefix, tmp);

                                        try
                                        {
                                            File.SetLastWriteTimeUtc(tmp, fi.LastWriteTimeUtc);
                                        }
                                        catch { }

                                        File.Move(tmp, STEM.Sys.IO.Path.AdjustPath(d));
                                    }
                                    finally
                                    {
                                        try
                                        {
                                            if (File.Exists(tmp))
                                                File.Delete(tmp);
                                        }
                                        catch { }
                                    }
                                }
                                else
                                {
                                    throw new System.IO.FileNotFoundException(s);
                                }
                            }
                            else
                            {
                                string bucket = Authentication.BucketFromPath(d);
                                string prefix = Authentication.PrefixFromPath(d);
								
                                TransferUtilityUpload(bucket, prefix, s, d);
                            }
                        }

                        if (Direction == S3Direction.ToS3Bucket)
                        {
                            Authentication.DeleteFile(s);
                        }
                        else
                        {
                            STEM.Sys.IO.File.STEM_Delete(s, false, Retry, RetryDelaySeconds);
                        }
                    }
                    catch { }
                }
            }
            else
            {
                Execute();
            }
        }

        protected override bool _Run()
        {
            if (ExecutionMode == ExecuteOn.ForwardExecution)
                return Execute();

            return true;
        }

        bool Execute()
        {
            try
            {
                if (Direction == S3Direction.ToS3Bucket)
                    ExpandDestination = false;
                else
                    ExpandSource = false;

                List<string> sources = new List<string>();
                if (ExpandSource)
                {
                    sources = STEM.Sys.IO.Path.ExpandRangedPath(SourcePath);
                }
                else
                {
                    sources.Add(SourcePath);
                }

                List<string> destinations = new List<string>();
                if (ExpandDestination)
                {
                    destinations = STEM.Sys.IO.Path.ExpandRangedPath(DestinationPath);
                }
                else
                {
                    destinations.Add(DestinationPath);
                }

                List<string> sourceFiles = new List<string>();

                int filesActioned = 0;

                foreach (string src in sources)
                {
                    List<S3Object> items = new List<S3Object>();

                    if (Direction == S3Direction.ToS3Bucket)
                    {
                        sourceFiles = STEM.Sys.IO.Directory.STEM_GetFiles(src, FileFilter, DirectoryFilter, (RecurseSource ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly), false);
                    }
                    else
                    {
                        string bucket = Authentication.BucketFromPath(src);
                        string prefix = Authentication.PrefixFromPath(src);
                        if (PopulatePostMortemMeta)
                        {
                            PostMortemMetaData["Bucket"] = bucket;
                            PostMortemMetaData["Prefix"] = prefix;
                        }

                        items = Authentication.ListObjects(bucket, prefix, STEM.Sys.IO.Listing.ListingType.File, RecurseSource, DirectoryFilter, FileFilter);

                        sourceFiles = items.Select(i => Authentication.ToString(i)).ToList();
                    }

                    foreach (string s in sourceFiles)
                    {
                        try
                        {
                            bool success = false;

                            Exception lastEX = null;

                            foreach (string d in destinations)
                            {
                                try
                                {
                                    string dFile = "";

                                    long fileSz = 0;

                                    if (Direction == S3Direction.ToS3Bucket)
                                        fileSz = new FileInfo(s).Length;
                                    else
                                        fileSz = Authentication.GetFileInfo(s).Size;

                                    _PartSize[s] = (int)Math.Max(MinimumPartSize, fileSz / MaximumNumberOfParts);

                                    try
                                    {
                                        if (PopulatePostMortemMeta)
                                        {
                                            PostMortemMetaData["SourceIP"] = STEM.Sys.IO.Path.IPFromPath(s);
                                            PostMortemMetaData["DestinationIP"] = STEM.Sys.IO.Path.IPFromPath(d);
                                            PostMortemMetaData["FileSize"] = fileSz.ToString();
                                        }
                                    }
                                    catch { }

                                    if (Direction == S3Direction.ToS3Bucket)
                                    {
                                        string dPath = STEM.Sys.IO.Path.AdjustPath(d);
                                        if (RecurseSource && RecreateTree)
                                        {
                                            dPath = System.IO.Path.Combine(dPath, STEM.Sys.IO.Path.GetDirectoryName(s).Replace(STEM.Sys.IO.Path.AdjustPath(src), "").Trim(System.IO.Path.DirectorySeparatorChar));
                                        }

                                        dPath = System.IO.Path.Combine(dPath, DestinationFilename);

                                        if (dPath.Contains("*.*"))
                                            dPath = dPath.Replace("*.*", STEM.Sys.IO.Path.GetFileName(s));

                                        if (dPath.Contains("*"))
                                            dPath = dPath.Replace("*", STEM.Sys.IO.Path.GetFileNameWithoutExtension(s));

                                        if (!Authentication.DirectoryExists(STEM.Sys.IO.Path.GetDirectoryName(dPath)))
                                            Authentication.CreateDirectory(STEM.Sys.IO.Path.GetDirectoryName(dPath));

                                        if (Authentication.FileExists(dPath))
                                        {
                                            switch (ExistsAction)
                                            {
                                                case Sys.IO.FileExistsAction.Overwrite:
                                                    Authentication.DeleteFile(dPath);
                                                    dFile = dPath;
                                                    break;

                                                case Sys.IO.FileExistsAction.OverwriteIfNewer:

                                                    if (Authentication.GetFileInfo(dPath).LastWriteTimeUtc >= File.GetLastWriteTimeUtc(s))
                                                        continue;

                                                    Authentication.DeleteFile(dPath);
                                                    dFile = dPath;
                                                    break;

                                                case Sys.IO.FileExistsAction.Skip:
                                                    continue;

                                                case Sys.IO.FileExistsAction.Throw:
                                                    throw new IOException("Destination file exists. (" + dPath + ")");

                                                case Sys.IO.FileExistsAction.MakeUnique:
                                                    dFile = Authentication.UniqueFilename(dPath);
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            dFile = dPath;
                                        }

                                        string bucket = Authentication.BucketFromPath(dFile);
                                        string prefix = Authentication.PrefixFromPath(dFile);
                                        if (PopulatePostMortemMeta)
                                        {
                                            PostMortemMetaData["Bucket"] = bucket;
                                            PostMortemMetaData["Prefix"] = prefix;
                                        }
										
                                        TransferUtilityUpload(bucket, prefix, s, s);
                                    }
                                    else
                                    {
                                        string dPath = STEM.Sys.IO.Path.AdjustPath(d);
                                        if (RecurseSource && RecreateTree)
                                        {
                                            dPath = System.IO.Path.Combine(dPath, STEM.Sys.IO.Path.GetDirectoryName(s).Replace(STEM.Sys.IO.Path.AdjustPath(src), "").Trim(System.IO.Path.DirectorySeparatorChar));
                                        }

                                        dPath = System.IO.Path.Combine(dPath, DestinationFilename);

                                        if (dPath.Contains("*.*"))
                                            dPath = dPath.Replace("*.*", STEM.Sys.IO.Path.GetFileName(s));

                                        if (dPath.Contains("*"))
                                            dPath = dPath.Replace("*", STEM.Sys.IO.Path.GetFileNameWithoutExtension(s));

                                        if (File.Exists(dPath))
                                        {
                                            switch (ExistsAction)
                                            {
                                                case Sys.IO.FileExistsAction.Overwrite:
                                                    File.Delete(dPath);
                                                    dFile = dPath;
                                                    break;
                                                case Sys.IO.FileExistsAction.OverwriteIfNewer:

                                                    if (File.GetLastWriteTimeUtc(dPath) >= Authentication.GetFileInfo(s).LastWriteTimeUtc)
                                                        continue;

                                                    File.Delete(dPath);
                                                    dFile = dPath;
                                                    break;

                                                case Sys.IO.FileExistsAction.Skip:
                                                    continue;

                                                case Sys.IO.FileExistsAction.Throw:
                                                    throw new IOException("Destination file exists. (" + dPath + ")");

                                                case Sys.IO.FileExistsAction.MakeUnique:
                                                    dFile = STEM.Sys.IO.File.UniqueFilename(dPath);
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            dFile = dPath;
                                        }

                                        if (!Directory.Exists(STEM.Sys.IO.Path.GetDirectoryName(dFile)))
                                            Directory.CreateDirectory(STEM.Sys.IO.Path.GetDirectoryName(dFile));

                                        string bucket = Authentication.BucketFromPath(s);
                                        string prefix = Authentication.PrefixFromPath(s);
                                        if (PopulatePostMortemMeta)
                                        {
                                            PostMortemMetaData["Bucket"] = bucket;
                                            PostMortemMetaData["Prefix"] = prefix;
                                        }

                                        STEM.Sys.IO.Listing.FileInfo fi = Authentication.GetFileInfo(s);

                                        if (fi != null)
                                        {
                                            string tmp = "";
                                            try
                                            {
                                                tmp = Path.Combine(STEM.Sys.IO.Path.GetDirectoryName(dFile), "TEMP");

                                                if (!Directory.Exists(tmp))
                                                    Directory.CreateDirectory(tmp);

                                                tmp = Path.Combine(tmp, STEM.Sys.IO.Path.GetFileName(dFile));
												
                                                TransferUtilityDownload(bucket, prefix, tmp);

                                                try
                                                {
                                                    File.SetLastWriteTimeUtc(tmp, fi.LastWriteTimeUtc);
                                                }
                                                catch { }

                                                File.Move(tmp, STEM.Sys.IO.Path.AdjustPath(dFile));
                                            }
                                            finally
                                            {
                                                try
                                                {
                                                    if (File.Exists(tmp))
                                                        File.Delete(tmp);
                                                }
                                                catch { }
                                            }
                                        }
                                        else
                                        {
                                            throw new System.IO.FileNotFoundException(s);
                                        }
                                    }

                                    if (!String.IsNullOrEmpty(dFile))
                                    {
                                        filesActioned++;

                                        _FilesActioned[s] = dFile;

                                        if (Action == ActionType.Move)
                                            AppendToMessage(s + " moved to " + dFile);
                                        else
                                            AppendToMessage(s + " copied to " + dFile);
                                    }

                                    success = true;

                                    if (DestinationActionRule == DestinationRule.FirstSuccess)
                                        break;
                                }
                                catch (Exception ex)
                                {
                                    lastEX = ex;

                                    if (DestinationActionRule == DestinationRule.AllOrNone)
                                        throw ex;
                                }
                            }

                            if (!success)
                                throw new Exception("No successful actions taken for " + s, lastEX); // + "\r\n" + ((lastEX == null) ? "No additional information." : lastEX.ToString()));

                            if (Action == ActionType.Move)
                            {
                                if (Direction == S3Direction.ToS3Bucket)
                                {
                                    File.Delete(s);
                                }
                                else
                                {
                                    Authentication.DeleteFile(s);
                                }
                            }
                        }
                        catch (AggregateException ex)
                        {
                            foreach (Exception e in ex.InnerExceptions)
                            {
                                AppendToMessage(e.Message);
                            }
							Exceptions.Add(ex); //add the entire message to the collection to maintain the top level exception's stack
                        }
                        catch (Exception ex)
                        {
                            AppendToMessage(ex.Message);
                            Exceptions.Add(ex);
                        }
                    }
                }

                if (PopulatePostMortemMeta)
                {
                    PostMortemMetaData["FilesActioned"] = filesActioned.ToString();
                }
            }
            catch (AggregateException ex)
            {
                foreach (Exception e in ex.InnerExceptions)
                {
                    AppendToMessage(e.Message);
                }
                Exceptions.Add(ex); //add the entire message to the collection to maintain the top level exception's stack
            }
            catch (Exception ex)
            {
                AppendToMessage(ex.Message);
                Exceptions.Add(ex);
            }

            if (_FilesActioned.Count == 0)
            {
                switch (ZeroFilesAction)
                {
                    case FailureAction.SkipRemaining:
                        SkipRemaining();
                        return true;

                    case FailureAction.SkipNext:
                        SkipNext();
                        return true;

                    case FailureAction.SkipToLabel:
                        SkipForwardToFlowControlLabel(FailureActionLabel);
                        return true;

                    case FailureAction.Rollback:
                        RollbackAllPreceedingAndSkipRemaining();
                        break;

                    case FailureAction.Continue:
                        return true;
                }

                Message = "0 Files Actioned\r\n" + Message;
            }

            return Exceptions.Count == 0;
        }
		
        protected void TransferUtilityDownload(string bucket, string prefix, string downloadpath)
        {	
			TransferUtility ftu = new TransferUtility(Authentication.Client);
													
			TransferUtilityDownloadRequest request = new TransferUtilityDownloadRequest
			{
                BucketName = bucket,
                Key = prefix,
                FilePath = downloadpath
			};

			ftu.DownloadAsync(request).Wait();
        }
		
        //pathkey should be the same as sourcepath except during rollback
        protected void TransferUtilityUpload(string bucket, string prefix, string sourcepath, string pathkey)
		{
			TransferUtility ftu = new TransferUtility(Authentication.Client);

			TransferUtilityUploadRequest request = new TransferUtilityUploadRequest
			{
                BucketName = bucket,
                Key = prefix,
                FilePath = STEM.Sys.IO.Path.AdjustPath(sourcepath),
                PartSize = _PartSize[pathkey]
			};

			switch (EncryptionMethod)
			{
                case S3EncryptionMethod.AES256:
                    request.ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256;
                    break;

                case S3EncryptionMethod.AWSKMS:
                    request.ServerSideEncryptionMethod = ServerSideEncryptionMethod.AWSKMS;
                    request.ServerSideEncryptionKeyManagementServiceKeyId = KMS_Key_ID;
                    break;
			}

			switch (ACL)
			{
                case S3ACL.AuthenticatedRead:
                    request.CannedACL = S3CannedACL.AuthenticatedRead;
                    break;

                case S3ACL.AWSExecRead:
                    request.CannedACL = S3CannedACL.AWSExecRead;
                    break;

                case S3ACL.BucketOwnerFullControl:
                    request.CannedACL = S3CannedACL.BucketOwnerFullControl;
                    break;

                case S3ACL.BucketOwnerRead:
                    request.CannedACL = S3CannedACL.BucketOwnerRead;
                    break;

                case S3ACL.LogDeliveryWrite:
                    request.CannedACL = S3CannedACL.LogDeliveryWrite;
                    break;

                case S3ACL.Private:
                    request.CannedACL = S3CannedACL.Private;
                    break;

                case S3ACL.PublicRead:
                    request.CannedACL = S3CannedACL.PublicRead;
                    break;

                case S3ACL.PublicReadWrite:
                    request.CannedACL = S3CannedACL.PublicReadWrite;
                    break;
			}

			ftu.UploadAsync(request).Wait();
        }
    }
}

