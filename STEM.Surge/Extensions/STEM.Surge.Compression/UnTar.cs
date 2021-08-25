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
    [DisplayName("UnTar")]
    [Description("UnTar a file.")]
    public class UnTar : Instruction
    {
        [Category("Source")]
        [DisplayName("Source File"), DescriptionAttribute("The path and filename of the tar file to be un-tared.")]
        public string SourceFile { get; set; }

        [Category("Destination")]
        [DisplayName("Destination Path"), DescriptionAttribute("The location where the un-tared file(s) are to be placed.")]
        public string DestinationPath { get; set; }

        [Category("Destination")]
        [DisplayName("Output File Exists Action"), DescriptionAttribute("What action should be taken if the output file already exists?")]
        public STEM.Sys.IO.FileExistsAction OutputFileExists { get; set; }

        public UnTar()
            : base()
        {
            SourceFile = @"[TargetPath]\[TargetName]";
            DestinationPath = "[DestinationPath]";
            OutputFileExists = Sys.IO.FileExistsAction.Throw;
        }

        List<string> _Files = new List<string>();
        List<string> _Directories = new List<string>();

        protected override bool _Run()
        {
            SourceFile = STEM.Sys.IO.Path.AdjustPath(SourceFile);
            DestinationPath = STEM.Sys.IO.Path.AdjustPath(DestinationPath);

            try
            {
                if (!Directory.Exists(STEM.Sys.IO.Path.GetDirectoryName(DestinationPath)))
                    Directory.CreateDirectory(STEM.Sys.IO.Path.GetDirectoryName(DestinationPath));

                using (FileStream fs = File.Open(SourceFile, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    using (TarInputStream tStream = new TarInputStream(fs))
                    {
                        tStream.IsStreamOwner = false;

                        TarEntry e = null;
                        while ((e = tStream.GetNextEntry()) != null)
                        {
                            string outputFile = Path.Combine(DestinationPath, e.Name);

                            if (e.IsDirectory)
                            {
                                if (!Directory.Exists(Path.Combine(DestinationPath, e.Name)))
                                    Directory.CreateDirectory(Path.Combine(DestinationPath, e.Name));

                                _Directories.Add(Path.Combine(DestinationPath, e.Name));

                                continue;
                            }

                            if (!Directory.Exists(STEM.Sys.IO.Path.GetDirectoryName(outputFile)))
                            {
                                Directory.CreateDirectory(STEM.Sys.IO.Path.GetDirectoryName(outputFile));
                                _Directories.Add(STEM.Sys.IO.Path.GetDirectoryName(outputFile));
                            }

                            if (File.Exists(outputFile))
                            {
                                switch (OutputFileExists)
                                {
                                    case Sys.IO.FileExistsAction.Throw:
                                        throw new IOException("The output file already exists (" + outputFile + ").");

                                    case Sys.IO.FileExistsAction.Overwrite:
                                        File.Delete(outputFile);
                                        break;

                                    case Sys.IO.FileExistsAction.OverwriteIfNewer:

                                        if (File.GetLastWriteTimeUtc(outputFile) >= File.GetLastWriteTimeUtc(SourceFile))
                                            continue;

                                        File.Delete(outputFile);
                                        break;

                                    case Sys.IO.FileExistsAction.Skip:
                                        continue;

                                    case Sys.IO.FileExistsAction.MakeUnique:
                                        outputFile = STEM.Sys.IO.File.UniqueFilename(outputFile);
                                        break;
                                }
                            }

                            _Files.Add(outputFile);

                            using (FileStream s = File.Open(outputFile, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None))
                            {
                                tStream.CopyEntryContents(s);
                            }
                            
                            File.SetLastWriteTimeUtc(outputFile, e.ModTime);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppendToMessage(ex.ToString());
                Exceptions.Add(ex);
            }

            return Exceptions.Count == 0;
        }

        protected override void _Rollback()
        {
            try
            {
                foreach (string file in _Files)
                    try
                    {
                        if (File.Exists(file))
                            File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        AppendToMessage(ex.ToString());
                        Exceptions.Add(ex);
                    }

                foreach (string dir in _Directories)
                    try
                    {
                        Directory.Delete(dir, true);
                    }
                    catch { }
            }
            catch (Exception ex)
            {
                AppendToMessage(ex.ToString());
                Exceptions.Add(ex);
            }
        }
    }
}
