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
using System.Xml.Serialization;
using STEM.Sys.IO.TCP;
using STEM.Sys.Messaging;

namespace STEM.Surge.Messages
{
    public class FileTransfer : STEM.Sys.Messaging.Message
    {
        internal string _SourceFile = null;

        public static DateTime MinValue = DateTime.Parse(DateTime.MinValue.ToString("G", System.Globalization.CultureInfo.CurrentCulture), System.Globalization.CultureInfo.CurrentCulture);

        public DateTime DeletedTimeUtc { get; set; }
        public DateTime CreationTimeUtc { get; set; }
        public DateTime LastWriteTimeUtc { get; set; }
        public bool OnlySaveIfNewer { get; set; }
        public bool Compressed { get; set; }
        public string DestinationPath { get; set; }
        public string DestinationFilename { get; set; }
        public STEM.Sys.IO.FileExistsAction FileExistsAction { get; set; }

        public string FileContent { get; set; }

        public delegate void FileActivity(MessageConnection sender, FileTransfer copyOfMessage);

        public event FileActivity onFileDeleting;
        public event FileActivity onFileDeleted;
        public event FileActivity onFileAdding;
        public event FileActivity onFileAdded;
        public event FileActivity onFileModifying;
        public event FileActivity onFileModified;

        public FileTransfer()
        {
            DeletedTimeUtc = MinValue;
            OnlySaveIfNewer = true;
            Compressed = true;
        }

        public FileTransfer(string sourceFile, string destinationPath, string destinationFilename, STEM.Sys.IO.FileExistsAction fileExistsAction, bool compress, bool onlySaveIfNewer)
        {
            _SourceFile = sourceFile;

            CreationTimeUtc = LastWriteTimeUtc = DeletedTimeUtc = MinValue;

            OnlySaveIfNewer = onlySaveIfNewer;
            Compressed = compress;

            FileExistsAction = fileExistsAction;

            DestinationPath = STEM.Sys.IO.Path.AdjustPath(destinationPath);
            DestinationFilename = STEM.Sys.IO.Path.AdjustPath(destinationFilename);

            RefreshContentFromFile();
        }

        public FileTransfer(DateTime creationTimeUtc, DateTime lastWriteTimeUtc, string destinationPath, string destinationFilename, byte[] fileContent, STEM.Sys.IO.FileExistsAction fileExistsAction, bool compress, bool onlySaveIfNewer)
        {
            _SourceFile = null;

            DeletedTimeUtc = MinValue;

            FileExistsAction = fileExistsAction;

            CreationTimeUtc = creationTimeUtc;
            LastWriteTimeUtc = lastWriteTimeUtc;

            CreationTimeUtc = DateTime.Parse(CreationTimeUtc.ToString("G", System.Globalization.CultureInfo.CurrentCulture), System.Globalization.CultureInfo.CurrentCulture);
            LastWriteTimeUtc = DateTime.Parse(LastWriteTimeUtc.ToString("G", System.Globalization.CultureInfo.CurrentCulture), System.Globalization.CultureInfo.CurrentCulture);

            if (Compressed)
            {
                byte[] cmp = STEM.Sys.IO.ByteCompression.Compress(fileContent, fileContent.Length);

                if (cmp == null)
                    cmp = new byte[0];

                FileContent = Convert.ToBase64String(cmp);
            }
            else
            {
                FileContent = Convert.ToBase64String(fileContent);
            }

            DestinationPath = STEM.Sys.IO.Path.AdjustPath(destinationPath);
            DestinationFilename = STEM.Sys.IO.Path.AdjustPath(destinationFilename);

            OnlySaveIfNewer = onlySaveIfNewer;
            Compressed = compress;
        }

        bool _Saved = false;
        string _SavedFile = null;

