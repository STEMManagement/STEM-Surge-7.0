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
using System.Linq;
using System.IO;
using System.Xml.Serialization;

namespace STEM.Sys.IO
{
    public class FileDescription
    {
        public FileDescription()
        {
            Filename = "";
            Filepath = "";
            LastWriteTimeUtc = CreationTimeUtc = DateTime.MinValue;
            Content = null;
        }

        public FileDescription(string filePath, string fileName, bool loadContent)
        {
            Filename = STEM.Sys.IO.Path.AdjustPath(fileName);
            Filepath = STEM.Sys.IO.Path.AdjustPath(filePath);

            FileInfo fi = new FileInfo(System.IO.Path.Combine(Filepath, Filename));
            LastWriteTimeUtc = fi.LastWriteTimeUtc;
            CreationTimeUtc = fi.CreationTimeUtc;

            if (loadContent)
                Content = System.IO.File.ReadAllBytes(System.IO.Path.Combine(Filepath, Filename));
        }

        public DateTime LastWriteTimeUtc { get; set; }
        public DateTime CreationTimeUtc { get; set; }
        public string Filepath { get; set; }
        public string Filename { get; set; }

        string _StringContent = null;
        [XmlIgnore]
        public string StringContent
        {
            get
            {
                lock (this)
                {
                    if (Content == null)
                        return null;

                    if (_StringContent == null)
                        _StringContent = System.Text.Encoding.UTF8.GetString(Content);

                    return _StringContent;
                }
            }

            set
            {
                lock (this)
                {
                    Content = System.Text.Encoding.UTF8.GetBytes(value);
                    _StringContent = value;
                }
            }
        }

        byte[] _Content = null;
        [XmlIgnore]
        public byte[] Content
        {
            get
            {
                lock (this)
                    return _Content;
            }

            set
            {
                lock (this)
                {
                    _Content = value;
                    _CompressedContent = null;
                    _StringContent = null;
                }
            }
        }

        string _CompressedContent = null;
        public string CompressedContent
        {
            get
            {
                lock (this)
                {
                    if (_Content == null)
                        return "";

                    if (_CompressedContent == null)
                        _CompressedContent = System.Convert.ToBase64String(STEM.Sys.IO.ByteCompression.Compress(_Content, _Content.Length));

                    return _CompressedContent;
                }
            }

            set
            {
                lock (this)
                {
                    if (String.IsNullOrEmpty(value))
                        _Content = null;
                    else if (_Content == null)
                    {
                        byte[] c = System.Convert.FromBase64String(value);
                        _Content = STEM.Sys.IO.ByteCompression.Decompress(c, c.Length);
                    }
                }
            }
        }

        public void CopyFrom(FileDescription source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            lock (this)
            {
                CreationTimeUtc = source.CreationTimeUtc;
                LastWriteTimeUtc = source.LastWriteTimeUtc;
                Filename = STEM.Sys.IO.Path.AdjustPath(source.Filename);
                Filepath = STEM.Sys.IO.Path.AdjustPath(source.Filepath);
                _Content = source._Content;
                _CompressedContent = source._CompressedContent;
                _StringContent = source._StringContent;
            }
        }

        public string Save(STEM.Sys.IO.FileExistsAction fileExistsAction)
        {
            lock (this)
                try
                {
                    if (String.IsNullOrEmpty(Filepath) || String.IsNullOrEmpty(Filename))
                        return null;

                    string file = STEM.Sys.IO.Path.AdjustPath(System.IO.Path.Combine(Filepath, Filename));

                    if (!System.IO.Directory.Exists(STEM.Sys.IO.Path.GetDirectoryName(file)))
                        System.IO.Directory.CreateDirectory(STEM.Sys.IO.Path.GetDirectoryName(file));

                    if (_Content != null)
                    {
                        if (System.IO.File.Exists(file))
                            switch (fileExistsAction)
                            {
                                case FileExistsAction.Skip:
                                    return null;

                                case FileExistsAction.Throw:
                                    throw new Exception("File " + file + " already exists.");

                                case FileExistsAction.MakeUnique:
                                    file = STEM.Sys.IO.File.UniqueFilename(file);
                                    break;
                            }

                        System.IO.File.WriteAllBytes(file, _Content);
                        System.IO.File.SetCreationTimeUtc(file, CreationTimeUtc);
                        System.IO.File.SetLastWriteTimeUtc(file, LastWriteTimeUtc);

                        return file;
                    }
                }
                catch (Exception ex)
                {
                    if (fileExistsAction == FileExistsAction.Throw)
                        throw new Exception("FileDescription.Save", ex);
                }

            return null;
        }

        public void Refresh(string filePath, string fileName)
        {
            lock (this)
            {
                Filename = STEM.Sys.IO.Path.AdjustPath(fileName);
                Filepath = STEM.Sys.IO.Path.AdjustPath(filePath);

                FileInfo fi = new FileInfo(System.IO.Path.Combine(Filepath, Filename));
                LastWriteTimeUtc = fi.LastWriteTimeUtc;
                CreationTimeUtc = fi.CreationTimeUtc;
                Content = System.IO.File.ReadAllBytes(System.IO.Path.Combine(Filepath, Filename));
            }
        }
    }
}
