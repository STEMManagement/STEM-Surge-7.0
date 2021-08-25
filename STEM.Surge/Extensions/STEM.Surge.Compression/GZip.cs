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

        [Category("Source")]
        [DisplayName("Delete Source"), DescriptionAttribute("Delete the source file upon successful gz creation?")]
        public bool DeleteSource { get; set; }

        public GZip()
            : base()
        {
            SourceFile = "[TargetPath]\\[TargetName]";
            OutputFile = @"[DestinationPath]\[TargetNameWithoutExt].gz";
            OutputFileExists = Sys.IO.FileExistsAction.Throw;
            DeleteSource = false;
        }

        string _CreatedFile = "";
        bool _LockOwner = false;

        protected override bool _Run()
        {
            SourceFile = STEM.Sys.IO.Path.AdjustPath(SourceFile);
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

                            if (File.GetLastWriteTimeUtc(OutputFile) >= File.GetLastWriteTimeUtc(SourceFile))
                                return true;

                            File.Delete(OutputFile);
                            break;

                        case Sys.IO.FileExistsAction.Skip:
                            return true;
                    }
                }

                long inLen = 0;
                long outLen = 0;

                if (InstructionSet.KeyManager.Lock(SourceFile))
                {
                    _LockOwner = true;

                    using (FileStream fs = File.Open(tmpFile, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None))
                    {
                        using (GZipOutputStream zStream = new GZipOutputStream(fs))
                        {
                            zStream.IsStreamOwner = false;
                            if (File.Exists(SourceFile))
                            {
                                using (FileStream s = File.Open(SourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    inLen = s.Length;
                                    s.CopyTo(zStream);
                                }
                            }
                            else
                            {
                                throw new FileNotFoundException(SourceFile + " does not exist.");
                            }
                        }

                        outLen = fs.Position;
                    }
                }

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
                    }
                }

                STEM.Sys.IO.File.STEM_Move(tmpFile, OutputFile, OutputFileExists, out _CreatedFile);

                if (DeleteSource)
                {
                    try
                    {
                        File.Delete(SourceFile);
                    }
                    catch
                    {
                    }
                }

                if (PopulatePostMortemMeta)
                {
                    PostMortemMetaData["OutputFilename"] = _CreatedFile;
                    PostMortemMetaData["InputBytes"] = inLen.ToString();
                    PostMortemMetaData["OutputBytes"] = outLen.ToString();
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

                if (_LockOwner)
                    InstructionSet.KeyManager.Unlock(SourceFile);
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

                if (File.Exists(_CreatedFile))
                {
                    if (!File.Exists(SourceFile))
                    {
                        using (FileStream fs = File.Open(_CreatedFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
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
                }
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
