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
using ICSharpCode.SharpZipLib.Tar;

namespace STEM.Surge.Compression
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Tar")]
    [Description("Tar a file or folder.")]
    public class Tar : Instruction
    {
        [Category("Source")]
        [DisplayName("Source Path"), DescriptionAttribute("The location of the file(s) to tar.")]
        public string SourcePath { get; set; }

        [Category("Source")]
        [DisplayName("Expand Source Path"), DescriptionAttribute("When searching for files, should the 'Source Path' be treated like an expandable?.")]
        public bool ExpandSource { get; set; }

        [Category("Source")]
        [DisplayName("File Filter"), DescriptionAttribute("The filter used on files in 'Source Path' to select files to tar. This can be a compound filter.")]
        public string FileFilter { get; set; }

        [Category("Source")]
        [DisplayName("Directory Filter"), DescriptionAttribute("The filter used on directories in 'Source Path' when 'Recurse Source' is specified. This can be a compound filter.")]
        public string DirectoryFilter { get; set; }

        [Category("Destination")]
        [DisplayName("Output File"), DescriptionAttribute("The location (path and filename) of the tar output file.")]
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

        public Tar()
            : base()
        {
            SourcePath = "[TargetPath]";
            FileFilter = "[TargetName]";
            DirectoryFilter = "!TEMP";
            OutputFile = @"[DestinationPath]\[TargetNameWithoutExt].tar";
            OutputFileExists = Sys.IO.FileExistsAction.Throw;
            RecurseSource = false;
            RetainDirectoryStructure = true;
            ExpandSource = false;
        }

        Dictionary<string, string> _Files = new Dictionary<string, string>();

        protected override bool _Run()
        {
            SourcePath = STEM.Sys.IO.Path.AdjustPath(SourcePath);
            OutputFile = STEM.Sys.IO.Path.AdjustPath(OutputFile);

            string tmpFile = Path.Combine(Path.Combine(STEM.Sys.IO.Path.GetDirectoryName(OutputFile), "Temp"), STEM.Sys.IO.Path.GetFileName(OutputFile));
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

                        case Sys.IO.FileExistsAction.MakeUnique:
                            OutputFile = STEM.Sys.IO.File.UniqueFilename(OutputFile);
                            break;
                    }
                }

                using (FileStream fs = File.Open(tmpFile, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None))
                {
                    using (TarOutputStream tStream = new TarOutputStream(fs))
                    {
                        tStream.IsStreamOwner = false;

                        foreach (string file in STEM.Sys.IO.Directory.STEM_GetFiles(SourcePath, FileFilter, DirectoryFilter, (RecurseSource ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly), ExpandSource))
                        {
                            string name = STEM.Sys.IO.Path.GetFileName(file);
                            if (RetainDirectoryStructure)
                                name = file.Replace(SourcePath, "");

                            name = name.Trim(Path.DirectorySeparatorChar);

                            TarEntry e = TarEntry.CreateEntryFromFile(file);
                            e.Name = name;
                            e.ModTime = File.GetLastWriteTimeUtc(file);

                            using (FileStream s = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                tStream.PutNextEntry(e);

                                s.CopyTo(tStream);

                                tStream.CloseEntry();

                                _Files[name] = file;
                            }
                        }
                    }
                }

                File.Move(tmpFile, OutputFile);
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
            }

            return Exceptions.Count == 0;
        }

        protected override void _Rollback()
        {
            try
            {
                if (_Files.Count > 0)
                {
                    using (FileStream fs = File.Open(OutputFile, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        using (TarInputStream tStream = new TarInputStream(fs))
                        {
                            tStream.IsStreamOwner = false;

                            TarEntry e = null;
                            while ((e = tStream.GetNextEntry()) != null)
                            {
                                if (!e.IsDirectory)
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
                                                tStream.CopyEntryContents(s);
                                            }

                                            File.SetLastWriteTimeUtc(file, e.ModTime);
                                        }
                                    }
                                    catch { }
                                }
                                else
                                {
                                    try
                                    {
                                        if (!Directory.Exists(Path.Combine(SourcePath, e.Name)))
                                            Directory.CreateDirectory(Path.Combine(SourcePath, e.Name));
                                    }
                                    catch { }
                                }
                            }
                        }
                    }

                    if (File.Exists(OutputFile))
                        File.Delete(OutputFile);
                }
            }
            catch (Exception ex)
            {
                AppendToMessage(ex.ToString());
                Exceptions.Add(ex);
            }
        }
    }
}