        public bool RefreshContentFromFile()
        {
            try
            {
                lock (this)
                    if (_SourceFile != null)
                    {
                        _SourceFile = STEM.Sys.IO.Path.AdjustPath(_SourceFile);

                        if (!System.IO.File.Exists(_SourceFile) && System.IO.Directory.Exists(System.IO.Path.GetPathRoot(_SourceFile)))
                        {
                            FileContent = null;

                            if (DeletedTimeUtc < LastWriteTimeUtc)
                            {
                                DeletedTimeUtc = DateTime.Parse(DateTime.UtcNow.ToString("G", System.Globalization.CultureInfo.CurrentCulture), System.Globalization.CultureInfo.CurrentCulture);
                                return true;
                            }

                            return false;
                        }
                        else if (!System.IO.File.Exists(_SourceFile))
                        {
                            return false;
                        }

                        DateTime creationTimeUtc = System.IO.File.GetCreationTimeUtc(_SourceFile);
                        creationTimeUtc = DateTime.Parse(creationTimeUtc.ToString("G", System.Globalization.CultureInfo.CurrentCulture), System.Globalization.CultureInfo.CurrentCulture);

                        DateTime lastWriteTimeUtc = System.IO.File.GetLastWriteTimeUtc(_SourceFile);
                        lastWriteTimeUtc = DateTime.Parse(lastWriteTimeUtc.ToString("G", System.Globalization.CultureInfo.CurrentCulture), System.Globalization.CultureInfo.CurrentCulture);

                        if (FileContent == null || LastWriteTimeUtc != lastWriteTimeUtc || CreationTimeUtc != creationTimeUtc)
                        {
                            if (Compressed)
                            {
                                byte[] b = System.IO.File.ReadAllBytes(_SourceFile);
                                byte[] cmp = STEM.Sys.IO.ByteCompression.Compress(b, b.Length);

                                if (cmp == null)
                                    cmp = new byte[0];

                                FileContent = Convert.ToBase64String(cmp);
                            }
                            else
                            {
                                FileContent = Convert.ToBase64String(System.IO.File.ReadAllBytes(_SourceFile));
                            }

                            CreationTimeUtc = creationTimeUtc;
                            LastWriteTimeUtc = lastWriteTimeUtc;

                            return true;
                        }
                    }
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("FileTransfer.RefreshContentFromFile", ex.ToString(), Sys.EventLog.EventLogEntryType.Error);
            }

            return false;
        }

        public string Save()
        {
            lock (this)
                if (!_Saved)
                {
                    if (String.IsNullOrEmpty(DestinationPath))
                        DestinationPath = ".";

                    _SavedFile = System.IO.Path.Combine(DestinationPath, DestinationFilename);

                    _SavedFile = STEM.Sys.IO.Path.AdjustPath(_SavedFile);

                    DateTime lastWriteTimeUtc = MinValue;
                    DateTime creationTimeUtc = MinValue;

                    if (System.IO.File.Exists(_SavedFile))
                    {
                        lastWriteTimeUtc = System.IO.File.GetLastWriteTimeUtc(_SavedFile);
                        lastWriteTimeUtc = DateTime.Parse(lastWriteTimeUtc.ToString("G", System.Globalization.CultureInfo.CurrentCulture), System.Globalization.CultureInfo.CurrentCulture);

                        creationTimeUtc = System.IO.File.GetCreationTimeUtc(_SavedFile);
                        creationTimeUtc = DateTime.Parse(creationTimeUtc.ToString("G", System.Globalization.CultureInfo.CurrentCulture), System.Globalization.CultureInfo.CurrentCulture);
                    }

                    if (DeletedTimeUtc > MinValue)
                    {
                        if (DeletedTimeUtc > lastWriteTimeUtc)
                        {
                            if (System.IO.File.Exists(_SavedFile))
                                try
                                {
                                    if (onFileDeleting != null)
                                        onFileDeleting(this.MessageConnection, Clone());
                                }
                                catch (Exception ex)
                                {
                                    STEM.Sys.EventLog.WriteEntry("FileTransfer.onFileDeleting", ex.ToString(), Sys.EventLog.EventLogEntryType.Error);
                                }

                            int retry = 0;
                            while (System.IO.File.Exists(_SavedFile) && retry++ < 5)
                            {
                                try
                                {
                                    System.IO.File.Delete(_SavedFile);
                                    _Saved = true;

                                    try
                                    {
                                        if (onFileDeleted != null)
                                            onFileDeleted(this.MessageConnection, Clone());
                                    }
                                    catch (Exception ex)
                                    {
                                        STEM.Sys.EventLog.WriteEntry("FileTransfer.onFileDeleted", ex.ToString(), Sys.EventLog.EventLogEntryType.Error);
                                    }

                                    return null;
                                }
                                catch { System.Threading.Thread.Sleep(1000); }
                            }

                            if (System.IO.File.Exists(_SavedFile))
                                return _SavedFile;
                            else
                                return null;
                        }
                        else
                        {
                            DeletedTimeUtc = MinValue;
                            LastWriteTimeUtc = lastWriteTimeUtc;
                            CreationTimeUtc = creationTimeUtc;
                            return _SavedFile;
                        }
                    }

                    if (OnlySaveIfNewer)
                    {
                        if (LastWriteTimeUtc > lastWriteTimeUtc)
                            _SavedFile = Save(_SavedFile, FileExistsAction);
                    }
                    else
                    {
                        _SavedFile = Save(_SavedFile, FileExistsAction);
                    }

                    FileContent = null;
                    _Saved = true;
                }
            
            return _SavedFile;
        }

