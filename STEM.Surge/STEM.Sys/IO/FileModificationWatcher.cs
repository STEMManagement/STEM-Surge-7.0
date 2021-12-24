using STEM.Sys.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace STEM.Sys.IO
{
    public class FileModificationWatcher : STEM.Sys.Threading.IThreadable
    {
        static STEM.Sys.Threading.ThreadPool _WatcherPool = new Threading.ThreadPool(8, true);

        Dictionary<string, FMWEventArgs> _Watching = new Dictionary<string, FMWEventArgs>(StringComparer.InvariantCultureIgnoreCase);

        public string Directory { get; private set; }
        public string FileFilter { get; private set; }
        public string DirectoryFilter { get; private set; }
        public bool Recurse { get; private set; }

        public FileModificationWatcher(string directory, string fileFilter, string directoryFilter, bool recurse)
        {
            Directory = directory;
            FileFilter = fileFilter;
            DirectoryFilter = directoryFilter;
            Recurse = recurse;

            _WatcherPool.BeginAsync(this);
        }

        public event EventHandler FileModified;

        protected override void Execute(ThreadPool owner)
        {
            try
            {
                List<string> found = new List<string>();
                List<FMWEventArgs> changed = new List<FMWEventArgs>();

                foreach (string file in STEM.Sys.IO.Directory.STEM_GetFiles(Directory, FileFilter, DirectoryFilter, Recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly, true))
                {
                    try
                    {
                        found.Add(file);

                        DateTime lwt = System.IO.File.GetLastWriteTimeUtc(file);

                        lock (_Watching)
                        {
                            if (_Watching.ContainsKey(file))
                            {
                                FMWEventArgs a = _Watching[file];

                                if (a.CurrentModificationTimeUtc != lwt)
                                {
                                    a.PreviousModificationTimeUtc = a.CurrentModificationTimeUtc;
                                    a.CurrentModificationTimeUtc = lwt;
                                    changed.Add(a);
                                }
                            }
                            else
                            {
                                FMWEventArgs a = new FMWEventArgs { File = file, PreviousModificationTimeUtc = DateTime.MinValue, CurrentModificationTimeUtc = lwt };
                                _Watching.Add(file, a);
                                changed.Add(a);
                            }
                        }
                    }
                    catch { }
                }

                lock (_Watching)
                    foreach (string file in _Watching.Keys.Except(found))
                    {
                        if (!System.IO.File.Exists(file))
                        {
                            FMWEventArgs a = _Watching[file];
                            a.PreviousModificationTimeUtc = a.CurrentModificationTimeUtc;
                            a.CurrentModificationTimeUtc = DateTime.MinValue;
                            changed.Add(a);
                        }
                    }

                foreach (FMWEventArgs a in changed)
                    CallChanged(a);
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("STEM.Sys.IO.FileModificationWatcher.Execute", ex, EventLog.EventLogEntryType.Error);
            }
        }

        public class FMWEventArgs : EventArgs
        {
            public string File { get; internal set; }
            public DateTime PreviousModificationTimeUtc { get; internal set; }
            public DateTime CurrentModificationTimeUtc { get; internal set; }

            public string ReadAllText()
            {
                try
                {
                    return System.IO.File.ReadAllText(File);
                }
                catch { }

                return "";
            }
            public byte[] ReadAllBytes()
            {
                try
                {
                    return System.IO.File.ReadAllBytes(File);
                }
                catch { }

                return new byte[0];
            }
        }

        public FMWEventArgs this[string file]
        {
            get 
            {
                lock (_Watching)
                {
                    if (_Watching.ContainsKey(file))
                    {
                        return _Watching[file];
                    }
                }

                return null;
            }
        }

        void CallChanged(FMWEventArgs args)
        {
            List<EventHandler> calls = null;

            if (FileModified != null)
                try
                {
                    calls = FileModified.GetInvocationList().Cast<EventHandler>().ToList();
                }
                catch { }

            foreach (EventHandler c in calls)
            {
                try
                {
                    c(this, args);
                }
                catch (Exception ex)
                {
                    STEM.Sys.EventLog.WriteEntry("STEM.Sys.IO.CallChanged", ex, EventLog.EventLogEntryType.Error);
                }
            }
        }
    }
}
