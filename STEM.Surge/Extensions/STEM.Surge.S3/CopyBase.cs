﻿/*
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
using Amazon.S3;
using Amazon.S3.Model;

namespace STEM.Surge.S3
{
    public abstract class CopyBase : STEM.Surge.Instruction
    {
        protected enum ActionType { Copy, Move }
        public enum DestinationRule { FirstSuccess, AllOrNone, OneOrMore }
        public enum ExecutoOn { ForwardExecution, Rollback }
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
        public ExecutoOn ExecutionMode { get; set; }

        [Category("S3")]
        [DisplayName("S3 Direction"), DescriptionAttribute("Is the action to or from an S3 bucket?")]
        public S3Direction Direction { get; set; }

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

            ExecutionMode = ExecutoOn.ForwardExecution;
        }

        Dictionary<string, string> _FilesActioned = new Dictionary<string, string>();

        protected override void _Rollback()
        {
            if (ExecutionMode == ExecutoOn.ForwardExecution)
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

                                FDCFileInfo fi = Authentication.GetFileInfo(s);

                                if (fi != null)
                                {
                                    System.Threading.Tasks.Task<System.IO.Stream> streamResult = Authentication.Client.GetObjectStreamAsync(bucket, prefix, null);
                                    streamResult.Wait();

                                    using (System.IO.Stream sStream = streamResult.Result)
                                    {
                                        using (System.IO.Stream dStream = System.IO.File.Open(STEM.Sys.IO.Path.AdjustPath(d), System.IO.FileMode.CreateNew, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None))
                                        {
                                            sStream.CopyTo(dStream);
                                        }
                                    }

                                    File.SetLastWriteTimeUtc(STEM.Sys.IO.Path.AdjustPath(d), fi.LastWriteTimeUtc);
                                }
                                else
                                {
                                    throw new System.IO.FileNotFoundException(s);
                                }
                            }
                            else
                            {
                                using (System.IO.Stream sStream = System.IO.File.Open(STEM.Sys.IO.Path.AdjustPath(s), System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None))
                                {
                                    string bucket = Authentication.BucketFromPath(d);
                                    string prefix = Authentication.PrefixFromPath(d);

                                    PutObjectRequest req = new PutObjectRequest { BucketName = bucket, AutoCloseStream = false, Key = prefix, InputStream = sStream };
                                    Authentication.Client.PutObjectAsync(req).Wait();
                                }
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
            if (ExecutionMode == ExecutoOn.ForwardExecution)
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

                        items = Authentication.ListObjects(bucket, prefix, S3ListType.File, RecurseSource, DirectoryFilter, FileFilter);

                        sourceFiles = items.Select(i => Authentication.ToString(i)).ToList();
                    }

                    foreach (string s in sourceFiles)
                    {
                        try
                        {
                            bool success = false;
                            foreach (string d in destinations)
                            {
                                try
                                {
                                    filesActioned++;

                                    string dFile = "";

                                    try
                                    {
                                        if (PopulatePostMortemMeta)
                                        {
                                            PostMortemMetaData["SourceIP"] = STEM.Sys.IO.Path.IPFromPath(s);
                                            PostMortemMetaData["DestinationIP"] = STEM.Sys.IO.Path.IPFromPath(d);

                                            if (Direction == S3Direction.ToS3Bucket)
                                                PostMortemMetaData["FileSize"] = new FileInfo(s).Length.ToString();
                                            else
                                                PostMortemMetaData["FileSize"] = Authentication.GetFileInfo(s).Size.ToString();
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

                                        using (System.IO.Stream sStream = System.IO.File.Open(STEM.Sys.IO.Path.AdjustPath(s), System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None))
                                        {
                                            string bucket = Authentication.BucketFromPath(dFile);
                                            string prefix = Authentication.PrefixFromPath(dFile);

                                            PutObjectRequest req = new PutObjectRequest { BucketName = bucket, AutoCloseStream = false, Key = prefix, InputStream = sStream };
                                            Authentication.Client.PutObjectAsync(req).Wait();
                                        }
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

                                        FDCFileInfo fi = Authentication.GetFileInfo(s);

                                        if (fi != null)
                                        {
                                            System.Threading.Tasks.Task<System.IO.Stream> streamResult = Authentication.Client.GetObjectStreamAsync(bucket, prefix, null);
                                            streamResult.Wait();

                                            using (System.IO.Stream sStream = streamResult.Result)
                                            {
                                                using (System.IO.Stream dStream = System.IO.File.Open(dFile, System.IO.FileMode.CreateNew, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None))
                                                {
                                                    sStream.CopyTo(dStream);
                                                }
                                            }

                                            File.SetLastWriteTimeUtc(dFile, fi.LastWriteTimeUtc);
                                        }
                                        else
                                        {
                                            throw new System.IO.FileNotFoundException(s);
                                        }
                                    }

                                    if (!String.IsNullOrEmpty(dFile))
                                    {
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
                                    if (DestinationActionRule == DestinationRule.AllOrNone)
                                        throw ex;
                                }
                            }

                            if (!success)
                                throw new Exception("No successful actions taken for " + s);

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
                                Exceptions.Add(e);
                            }
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
                    Exceptions.Add(e);
                }
            }
            catch (Exception ex)
            {
                AppendToMessage(ex.Message);
                Exceptions.Add(ex);
            }

            return Exceptions.Count == 0;
        }

        protected override void Dispose(bool dispose)
        {
            base.Dispose(dispose);

            Authentication.Dispose();
        }
    }
}