        FileTransfer Clone()
        {
            FileTransfer r = new FileTransfer();
            r.CopyFrom(this);
            return r;
        }

        public override void CopyFrom(Message source)
        {
            if (source != null)
            {
                if (object.ReferenceEquals(source, this))
                    return;

                FileTransfer m = source as FileTransfer;

                if (m != null)
                {
                    base.CopyFrom(source);
                    
                    _SourceFile = m._SourceFile;
                    DeletedTimeUtc = m.DeletedTimeUtc;
                    CreationTimeUtc = m.CreationTimeUtc;
                    LastWriteTimeUtc = m.LastWriteTimeUtc;
                    OnlySaveIfNewer = m.OnlySaveIfNewer;
                    Compressed = m.Compressed;
                    DestinationPath = m.DestinationPath;
                    DestinationFilename = m.DestinationFilename;
                    FileExistsAction = m.FileExistsAction;
                    FileContent = m.FileContent;
                }
            }
        }

        string Save(string destination, STEM.Sys.IO.FileExistsAction fileExistsAction)
        {
            if (FileContent == null)
                return null;

            destination = STEM.Sys.IO.Path.AdjustPath(destination);

            try
            {
                if (System.IO.File.Exists(destination))
                {
                    if (fileExistsAction == Sys.IO.FileExistsAction.Skip)
                        return null;

                    if (fileExistsAction == Sys.IO.FileExistsAction.Throw)
                        throw new System.IO.IOException("Destination file already exists: " + destination);
                }
            }
            catch { }

            if (!System.IO.Directory.Exists(STEM.Sys.IO.Path.GetDirectoryName(destination)))
                System.IO.Directory.CreateDirectory(STEM.Sys.IO.Path.GetDirectoryName(destination));

            string tmp = System.IO.Path.GetTempFileName();

            try
            {
                if (Compressed)
                {
                    byte[] b = Convert.FromBase64String(FileContent);
                    System.IO.File.WriteAllBytes(tmp, STEM.Sys.IO.ByteCompression.Decompress(b, b.Length));
                }
                else
                {
                    System.IO.File.WriteAllBytes(tmp, Convert.FromBase64String(FileContent));
                }

                System.IO.File.SetCreationTimeUtc(tmp, CreationTimeUtc);
                System.IO.File.SetLastWriteTimeUtc(tmp, LastWriteTimeUtc);

                try
                {
                    if (System.IO.File.Exists(destination))
                    {
                        if (fileExistsAction == Sys.IO.FileExistsAction.Skip)
                            return null;

                        if (fileExistsAction == Sys.IO.FileExistsAction.Throw)
                            throw new System.IO.IOException("Destination file already exists: " + destination);

                        if (fileExistsAction == Sys.IO.FileExistsAction.Overwrite)
                        {
                            try
                            {
                                if (onFileModifying != null)
                                    onFileModifying(this.MessageConnection, Clone());
                            }
                            catch (Exception ex)
                            {
                                STEM.Sys.EventLog.WriteEntry("FileTransfer.onFileModifying", ex.ToString(), Sys.EventLog.EventLogEntryType.Error);
                            }

                            int retry = 0;
                            while (true)
                            {
                                try
                                {
                                    System.IO.File.SetAttributes(destination, System.IO.FileAttributes.Normal);
                                    System.IO.File.Copy(tmp, destination, true);
                                    break;
                                }
                                catch (Exception ex)
                                {
                                    if (++retry > 3)
                                        throw new System.IO.IOException("Destination file could not be overwritten: " + destination, ex);

                                    System.Threading.Thread.Sleep(1000); 
                                }
                            }
                            
                            try
                            {
                                if (onFileModified != null)
                                    onFileModified(this.MessageConnection, Clone());
                            }
                            catch (Exception ex)
                            {
                                STEM.Sys.EventLog.WriteEntry("FileTransfer.onFileModified", ex.ToString(), Sys.EventLog.EventLogEntryType.Error);
                            }
                        }
                        else if (fileExistsAction == Sys.IO.FileExistsAction.MakeUnique)
                        {
                            string d = destination;
                            while (true)
                            {
                                if (!System.IO.File.Exists(d))
                                    try
                                    {
                                        DestinationFilename = STEM.Sys.IO.Path.GetFileName(d);

                                        try
                                        {
                                            if (onFileAdding != null)
                                                onFileAdding(this.MessageConnection, Clone());
                                        }
                                        catch (Exception ex)
                                        {
                                            STEM.Sys.EventLog.WriteEntry("FileTransfer.onFileAdding", ex.ToString(), Sys.EventLog.EventLogEntryType.Error);
                                        }

                                        System.IO.File.Copy(tmp, d, false);

                                        try
                                        {
                                            if (onFileAdded != null)
                                                onFileAdded(this.MessageConnection, Clone());
                                        }
                                        catch (Exception ex)
                                        {
                                            STEM.Sys.EventLog.WriteEntry("FileTransfer.onFileAdded", ex.ToString(), Sys.EventLog.EventLogEntryType.Error);
                                        }

                                        return d;
                                    }
                                    catch { }

                                d = STEM.Sys.IO.File.UniqueFilename(destination);
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            try
                            {
                                if (onFileAdding != null)
                                    onFileAdding(this.MessageConnection, Clone());
                            }
                            catch (Exception ex)
                            {
                                STEM.Sys.EventLog.WriteEntry("FileTransfer.onFileAdding", ex.ToString(), Sys.EventLog.EventLogEntryType.Error);
                            }

                            System.IO.File.Copy(tmp, destination, false);

                            try
                            {
                                if (onFileAdded != null)
                                    onFileAdded(this.MessageConnection, Clone());
                            }
                            catch (Exception ex)
                            {
                                STEM.Sys.EventLog.WriteEntry("FileTransfer.onFileAdded", ex.ToString(), Sys.EventLog.EventLogEntryType.Error);
                            }

                            return destination;
                        }
                        catch (Exception ex)
                        {
                            if (System.IO.File.Exists(destination))
                                return Save(destination, fileExistsAction);

                            throw ex;
                        }
                    }
                }
                catch { }
            }
            finally
            {
                int retry = 0;
                while (System.IO.File.Exists(tmp) && retry++ < 5)
                    try
                    {
                        System.IO.File.Delete(tmp);
                        break;
                    }
                    catch { System.Threading.Thread.Sleep(1000); }
            }

            return null;
        }
    }
}
