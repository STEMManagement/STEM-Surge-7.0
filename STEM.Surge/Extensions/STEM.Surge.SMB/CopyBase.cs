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
using System.Linq;
using System.IO;
using STEM.Surge;

namespace STEM.Surge.SMB
{   
    public abstract class CopyBase : Instruction
    {
        protected enum ActionType { Copy, Move }
        public enum DestinationRule { FirstSuccess, AllOrNone, OneOrMore }
        public enum ExecutoOn { ForwardExecution, Rollback }

        protected ActionType Action { get; set; }


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
        [DisplayName("Use a TEMP directory"), DescriptionAttribute("Should I a copy to a TEMP subdirectory under the destination, then a move up one level.")]
        public bool UseTempHop { get; set; }

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

        [Category("Flow")]
        [DisplayName("Zero Files Action"), Description("What flow action should be taken if no files are found?")]
        public FailureAction ZeroFilesAction { get; set; }

        public CopyBase()
            : base()
        {
            ExistsAction = STEM.Sys.IO.FileExistsAction.MakeUnique;
            Retry = 1;
            RetryDelaySeconds = 2;

            DestinationActionRule = DestinationRule.AllOrNone;

            ExpandSource = false;
            SourcePath = "[TargetPath]";
            FileFilter = "[TargetName]";
            DirectoryFilter = "!TEMP";
            RecurseSource = false;
            RecreateTree = false;

            ExpandDestination = false;
            UseTempHop = true;
            DestinationPath = "[DestinationPath]\\[SubDir]";
            DestinationFilename = "*.*";

            ExecutionMode = ExecutoOn.ForwardExecution;
            ZeroFilesAction = FailureAction.SkipRemaining;
        }
        
        Dictionary<string, string> _FilesActioned = new Dictionary<string, string>();

        protected override void _Rollback()
        {
            if (ExecutionMode == ExecutoOn.ForwardExecution)
            {
                foreach (string d in _FilesActioned.Keys)
                {
                    string dFile = "";
                    try
                    {
                        string s = _FilesActioned[d];

                        if (Action == ActionType.Move)
                            STEM.Sys.IO.File.STEM_Move(s, d, STEM.Sys.IO.FileExistsAction.Skip, out dFile, Retry, RetryDelaySeconds, UseTempHop);
                        
                        STEM.Sys.IO.File.STEM_Delete(s, false, Retry, RetryDelaySeconds);
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
                    Random rnd = new Random();
                    destinations = STEM.Sys.IO.Path.ExpandRangedPath(DestinationPath).OrderBy(i => rnd.Next()).ToList();
                }
                else
                {
                    destinations.Add(DestinationPath);
                }

                int filesActioned = 0;

                foreach (string src in sources)
                {
                    foreach (string s in STEM.Sys.IO.Directory.STEM_GetFiles(src, FileFilter, DirectoryFilter, (RecurseSource ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly), false))
                    {
                        try
                        {
                            bool success = false;

                            foreach (string d in destinations)
                            {
                                try
                                {
                                    string dFile = "";
                                    try
                                    {
                                        if (PopulatePostMortemMeta)
                                        {
                                            PostMortemMetaData["SourceIP"] = STEM.Sys.IO.Path.IPFromPath(s);
                                            PostMortemMetaData["DestinationIP"] = STEM.Sys.IO.Path.IPFromPath(d);
                                            PostMortemMetaData["FileSize"] = new FileInfo(s).Length.ToString();
                                        }
                                    }
                                    catch { }

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

                                    STEM.Sys.IO.File.STEM_Copy(s, dPath, ExistsAction, out dFile, Retry, RetryDelaySeconds, UseTempHop);

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
                                    if (DestinationActionRule == DestinationRule.AllOrNone)
                                        throw ex;
                                }
                            }

                            if (!success)
                                throw new Exception("No successful actions taken for " + s);

                            if (Action == ActionType.Move)
                                File.Delete(STEM.Sys.IO.Path.AdjustPath(s));
                        }
                        catch (Exception ex)
                        {
                            AppendToMessage(ex.Message);
                            Exceptions.Add(ex);
                        }
                    }

                    if (PopulatePostMortemMeta)
                    {
                        PostMortemMetaData["FilesActioned"] = filesActioned.ToString();
                    }
                }
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
                        Exceptions.Clear();
                        SkipRemaining();
                        break;

                    case FailureAction.SkipNext:
                        Exceptions.Clear();
                        SkipNext();
                        break;

                    case FailureAction.SkipToLabel:
                        Exceptions.Clear();
                        SkipForwardToFlowControlLabel(FailureActionLabel);
                        break;

                    case FailureAction.Rollback:
                        RollbackAllPreceedingAndSkipRemaining();
                        break;

                    case FailureAction.Continue:
                        Exceptions.Clear();
                        break;
                }

                Message = "0 Files Actioned\r\n" + Message;
            }

            return Exceptions.Count == 0;
        }
    }
}
