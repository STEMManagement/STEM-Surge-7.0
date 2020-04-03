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
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace STEM.Surge.SSH
{
    public abstract class CopyBase : STEM.Surge.Instruction
    {
        protected enum ActionType { Copy, Move }
        public enum DestinationRule { FirstSuccess, AllOrNone, OneOrMore }
        public enum SshDirection { ToSshServer, FromSshServer }

        protected ActionType Action { get; set; }

        [Category("SSH Server")]
        [DisplayName("Authentication"), DescriptionAttribute("The authentication configuration to be used.")]
        public Authentication Authentication { get; set; }

        [Category("SSH Server")]
        [DisplayName("SSH Server Address"), DescriptionAttribute("What is the SSH Server Address?")]
        public string ServerAddress { get; set; }

        [Category("SSH Server")]
        [DisplayName("SSH Port"), DescriptionAttribute("What is the SSH Port?")]
        public string Port { get; set; }

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

        [Category("SSH Server")]
        [DisplayName("SSH Direction"), DescriptionAttribute("Is the action to or from an SSH server?")]
        public SshDirection Direction { get; set; }

        [Category("Flow")]
        [DisplayName("Zero Files Action"), Description("What flow action should be taken if no files are found?")]
        public FailureAction ZeroFilesAction { get; set; }

        public CopyBase()
            : base()
        {
            Authentication = new Authentication();
            ServerAddress = "[SshServerAddress]";
            Port = "[SshServerPort]";

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
        }

        Dictionary<string, string> _FilesActioned = new Dictionary<string, string>();
        string _Address = null;

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
                            SftpClient client = Authentication.OpenSftpClient(_Address, Int32.Parse(Port));

                            try
                            {
                                if (Direction == SshDirection.ToSshServer)
                                {
                                    using (System.IO.Stream dStream = System.IO.File.Open(STEM.Sys.IO.Path.AdjustPath(d), System.IO.FileMode.CreateNew, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None))
                                    {
                                        client.DownloadFile(Authentication.AdjustPath(_Address, d), dStream);
                                    }
                                }
                                else
                                {
                                    using (System.IO.Stream sStream = System.IO.File.Open(STEM.Sys.IO.Path.AdjustPath(s), System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None))
                                    {
                                        client.UploadFile(sStream, Authentication.AdjustPath(_Address, d));
                                    }
                                }
                            }
                            finally
                            {
                                Authentication.RecycleClient(client);
                            }
                        }

                        if (Direction == SshDirection.ToSshServer)
                        {
                            Authentication.DeleteFile(_Address, Int32.Parse(Port), s);
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
                int r = Retry;
                while (r-- >= 0 && !Stop)
                {
                    _Address = Authentication.NextAddress(ServerAddress);

                    if (_Address == null)
                    {
                        Exception ex = new Exception("No valid address. (" + ServerAddress + ")");
                        Exceptions.Add(ex);
                        AppendToMessage(ex.Message);
                        return;
                    }

                    Exceptions.Clear();
                    Message = "";
                    bool success = Execute();
                    if (success)
                        return;

                    System.Threading.Thread.Sleep(RetryDelaySeconds * 1000);
                }
            }
        }

        protected override bool _Run()
        {
            if (ExecutionMode == ExecuteOn.ForwardExecution)
            {
                int r = Retry;
                while (r-- >= 0 && !Stop)
                {
                    _Address = Authentication.NextAddress(ServerAddress);

                    if (_Address == null)
                    {
                        Exception ex = new Exception("No valid address. (" + ServerAddress + ")");
                        Exceptions.Add(ex);
                        AppendToMessage(ex.Message);
                        return false;
                    }

                    Exceptions.Clear();
                    Message = "";
                    bool success = Execute();
                    if (success)
                        return true;

                    System.Threading.Thread.Sleep(RetryDelaySeconds * 1000);
                }

                return false;
            }

            return true;
        }

        bool Execute()
        {
            try
            {
                if (Direction == SshDirection.ToSshServer)
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
                    List<SftpFile> items = new List<SftpFile>();

                    if (Direction == SshDirection.ToSshServer)
                    {
                        PostMortemMetaData["LastOperation"] = "STEM_GetFiles:SourceFile";

                        sourceFiles = STEM.Sys.IO.Directory.STEM_GetFiles(src, FileFilter, DirectoryFilter, (RecurseSource ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly), false);
                    }
                    else
                    {
                        PostMortemMetaData["LastOperation"] = "ListDirectory:SourceFile";

                        items = Authentication.ListDirectory(_Address, Int32.Parse(Port), src, SSHListType.File, RecurseSource, DirectoryFilter, FileFilter);

                        sourceFiles = items.Select(i => i.FullName).ToList();
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

                                    try
                                    {
                                        if (PopulatePostMortemMeta)
                                        {
                                            PostMortemMetaData["SourceIP"] = STEM.Sys.IO.Path.IPFromPath(s);
                                            PostMortemMetaData["DestinationIP"] = STEM.Sys.IO.Path.IPFromPath(d);

                                            PostMortemMetaData["LastOperation"] = "GetFileInfo:SourceFile";

                                            if (Direction == SshDirection.ToSshServer)
                                                PostMortemMetaData["FileSize"] = new FileInfo(s).Length.ToString();
                                            else
                                                PostMortemMetaData["FileSize"] = Authentication.GetFileInfo(_Address, Int32.Parse(Port), s).Size.ToString();
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

                                    if (Direction == SshDirection.ToSshServer)
                                    {
                                        string directory = STEM.Sys.IO.Path.GetDirectoryName(dPath);

                                        PostMortemMetaData["LastOperation"] = "DirectoryExists:DestinationDirectory";

                                        if (!Authentication.DirectoryExists(_Address, Int32.Parse(Port), directory))
                                            Authentication.CreateDirectory(_Address, Int32.Parse(Port), directory);

                                        PostMortemMetaData["LastOperation"] = "FileExists:DestinationFile";

                                        if (Authentication.FileExists(_Address, Int32.Parse(Port), dPath))
                                        {
                                            switch (ExistsAction)
                                            {
                                                case Sys.IO.FileExistsAction.Overwrite:
                                                    Authentication.DeleteFile(_Address, Int32.Parse(Port), dPath);
                                                    dFile = dPath;
                                                    break;

                                                case Sys.IO.FileExistsAction.Skip:
                                                    continue;

                                                case Sys.IO.FileExistsAction.Throw:
                                                    throw new IOException("Destination file exists. (" + dPath + ")");

                                                case Sys.IO.FileExistsAction.MakeUnique:
                                                    dFile = Authentication.UniqueFilename(_Address, Int32.Parse(Port), dPath);
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            dFile = dPath;
                                        }

                                        DateTime mt = File.GetLastWriteTimeUtc(s);

                                        PostMortemMetaData["LastOperation"] = "OpenSftpClient";

                                        SftpClient client = Authentication.OpenSftpClient(_Address, Int32.Parse(Port));

                                        try
                                        {
                                            PostMortemMetaData["LastOperation"] = "UploadFile";

                                            using (System.IO.Stream sStream = System.IO.File.Open(STEM.Sys.IO.Path.AdjustPath(s), System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None))
                                            {
                                                client.UploadFile(sStream, Authentication.AdjustPath(_Address, dPath));
                                                //client.SetLastWriteTimeUtc(Authentication.AdjustPath(_Address, dPath), mt);
                                            }
                                        }
                                        finally
                                        {
                                            PostMortemMetaData["LastOperation"] = "RecycleClient";
                                            Authentication.RecycleClient(client);
                                        }
                                    }
                                    else
                                    {
                                        PostMortemMetaData["LastOperation"] = "File.Exists:DestinationFile";
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

                                        PostMortemMetaData["LastOperation"] = "Directory.Exists:DestinationDirectory";

                                        if (!Directory.Exists(STEM.Sys.IO.Path.GetDirectoryName(dFile)))
                                            Directory.CreateDirectory(STEM.Sys.IO.Path.GetDirectoryName(dFile));

                                        SftpClient client = Authentication.OpenSftpClient(_Address, Int32.Parse(Port));

                                        try
                                        {
                                            PostMortemMetaData["LastOperation"] = "DownloadFile";

                                            using (System.IO.Stream dStream = System.IO.File.Open(dFile, System.IO.FileMode.CreateNew, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None))
                                            {
                                                client.DownloadFile(Authentication.AdjustPath(_Address, s), dStream);
                                            }
                                        }
                                        finally
                                        {
                                            PostMortemMetaData["LastOperation"] = "RecycleClient";
                                            Authentication.RecycleClient(client);
                                        }

                                        PostMortemMetaData["LastOperation"] = "SetLastWriteTimeUtc:DestinationFile";
                                        try
                                        {
                                            File.SetLastWriteTimeUtc(dFile, Authentication.GetFileInfo(_Address, Int32.Parse(Port), s).LastWriteTimeUtc);
                                        }
                                        catch { }
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
                                throw new Exception("No successful actions taken for " + s, lastEX);

                            if (Action == ActionType.Move)
                            {
                                if (Direction == SshDirection.ToSshServer)
                                {
                                    PostMortemMetaData["LastOperation"] = "File.Delete:SourceFile";
                                    File.Delete(s);
                                }
                                else
                                {
                                    PostMortemMetaData["LastOperation"] = "DeleteFile:SourceFile";
                                    Authentication.DeleteFile(_Address, Int32.Parse(Port), s);
                                }
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

        protected override void Dispose(bool dispose)
        {
            base.Dispose(dispose);

            Authentication.Dispose();
        }
    }
}

