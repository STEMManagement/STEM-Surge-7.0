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
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

namespace STEM.Surge.Compression
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("GZip")]
    [Description("GZip a file.")]
    public class GZip : Instruction
    {
        [Category("Source")]
        [DisplayName("Source File"), DescriptionAttribute("The path and filename of the file to be zipped.")]
        public string SourceFile { get; set; }
        
        [Category("Destination")]
        [DisplayName("Output File"), DescriptionAttribute("The location (path and filename) of the zipped output file.")]
        public string OutputFile { get; set; }

        [Category("Destination")]
        [DisplayName("Output File Exists Action"), DescriptionAttribute("What action should be taken if the output file already exists?")]
        public STEM.Sys.IO.FileExistsAction OutputFileExists { get; set; }

        public GZip()
            : base()
        {
            SourceFile = "[TargetPath]\\[TargetName]";
            OutputFile = @"[DestinationPath]\[TargetNameWithoutExt].tar.gz";
            OutputFileExists = Sys.IO.FileExistsAction.Throw;
        }
        
        protected override bool _Run()
        {
            SourceFile = STEM.Sys.IO.Path.AdjustPath(SourceFile);
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

                            if (File.GetLastWriteTimeUtc(OutputFile) >= File.GetLastWriteTimeUtc(SourceFile))
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
                    using (GZipOutputStream zStream = new GZipOutputStream(fs))
                    {
                        zStream.IsStreamOwner = false;
                        if (File.Exists(SourceFile))
                        {
                            string name = STEM.Sys.IO.Path.GetFileName(SourceFile).Trim(Path.DirectorySeparatorChar);

                            using (FileStream s = File.Open(SourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                s.CopyTo(zStream);
                            }
                        }
                        else
                        {
                            throw new FileNotFoundException(SourceFile + " does not exist.");
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
                if (File.Exists(OutputFile))
                {
                    if (!File.Exists(SourceFile))
                    {
                        using (FileStream fs = File.Open(OutputFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                        {
                            using (GZipInputStream zStream = new GZipInputStream(fs))
                            {
                                zStream.IsStreamOwner = false;

                                using (FileStream os = File.Open(SourceFile, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None))
                                {
                                    zStream.CopyTo(os);
                                }
                            }
                        }
                    }

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
