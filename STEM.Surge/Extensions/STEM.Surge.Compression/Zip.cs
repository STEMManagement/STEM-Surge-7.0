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
using ICSharpCode.SharpZipLib.Zip;

namespace STEM.Surge.Compression
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Zip")]
    [Description("Zip a file or folder.")]
    public class Zip : Instruction
    {
        [Category("Source")]
        [DisplayName("Source Path"), DescriptionAttribute("The location of the file(s) to be zipped.")]
        public string SourcePath { get; set; }

        [Category("Source")]
        [DisplayName("Expand Source Path"), DescriptionAttribute("When searching for files, should the 'Source Path' be treated like an expandable?.")]
        public bool ExpandSource { get; set; }

        [Category("Source")]
        [DisplayName("File Filter"), DescriptionAttribute("The filter used on files in 'Source Path' to select files to zip. This can be a compound filter.")]
        public string FileFilter { get; set; }

        [Category("Source")]
        [DisplayName("Directory Filter"), DescriptionAttribute("The filter used on directories in 'Source Path' when 'Recurse Source' is specified. This can be a compound filter.")]
        public string DirectoryFilter { get; set; }

        [Category("Destination")]
        [DisplayName("Output File"), DescriptionAttribute("The location (path and filename) of the zipped output file.")]
        public string OutputFile { get; set; }

        [Category("Destination")]
        [DisplayName("Output File Exists Action"), DescriptionAttribute("What action should be taken if the output file already exists?")]
        public STEM.Sys.IO.FileExistsAction OutputFileExists { get; set; }

        [Category("Source")]
        [DisplayName("Recurse Source"), DescriptionAttribute("Recurse the source for files to zip?")]
        public bool RecurseSource { get; set; }

        [Category("Destination")]
        [DisplayName("Retain Directory Structure"), DescriptionAttribute("Retain the directory tree or flatten it to a single file listing?")]
        public bool RetainDirectoryStructure { get; set; }

        [Category("Destination")]
        [DisplayName("Allow Empty Zip Result"), DescriptionAttribute("When no source file(s) exist create an empty zip?")]
        public bool AllowEmptyZipResult { get; set; }

        [Category("Source")]
        [DisplayName("Delete Source"), DescriptionAttribute("Delete the source file(s) upon successful zip creation?")]
        public bool DeleteSource { get; set; }



        public Zip()
            : base()
        {
            SourcePath = "[TargetPath]";
            FileFilter = "[TargetName]";
            DirectoryFilter = "!TEMP";
            OutputFile = @"[DestinationPath]\[TargetNameWithoutExt].zip";
            OutputFileExists = Sys.IO.FileExistsAction.Throw;
            RecurseSource = false;
            RetainDirectoryStructure = true;
            ExpandSource = false;
            AllowEmptyZipResult = false;
            DeleteSource = false;
        }

        Dictionary<string, string> _Files = new Dictionary<string, string>();
        string _CreatedFile = "";

        protected override bool _Run()
        {
            SourcePath = STEM.Sys.IO.Path.AdjustPath(SourcePath);
            OutputFile = STEM.Sys.IO.Path.AdjustPath(OutputFile);

            string tmpFile = Path.Combine(Path.Combine(STEM.Sys.IO.Path.GetDirectoryName(OutputFile), "TEMP"), STEM.Sys.IO.Path.GetFileName(OutputFile));
            try
            {
                if (!Directory.Exists(STEM.Sys.IO.Path.GetDirectoryName(tmpFile)))
                    Directory.CreateDirectory(STEM.Sys.IO.Path.GetDirectoryName(tmpFile));

                if (File.Exists(tmpFile))
                    File.Delete(tmpFile);

                if (File.Exists(OutputFile))
                {
                    switch (OutputFileExists)
                    {
                        case Sys.IO.FileExistsAction.Throw:
                            throw new IOException("The output file already exists.");

                        case Sys.IO.FileExistsAction.Overwrite:
                            File.Delete(OutputFile);
                            break;

                        case Sys.IO.FileExistsAction.OverwriteIfNewer:

                            if (File.GetLastWriteTimeUtc(OutputFile) >= Directory.GetLastWriteTimeUtc(SourcePath))
                                return true;

                            File.Delete(OutputFile);
                            break;

                        case Sys.IO.FileExistsAction.Skip:
                            return true;
                    }
                }

                long inLen = 0;
                long outLen = 0;
                using (FileStream fs = File.Open(tmpFile, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None))
                {
                    using (ZipOutputStream zStream = new ZipOutputStream(fs))
                    {
                        zStream.IsStreamOwner = false;

                        foreach (string file in STEM.Sys.IO.Directory.STEM_GetFiles(SourcePath, FileFilter, DirectoryFilter, (RecurseSource ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly), ExpandSource))
                        {
                            string name = STEM.Sys.IO.Path.GetFileName(file);
                            if (RetainDirectoryStructure)
                                name = file.Replace(SourcePath, "");

                            name = name.Trim(Path.DirectorySeparatorChar);

                            if (InstructionSet.KeyManager.Lock(file))
                            {
                                try
                                {
                                    using (FileStream s = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                                    {
                                        inLen += s.Length;

                                        ZipEntry e = new ZipEntry(name);
                                        e.Size = s.Length;
                                        e.DateTime = File.GetLastWriteTimeUtc(file);

                                        zStream.PutNextEntry(e);

                                        s.CopyTo(zStream);

                                        zStream.CloseEntry();

                                        _Files[name] = file;
                                    }
                                }
                                finally
                                {
                                    if (!_Files.ContainsKey(name))
                                        InstructionSet.KeyManager.Unlock(file);
                                }
                            }
                        }
                    }

                    outLen = fs.Position;
                }

                bool goodOutput = (_Files.Count > 0) || AllowEmptyZipResult;

                if (goodOutput)
                {
                    if (File.Exists(OutputFile))
                    {
                        switch (OutputFileExists)
                        {
                            case Sys.IO.FileExistsAction.Throw:
                                throw new IOException("The output file already exists.");

                            case Sys.IO.FileExistsAction.Overwrite:
                                File.Delete(OutputFile);
                                break;

                            case Sys.IO.FileExistsAction.OverwriteIfNewer:

                                if (File.GetLastWriteTimeUtc(OutputFile) >= Directory.GetLastWriteTimeUtc(SourcePath))
                                    return true;

                                File.Delete(OutputFile);
                                break;

                            case Sys.IO.FileExistsAction.Skip:
                                return true;
                        }
                    }

                    STEM.Sys.IO.File.STEM_Move(tmpFile, OutputFile, OutputFileExists, out _CreatedFile);

                    if (DeleteSource)
                    {
                        foreach (string file in _Files.Values)
                        {
                            try
                            {
                                File.Delete(file);
                            }
                            catch
                            {
                            }
                        }
                    }

                    if (PopulatePostMortemMeta)
                    {
                        PostMortemMetaData["FilesArchived"] = _Files.Count.ToString();
                        PostMortemMetaData["OutputFilename"] = _CreatedFile;
                        PostMortemMetaData["InputBytes"] = inLen.ToString();
                        PostMortemMetaData["OutputBytes"] = outLen.ToString();
                    }
                }
                else
                {
                    File.Delete(tmpFile);
                }
            }
            catch (Exception ex)
            {
                AppendToMessage(ex.ToString());
                Exceptions.Add(ex);
            }
            finally
            {
                try
                {
                    if (File.Exists(tmpFile))
                        File.Delete(tmpFile);
                }
                catch { }

                foreach (string file in _Files.Values)
                    InstructionSet.KeyManager.Unlock(file);
            }

            return Exceptions.Count == 0;
        }

        protected override void _Rollback()
        {
            try
            {
                if (String.IsNullOrEmpty(_CreatedFile))
                    return;

                if (!File.Exists(_CreatedFile))
                    return;

                if (_Files.Count > 0)
                {
                    using (FileStream fs = File.Open(_CreatedFile, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        using (ZipInputStream zStream = new ZipInputStream(fs))
                        {
                            zStream.IsStreamOwner = false;

                            ZipEntry e = null;
                            while ((e = zStream.GetNextEntry()) != null)
                            {
                                try
                                {
                                    string file = _Files[e.Name];

                                    if (!File.Exists(file))
                                    {
                                        if (!Directory.Exists(STEM.Sys.IO.Path.GetDirectoryName(file)))
                                            Directory.CreateDirectory(STEM.Sys.IO.Path.GetDirectoryName(file));

                                        using (FileStream s = File.Open(file, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None))
                                        {
                                            zStream.CopyTo(s);
                                        }

                                        File.SetLastWriteTimeUtc(file, e.DateTime);
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }

                _Files.Clear();
            }
            catch (Exception ex)
            {
                AppendToMessage(ex.ToString());
                Exceptions.Add(ex);
                _CreatedFile = "";
            }
            finally
            {
                try
                {
                    if (!String.IsNullOrEmpty(_CreatedFile))
                        if (_Files.Count == 0)
                            if (File.Exists(_CreatedFile))
                                File.Delete(_CreatedFile);
                }
                catch (Exception ex)
                {
                    AppendToMessage(ex.ToString());
                    Exceptions.Add(ex);
                }
            }
        }
    }
}
