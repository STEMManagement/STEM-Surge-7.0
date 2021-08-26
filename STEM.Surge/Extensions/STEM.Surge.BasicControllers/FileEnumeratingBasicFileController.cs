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
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using STEM.Sys.Threading;

namespace STEM.Surge.BasicControllers
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("FileEnumeratingBasicFileController")]
    [Description("Customize an InstructionSet Template using placeholders related to the file properties for each file discovered. (e.g. [TargetPath], [TargetName], [LastWriteTimeUtc]...) " +
        "Note that with this controller [DestinationPath] can be an expandable path where the controller will choose the best path based on network proximity and destination directory existence (if 'Check for directory existence' is enabled)." +
        "This controller differs from the BasicFileController in that partial file listings are obtained in the background and returned from FileListPreprocess possibly incomplete. This " +
        "Controller can be used when the file source is on a slow network connection or contains millions of files.")]
    public class FileEnumeratingBasicFileController : BasicFileController
    {
        internal static class NativeWin32
        {
            public const int MAX_PATH = 260;

            /// <summary>
            /// Win32 FILETIME structure.  The win32 documentation says this:
            /// "Contains a 64-bit value representing the number of 100-nanosecond intervals since January 1, 1601 (UTC)."
            /// </summary>
            /// <see cref="http://msdn.microsoft.com/en-us/library/ms724284%28VS.85%29.aspx"/>
            [StructLayout(LayoutKind.Sequential)]
            public struct FILETIME
            {
                public uint dwLowDateTime;
                public uint dwHighDateTime;
            }

            /// <summary>
            /// The Win32 find data structure.  The documentation says:
            /// "Contains information about the file that is found by the FindFirstFile, FindFirstFileEx, or FindNextFile function."
            /// </summary>
            /// <see cref="http://msdn.microsoft.com/en-us/library/aa365740%28VS.85%29.aspx"/>
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct WIN32_FIND_DATA
            {
                public FileAttributes dwFileAttributes;
                public FILETIME ftCreationTime;
                public FILETIME ftLastAccessTime;
                public FILETIME ftLastWriteTime;
                public uint nFileSizeHigh;
                public uint nFileSizeLow;
                public uint dwReserved0;
                public uint dwReserved1;

                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
                public string cFileName;

                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
                public string cAlternateFileName;
            }

            /// <summary>
            /// Searches a directory for a file or subdirectory with a name that matches a specific name (or partial name if wildcards are used).
            /// </summary>
            /// <param name="lpFileName">The directory or path, and the file name, which can include wildcard characters, for example, an asterisk (*) or a question mark (?). </param>
            /// <param name="lpFindData">A pointer to the WIN32_FIND_DATA structure that receives information about a found file or directory.</param>
            /// <returns>
            /// If the function succeeds, the return value is a search handle used in a subsequent call to FindNextFile or FindClose, and the lpFindFileData parameter contains information about the first file or directory found.
            /// If the function fails or fails to locate files from the search string in the lpFileName parameter, the return value is INVALID_HANDLE_VALUE and the contents of lpFindFileData are indeterminate.
            ///</returns>
            ///<see cref="http://msdn.microsoft.com/en-us/library/aa364418%28VS.85%29.aspx"/>
            [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern SafeSearchHandle FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindData);

            /// <summary>
            /// Continues a file search from a previous call to the FindFirstFile or FindFirstFileEx function.
            /// </summary>
            /// <param name="hFindFile">The search handle returned by a previous call to the FindFirstFile or FindFirstFileEx function.</param>
            /// <param name="lpFindData">A pointer to the WIN32_FIND_DATA structure that receives information about the found file or subdirectory.
            /// The structure can be used in subsequent calls to FindNextFile to indicate from which file to continue the search.
            /// </param>
            /// <returns>
            /// If the function succeeds, the return value is nonzero and the lpFindFileData parameter contains information about the next file or directory found.
            /// If the function fails, the return value is zero and the contents of lpFindFileData are indeterminate.
            /// </returns>
            /// <see cref="http://msdn.microsoft.com/en-us/library/aa364428%28VS.85%29.aspx"/>
            [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool FindNextFile(SafeSearchHandle hFindFile, out WIN32_FIND_DATA lpFindData);

            /// <summary>
            /// Closes a file search handle opened by the FindFirstFile, FindFirstFileEx, or FindFirstStreamW function.
            /// </summary>
            /// <param name="hFindFile">The file search handle.</param>
            /// <returns>
            /// If the function succeeds, the return value is nonzero.
            /// If the function fails, the return value is zero. 
            /// </returns>
            /// <see cref="http://msdn.microsoft.com/en-us/library/aa364413%28VS.85%29.aspx"/>
            [DllImport("kernel32", SetLastError = true)]
            public static extern bool FindClose(IntPtr hFindFile);

            /// <summary>
            /// Class to encapsulate a seach handle returned from FindFirstFile.  Using a wrapper
            /// like this ensures that the handle is properly cleaned up with FindClose.
            /// </summary>
            public class SafeSearchHandle : SafeHandleZeroOrMinusOneIsInvalid
            {
                public SafeSearchHandle() : base(true) { }

                protected override bool ReleaseHandle()
                {
                    return NativeWin32.FindClose(base.handle);
                }
            }
        }

        static STEM.Sys.Threading.ThreadPool _PollerPool = new Sys.Threading.ThreadPool(TimeSpan.FromMilliseconds(1000), Environment.ProcessorCount, true);

        class Poller : STEM.Sys.Threading.IThreadable
        {
            class FileInfo
            {
                public long FileSize { get; set; }
                public string FileName { get; set; }
            }

            IEnumerable<FileInfo> EnumerateFiles(string directory, string searchPattern)
            {
                NativeWin32.WIN32_FIND_DATA findData;
                using (NativeWin32.SafeSearchHandle hFindFile = NativeWin32.FindFirstFile(Path.Combine(directory, searchPattern), out findData))
                {
                    if (!hFindFile.IsInvalid)
                    {
                        do
                        {
                            if ((findData.dwFileAttributes & FileAttributes.Directory) == 0 && findData.cFileName != "." && findData.cFileName != "..")
                            {
                                long len = (((long)findData.nFileSizeHigh) << 0x20) | findData.nFileSizeLow;
                                yield return new FileInfo { FileName = Path.Combine(directory, findData.cFileName), FileSize = len };
                            }
                        } while (NativeWin32.FindNextFile(hFindFile, out findData));
                    }
                }
            }

            public static List<Poller> Pollers { get; private set; }
            public string SourceDirectory { get; private set; }
            public string FileFilter { get; private set; }            

            Dictionary<string, long> Files { get; set; }
            object _ObjectLock = new object();

            List<FileEnumeratingBasicFileController> _References = new List<FileEnumeratingBasicFileController>();

            static Poller()
            {
                Pollers = new List<Poller>();
            }

            private Poller()
            {
            }

            public void Dispose(FileEnumeratingBasicFileController controller)
            {
                lock (Pollers)
                {
                    if (_References.Contains(controller))
                        _References.Remove(controller);

                    if (_References.Count == 0)
                    {
                        Dispose();

                        if (Pollers.Contains(this))
                            Pollers.Remove(this);
                    }
                }
            }

            public static Poller GetPoller(string sourceDirectory, string fileFilter, FileEnumeratingBasicFileController controller)
            {                
                lock (Pollers)
                {
                    Poller p = Pollers.FirstOrDefault(i => i.SourceDirectory.Equals(sourceDirectory, StringComparison.InvariantCultureIgnoreCase) && i.FileFilter.Equals(fileFilter, StringComparison.InvariantCultureIgnoreCase));

                    if (p == null)
                    {
                        p = new Poller();
                        p.SourceDirectory = sourceDirectory;
                        p.FileFilter = fileFilter;
                        p.Files = new Dictionary<string, long>(StringComparer.InvariantCultureIgnoreCase);

                        p._References.Add(controller);

                        _PollerPool.BeginAsync(p);

                        Pollers.Add(p);
                    }

                    if (!p._References.Contains(controller))
                        p._References.Add(controller);

                    return p;
                }
            }

            protected override void Execute(ThreadPool owner)
            {
                try
                {
                    Regex inc = STEM.Sys.IO.Path.BuildInclusiveFilter(FileFilter);
                    Regex exc = STEM.Sys.IO.Path.BuildExclusiveFilter(FileFilter);

                    FileEnumeratingBasicFileController any = _References.FirstOrDefault();

                    if (any == null)
                        return;

                    foreach (FileInfo f in EnumerateFiles(SourceDirectory, "*.*"))
                    {
                        if (inc == null || STEM.Sys.IO.Path.StringMatches(f.FileName, inc))
                            if (exc == null || !STEM.Sys.IO.Path.StringMatches(f.FileName, exc))
                            {
                                if (any.CoordinatedKeyManager.Locked(f.FileName))
                                    continue;

                                if (any.CoordinatedKeyManager.CoordinatedMachineHasLock(f.FileName, any.CoordinateWith) == Sys.State.CoordinatedKeyManager.RemoteLockExists.False)
                                    lock (_ObjectLock)
                                        Files[f.FileName] = f.FileSize;
                            }
                    }
                }
                catch
                {
                }
            }

            protected override void Dispose(bool dispose)
            {
                lock (Pollers)
                {
                    if (Pollers.Contains(this))
                        Pollers.Remove(this);
                }

                base.Dispose(dispose);
            }

            public List<string> GetFiles(long upperFilesizeLimit, long lowerFileSizeLimit)
            {
                //List<FileInfo> ret = null;

                lock (_ObjectLock)
                    return Files.Where(i => i.Value >= lowerFileSizeLimit && i.Value <= upperFilesizeLimit).Select(i => i.Key).ToList();
                //ret = Files.Where(i => i.FileSize >= lowerFileSizeLimit && i.FileSize <= upperFilesizeLimit).ToList();

                //if (ret.Count == 0)
                //    return new List<string>();

                //lock (_ObjectLock)
                //    Files = Files.Except(ret).ToList();

                //return ret.Select(i => i.FileName).ToList();
            }

            public void Remove(string file)
            {
                lock (_ObjectLock)
                    if (Files.ContainsKey(file))
                        Files.Remove(file);
            }
        }

        public FileEnumeratingBasicFileController() : base()
        {
            PreprocessPerformsDiscovery = true;
        }

        public enum PreProcessSortMethod { None, Random, Alphabetical }

        public PreProcessSortMethod SortMethod { get; set; }

        protected override void Dispose(bool dispose)
        {
            base.Dispose(dispose);

            try
            {
                if (_Recurser != null)
                    _Recurser.Dispose();
            }
            catch { }

            foreach (Poller p in _Pollers.ToList())
                try
                {
                    p.Dispose(this);
                }
                catch { }
        }

        List<Poller> _Pollers = new List<Poller>();

        public override bool FileExists(string file)
        {
            bool e = base.FileExists(file);

            if (!e)
                foreach (Poller p in _Pollers)
                    p.Remove(file);

            return e;
        }


        class Recurser : STEM.Sys.Threading.IThreadable
        {
            FileEnumeratingBasicFileController _Controller;
            string _SourceDirectory;
            string _DirectoryFilter;
            string _FileFilter;

            public Recurser(string sourceDirectory, string directoryFilter, string fileFilter, FileEnumeratingBasicFileController controller)
            {
                _SourceDirectory = sourceDirectory;
                _DirectoryFilter = directoryFilter;
                _FileFilter = fileFilter;
                _Controller = controller;

                ExecutionInterval = TimeSpan.FromSeconds(10);
                _PollerPool.BeginAsync(this);
            }

            protected override void Execute(ThreadPool owner)
            {
                try
                {
                    foreach (string d in STEM.Sys.IO.Directory.STEM_GetDirectories(_SourceDirectory, _DirectoryFilter, SearchOption.AllDirectories, false))
                    {
                        lock (_Controller._Pollers)
                        {
                            if (!_Controller._Pollers.Exists(i => i.SourceDirectory.Equals(d, StringComparison.InvariantCultureIgnoreCase)))
                                _Controller._Pollers.Add(Poller.GetPoller(d, _FileFilter, _Controller));
                        }
                    }
                }
                catch { }
            }
        }

        Recurser _Recurser = null;

        protected List<string> ListPreprocess(string sourceDirectory, string directoryFilter, string fileFilter, long upperFilesizeLimit, long lowerFileSizeLimit)
        {
            if (PollerRecurseSetting)
            {
                lock (_Pollers)
                    if (_Recurser == null)
                    {
                        _Recurser = new Recurser(sourceDirectory, directoryFilter, fileFilter, this);
                        _Pollers.Add(Poller.GetPoller(sourceDirectory, fileFilter, this));
                    }
            }
            else
            {
                if (_Pollers.Count == 0)
                {
                    _Pollers.Add(Poller.GetPoller(sourceDirectory, fileFilter, this));
                }
            }

            List<string> files = new List<string>();

            lock (_Pollers)
                foreach (Poller p in _Pollers)
                {
                    files.AddRange(p.GetFiles(upperFilesizeLimit, lowerFileSizeLimit));
                }

            if (SortMethod == PreProcessSortMethod.None)
                return files;

            if (SortMethod == PreProcessSortMethod.Alphabetical)
                return files.OrderBy(i => i).ToList();

            Random rand = new Random();
            if (SortMethod == PreProcessSortMethod.Random)
                return files.OrderBy(i => rand.Next()).ToList();

            if (HonorPriorityFilters)
                files = ApplyPriorityFilterOrdering(files);

            return files;
        }

        public override List<string> ListPreprocess(IReadOnlyList<string> list)
        {
            return ListPreprocess(PollerSourceString, PollerDirectoryFilter, PollerFileFilter, Int64.MaxValue, 0);
        }

        public override DeploymentDetails GenerateDeploymentDetails(IReadOnlyList<string> listPreprocessResult, string initiationSource, string recommendedBranchIP, IReadOnlyList<string> limitedToBranches)
        {            
            foreach (Poller p in _Pollers.ToList())
                p.Remove(initiationSource);

            return base.GenerateDeploymentDetails(listPreprocessResult, initiationSource, recommendedBranchIP, limitedToBranches);
        }
    }
}
