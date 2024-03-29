﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using STEM.Sys.Threading;

namespace STEM.Sys.Serialization
{
    public static class VersionManager
    {
        class _Cache
        {
            public string Path { get; set; }
            public bool RenameSourceAssemblies { get; set; }
            public bool Recurse { get; set; }
        }

        static List<_Cache> _Caches = new List<_Cache>();

        static System.Collections.Generic.Dictionary<string, DateTime> _Evaluated = new System.Collections.Generic.Dictionary<string, DateTime>(StringComparer.InvariantCultureIgnoreCase);

        static System.Collections.Generic.Dictionary<AssemblyName, Assembly> _AssemblyByName = new System.Collections.Generic.Dictionary<AssemblyName, Assembly>();

        static System.Collections.Generic.Dictionary<string, Assembly> _CachedAssemblies = new System.Collections.Generic.Dictionary<string, Assembly>(StringComparer.InvariantCultureIgnoreCase);
        
        static STEM.Sys.State.KeyManager _Keymanager = new State.KeyManager();

        static Thread _CachePoller = null;

        static void _Initialize()
        {
            lock (_CachedAssemblies)
            {
                if (_CachePoller != null)
                    return;

                foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        if (!_Evaluated.ContainsKey(asm.Location))
                        {
                            _Evaluated[asm.Location] = File.GetLastWriteTimeUtc(asm.Location);
                        }
                        else
                        {
                            continue;
                        }

                        AssemblyName aName = new AssemblyName(asm.FullName);

                        lock (_AssemblyByName)
                            _AssemblyByName[aName] = asm;

                        string xform = System.IO.Path.Combine(STEM.Sys.IO.Path.GetDirectoryName(asm.Location), TransformFilename(asm.Location));

                        if (!_CachedAssemblies.ContainsKey(xform))
                           _CachedAssemblies[xform] = asm;

                        if (!_CachedAssemblies.ContainsKey(asm.Location))
                            _CachedAssemblies[asm.Location] = asm;
                    }
                    catch { }
                }

                List<FileInfo> infos = new List<FileInfo>();
                foreach (string file in STEM.Sys.IO.Directory.STEM_GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll|*.so|*.a|*.lib", false))
                    infos.Add(new FileInfo(file));

                foreach (FileInfo file in infos.OrderByDescending(i => i.LastWriteTimeUtc))
                {
                    if (!file.FullName.EndsWith("STEM.Auth.dll", StringComparison.InvariantCultureIgnoreCase))
                        if (!_Evaluated.ContainsKey(file.FullName.ToUpper(System.Globalization.CultureInfo.CurrentCulture)))
                        {
                            try
                            {
                                Assembly asm = Assembly.LoadFile(file.FullName);
                                _Evaluated[asm.Location] = File.GetLastWriteTimeUtc(asm.Location);
                            }
                            catch { }
                        }
                }

                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolve);
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += new ResolveEventHandler(AssemblyResolve);
                AppDomain.CurrentDomain.TypeResolve += new ResolveEventHandler(TypeResolve);

                if (_CachePoller == null)
                {
                    _CachePoller = new Thread(new ThreadStart(CachePoller));
                    _CachePoller.IsBackground = true;
                    _CachePoller.Start();
                }
            }
        }

        static public void Initialize()
        {
            Initialize(new List<string>(new string[] { VersionCache }), false, false);
        }

        static public void Initialize(List<string> caches)
        {
            Initialize(caches, false, false);
        }

        static public void Initialize(List<string> caches, bool flushLocal)
        {
            Initialize(caches, flushLocal, false);
        }

        static public void Initialize(List<string> caches, bool flushLocal, bool renameSourceAssemblies)
        {
            if (flushLocal)
            {
                foreach (string d in Directory.GetDirectories(VersionCache))
                {
                    foreach (string f in Directory.GetFiles(d))
                        try
                        {
                            File.Delete(f);
                        }
                        catch { }
                }

                foreach (string f in Directory.GetFiles(VersionCache))
                    try
                    {
                        File.Delete(f);
                    }
                    catch { }

                foreach (string d in Directory.GetDirectories(VersionCache))
                    try
                    {
                        Directory.Delete(d, true);
                    }
                    catch { }
            }

            AddCache(VersionCache, false, true);
            AddCache(caches, renameSourceAssemblies);
        }

        static public List<string> Caches
        {
            get
            {
                lock (_Caches)
                    return _Caches.Select(i => i.Path).ToList();
            }
        }

        static public void AddCache(string path, bool renameSourceAssemblies, bool recurse, bool loadImmediately)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            path = STEM.Sys.IO.Path.AdjustPath(System.IO.Path.GetFullPath(path));

            try
            {
                _Initialize();

                lock (_Caches)
                    try
                    {
                        _AwaitingLoad = loadImmediately;

                        foreach (string dir in STEM.Sys.IO.Path.ExpandRangedPath(path))
                        {
                            _Cache c = _Caches.FirstOrDefault(i => i.Path.Equals(dir, StringComparison.InvariantCultureIgnoreCase));

                            if (c == null)
                            {
                                c = new _Cache { Path = dir, Recurse = recurse, RenameSourceAssemblies = renameSourceAssemblies };

                                if (dir.TrimEnd(Path.DirectorySeparatorChar).Equals(AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar), StringComparison.InvariantCultureIgnoreCase))
                                {
                                    c.Recurse = false;
                                    c.RenameSourceAssemblies = false;
                                }
                                else
                                {
                                    c.Recurse = recurse;
                                    c.RenameSourceAssemblies = renameSourceAssemblies;
                                }

                                _Caches.Add(c);

                                if (loadImmediately)
                                {
                                    Load(c);

                                    while (_CacherPool.LoadLevel > 0)
                                        System.Threading.Thread.Sleep(10);
                                }
                            }
                            else
                            {
                                if (dir.TrimEnd(Path.DirectorySeparatorChar).Equals(AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar), StringComparison.InvariantCultureIgnoreCase))
                                {
                                    c.Recurse = false;
                                    c.RenameSourceAssemblies = false;
                                }
                                else
                                {
                                    c.Recurse = recurse;
                                    c.RenameSourceAssemblies = renameSourceAssemblies;
                                }

                                if (loadImmediately)
                                {
                                    Load(c);

                                    while (_CacherPool.LoadLevel > 0)
                                        System.Threading.Thread.Sleep(10);
                                }
                            }
                        }
                    }
                    finally
                    {
                        _AwaitingLoad = false;
                    }
            }
            catch { }
        }

        static public void AddCache(string path, bool renameSourceAssemblies, bool recurse)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            path = STEM.Sys.IO.Path.AdjustPath(path);

            AddCache(path, renameSourceAssemblies, recurse, true);
        }

        static public void AddCache(string path)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            path = STEM.Sys.IO.Path.AdjustPath(path);

            AddCache(path, false, false);
        }

        static public void AddCache(string path, bool renameSourceAssemblies)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            path = STEM.Sys.IO.Path.AdjustPath(path);

            AddCache(path, renameSourceAssemblies, false);
        }

        static public void AddCache(List<string> caches, bool renameSourceAssemblies, bool recurse)
        {
            if (caches == null)
                throw new ArgumentNullException(nameof(caches));

            try
            {
                foreach (string s in caches)
                    AddCache(s, renameSourceAssemblies, recurse);
            }
            catch { }
        }

        static public void AddCache(List<string> caches, bool renameSourceAssemblies)
        {
            if (caches == null)
                throw new ArgumentNullException(nameof(caches));

            try
            {
                foreach (string s in caches)
                    AddCache(s, renameSourceAssemblies, false);
            }
            catch { }
        }

        static public void AddCache(List<string> caches)
        {
            if (caches == null)
                throw new ArgumentNullException(nameof(caches));

            AddCache(caches, false);
        }

        static public void RemoveCache(List<string> caches)
        {
            if (caches == null)
                throw new ArgumentNullException(nameof(caches));

            try
            {
                foreach (string s in caches)
                    RemoveCache(s);
            }
            catch { }
        }

        static public void RemoveCache(string path)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            path = STEM.Sys.IO.Path.AdjustPath(path);

            try
            {
                lock (_Caches)
                    foreach (string dir in STEM.Sys.IO.Path.ExpandRangedPath(path))
                    {
                        _Cache c = _Caches.FirstOrDefault(i => i.Path.Equals(dir, StringComparison.InvariantCultureIgnoreCase));
                        if (c != null)
                            _Caches.Remove(c);
                    }
            }
            catch { }
        }

        static bool _AwaitingLoad = false;
        static void CachePoller()
        {
            try
            {
                _Initialize();
            }
            catch { }

            while (true)
            {
                while (_AwaitingLoad)
                    System.Threading.Thread.Sleep(10);

                while (_CacherPool.LoadLevel > 0)
                    System.Threading.Thread.Sleep(10);

                try
                {
                    List<_Cache> caches = null;
                    lock (_Caches)
                        caches = _Caches.ToList();

                    foreach (_Cache c in caches)
                        try
                        {
                            if (_AwaitingLoad)
                                break;

                            Load(c);
                        }
                        catch { }
                }
                catch { }

                Thread.Sleep(5000);
            }
        }

        public delegate void AssemblyLoaded(string asmFile, Assembly assembly);
        public static AssemblyLoaded onAssemblyLoaded;

        static void Load(_Cache cache)
        {
            try
            {
                lock (_Caches)
                    if (!_Caches.Contains(cache))
                        return;

                List<string> files = new List<string>();

                if (Directory.Exists(cache.Path))
                {
                    List<string> dirs = new List<string>();
                    if (cache.Recurse)
                        dirs = STEM.Sys.IO.Directory.STEM_GetDirectories(cache.Path, "!.Archive|!TEMP", SearchOption.AllDirectories, false);

                    if (!dirs.Contains(cache.Path))
                        dirs.Add(cache.Path);

                    foreach (string dir in dirs)
                    {
                        List<FileInfo> infos = new List<FileInfo>();
                        foreach (string file in STEM.Sys.IO.Directory.STEM_GetFiles(dir, "*.dll|*.so|*.a|*.lib", false))
                            try
                            {
                                infos.Add(new FileInfo(file));
                            }
                            catch{ }

                        foreach (FileSystemInfo file in infos.OrderByDescending(i => i.LastWriteTimeUtc))
                            if (!file.FullName.EndsWith("STEM.Auth.dll", StringComparison.InvariantCultureIgnoreCase))
                                lock (_CachedAssemblies)
                                    if (!_Evaluated.ContainsKey(file.FullName) || _Evaluated[file.FullName] != file.LastWriteTimeUtc)
                                        if (!_Attempts.ContainsKey(file.FullName) || _Attempts[file.FullName] < 3)
                                        {
                                            bool myLock = false;
                                            try
                                            {
                                                if (_Keymanager.Lock(file.FullName))
                                                {
                                                    myLock = true;
                                                    _CacherPool.RunOnce(new Cacher(cache, file.FullName, cache.RenameSourceAssemblies));
                                                }
                                            }
                                            catch
                                            {
                                                if (myLock)
                                                    _Keymanager.Unlock(file.FullName);
                                            }
                                        }
                    }
                }
            }
            catch { }
        }

        static string _VersionCache = null;
        static public string VersionCache
        {
            get
            {
                string path = _VersionCache;

                if (path == null)
                    path = Path.Combine(System.Environment.CurrentDirectory, "VersionCache");

                try
                {
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                }
                catch { }

                return path;
            }
        }

        public static void SetVersionCache(string versionCache)
        {
            _VersionCache = STEM.Sys.IO.Path.AdjustPath(versionCache);
        }

        static Assembly TypeResolve(object sender, ResolveEventArgs args)
        {
            Type found = null;
            List<Assembly> asms = LoadedAssemblies();

            foreach (Assembly a in asms)
                try
                {
                    Type t = a.GetType(args.Name, false, true);

                    if (t != null)
                        if (found == null)
                        {
                            found = t;
                        }
                        else
                        {
                            if (t.Assembly.GetName().Version > found.Assembly.GetName().Version)
                                found = t;
                        }
                }
                catch { }

            if (found != null)
                return found.Assembly;

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                try
                {
                    Type t = a.GetType(args.Name, false, true);

                    if (t != null)
                        if (found == null)
                        {
                            found = t;
                        }
                        else
                        {
                            if (t.Assembly.GetName().Version > found.Assembly.GetName().Version)
                                found = t;
                        }
                }
                catch { }

            if (found != null)
                return found.Assembly;

            return null;
        }

        static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            lock (_AssemblyByName)
            {
                Assembly ret = _AssemblyByName.Where(i => i.Key.FullName == args.Name).Select(i => i.Value).FirstOrDefault();

                if (ret != null)
                    return ret;

                ret = _AssemblyByName.Where(i => i.Key.FullName.Replace(i.Key.Name, i.Key.Name + "." + i.Key.Version.ToString()) == args.Name).Select(i => i.Value).FirstOrDefault();

                if (ret != null)
                    return ret;

                AssemblyName argsName = new AssemblyName(args.Name);

                ret = _AssemblyByName.Where(i => i.Key.Name == argsName.Name && i.Key.Version >= argsName.Version).OrderBy(i => i.Key.Version).Select(i => i.Value).FirstOrDefault();

                if (ret != null)
                    return ret;
            }


            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    if (a.FullName.Equals(args.Name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return a;
                    }
                }
                catch { }

                try
                {
                    string s = a.FullName;
                    AssemblyName n = new AssemblyName(s);
                    s = s.Replace(n.Name, n.Name + "." + n.Version.ToString());
                    if (s.Equals(args.Name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return a;
                    }
                }
                catch { }
            }

            return null;
        }

        public static List<Assembly> BaseAssemblies()
        {
            List<Assembly> ret = new List<Assembly>();

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                try
                {
                    if ((STEM.Sys.IO.Path.GetDirectoryName(a.Location).TrimEnd(Path.DirectorySeparatorChar)).Equals(AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar), StringComparison.CurrentCultureIgnoreCase))
                        ret.Add(a);
                }
                catch { }

            return ret;
        }

        public static List<Type> LoadedTypes()
        {
            List<Type> ret = new List<Type>();

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                try
                {
                    foreach (Type t in a.GetTypes())
                        ret.Add(t);
                }
                catch { }

            return ret;
        }

        public static List<Type> ObjectsOfType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            List<Type> ret = new List<Type>();

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                try
                {
                    foreach (Type t in a.GetTypes())
                        if (type.IsAssignableFrom(t))
                            ret.Add(t);
                }
                catch { }

            return ret;
        }

        static bool IsAssembly(string file)
        {
            if (String.IsNullOrEmpty(file))
                throw new ArgumentNullException(nameof(file));

            file = STEM.Sys.IO.Path.AdjustPath(file);

            try
            {
                System.Reflection.AssemblyName asm = System.Reflection.AssemblyName.GetAssemblyName(file);
            }
            catch (System.BadImageFormatException)
            {
                return false;
            }
            catch
            {
            }

            return true;
        }

        public static string TransformFilename(string file)
        {
            if (String.IsNullOrEmpty(file))
                throw new ArgumentNullException(nameof(file));

            file = STEM.Sys.IO.Path.AdjustPath(file);

            int retry = 3;
            while (retry-- > 0)
                try
                {
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(file);

                    if (!string.IsNullOrEmpty(fvi.OriginalFilename) && fvi.OriginalFilename.EndsWith(".DLL", StringComparison.InvariantCultureIgnoreCase))
                        return STEM.Sys.IO.Path.GetFileNameWithoutExtension(fvi.OriginalFilename) + "." + fvi.FileMajorPart + "." + fvi.FileMinorPart + "." + fvi.FileBuildPart + "." + fvi.FilePrivatePart + ".dll";
                    else
                        return STEM.Sys.IO.Path.GetFileName(file);
                }
                catch { System.Threading.Thread.Sleep(100); }

            return null;
        }

        static public List<Assembly> LoadedAssemblies()
        {
            lock (_CachedAssemblies)
                try
                {
                    return _CachedAssemblies.Values.ToList();
                }
                catch { }

            return new List<Assembly>();
        }

        static public Assembly LoadedAssembly(string fullAssemblyName)
        {
            lock (_CachedAssemblies)
                try
                {
                    return _CachedAssemblies.Values.FirstOrDefault(i => i.FullName.Equals(fullAssemblyName, StringComparison.InvariantCultureIgnoreCase));
                }
                catch { }

            return null;
        }

        static public string AssemblyLocation(Assembly assembly)
        {
            lock (_CachedAssemblies)
                try
                {
                    return _CachedAssemblies.Where(i => i.Value == assembly).Select(i => i.Key).FirstOrDefault();
                }
                catch { }

            return null;
        }

        static public Dictionary<string, string> TransformedFileListing(string cacheDirectory)
        {
            Dictionary<string, string> found = new Dictionary<string, string>();
            foreach (string dir in STEM.Sys.IO.Path.ExpandRangedPath(cacheDirectory))
            {
                try
                {
                    foreach (string dll in STEM.Sys.IO.Directory.STEM_GetFiles(dir, "*.dll|*.so|*.a|*.lib", false))
                        try
                        {
                            found[dll] = TransformFilename(dll);
                        }
                        catch { }
                }
                catch { }
            }

            return found;
        }

        static public string Cache(string file, bool renameSourceAssemblies)
        {
            if (String.IsNullOrEmpty(file))
                throw new ArgumentNullException(nameof(file));

            file = STEM.Sys.IO.Path.AdjustPath(file);

            if (_Keymanager.Lock(file))
            {
                try
                {
                    _Cache cache = null;

                    lock (_Caches)
                        cache = _Caches.Where(i => STEM.Sys.IO.Path.GetDirectoryName(file).StartsWith(i.Path, StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(i => i.Path.Length).FirstOrDefault();

                    if (cache != null)
                        return Cache(cache, file, cache.RenameSourceAssemblies);
                    else
                        return Cache(null, file, renameSourceAssemblies);
                }
                finally
                {
                    _Keymanager.Unlock(file);
                }
            }

            return null;
        }

        static STEM.Sys.Threading.ThreadPool _CacherPool = new Threading.ThreadPool(Int32.MaxValue);

        class Cacher : STEM.Sys.Threading.IThreadable
        {
            _Cache _Cache;
            string _File;
            bool _RenameSourceAssemblies;

            public Cacher(_Cache cache, string file, bool renameSourceAssemblies)
            {
                _Cache = cache;
                _File = file;
                _RenameSourceAssemblies = renameSourceAssemblies;
            }

            protected override void Execute(Threading.ThreadPool owner)
            {
                try
                {
                    lock (_CachedAssemblies)
                        if (_LastAttempt.ContainsKey(_File))
                            if ((DateTime.UtcNow - _LastAttempt[_File]).TotalSeconds < 10)
                                return;

                    VersionManager.Cache(_Cache, _File, _RenameSourceAssemblies);
                }
                catch (Exception ex)
                {
                    string s = ex.ToString();
                }
            }

            protected override void Dispose(bool dispose)
            {
                _Keymanager.Unlock(_File);

                base.Dispose(dispose);
            }
        }

        static Dictionary<string, int> _Attempts = new Dictionary<string, int>();
        static Dictionary<string, DateTime> _LastAttempt = new Dictionary<string, DateTime>();

        static string Cache(_Cache cache, string file, bool renameSourceAssemblies)
        {
            if (String.IsNullOrEmpty(file))
                throw new ArgumentNullException(nameof(file));

            file = STEM.Sys.IO.Path.AdjustPath(file);

            if (file.EndsWith("STEM.Auth.dll", StringComparison.InvariantCultureIgnoreCase))
                return file;

            bool isAsm = IsAssembly(file);

            string xform = Path.GetFileName(file);
            
            if (isAsm)
                xform = TransformFilename(file);

            if (String.IsNullOrEmpty(xform))
                STEM.Sys.EventLog.WriteEntry("VersionManager.Cache", new Exception(file, new ArgumentNullException(nameof(xform))), EventLog.EventLogEntryType.Information);

            string vcFile = xform;

            if (vcFile != null)
            {
                if (xform.StartsWith("STEM.Surge.7.0.0.0", StringComparison.InvariantCultureIgnoreCase))
                    return file;
                if (xform.StartsWith("STEM.Surge.Internal.7.0.0.0", StringComparison.InvariantCultureIgnoreCase))
                    return file;
                if (xform.StartsWith("STEM.Sys.7.0.0.0", StringComparison.InvariantCultureIgnoreCase))
                    return file;
                if (xform.StartsWith("STEM.Sys.Internal.7.0.0.0", StringComparison.InvariantCultureIgnoreCase))
                    return file;

                string fileXformed = Path.Combine(STEM.Sys.IO.Path.GetDirectoryName(file), xform);

                if ((STEM.Sys.IO.Path.GetDirectoryName(file).TrimEnd(Path.DirectorySeparatorChar)).Equals(AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar), StringComparison.CurrentCultureIgnoreCase))
                {
                    try
                    {
                        using (new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None)) { }
                    }
                    catch
                    {
                        try
                        {
                            lock (_CachedAssemblies)
                                _Evaluated[file] = File.GetLastWriteTimeUtc(file);
                        }
                        catch { }

                        return vcFile;
                    }
                }

                if (file.IndexOf("win-x86", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    file.IndexOf("win-x64", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    file.IndexOf("linux-x86", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    file.IndexOf("linux-x64", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    if (!xform.StartsWith("STEM."))
                        renameSourceAssemblies = false;
                }

                vcFile = Path.Combine(VersionCache, xform);
                if (!renameSourceAssemblies)
                    vcFile = Path.Combine(VersionCache, STEM.Sys.IO.Path.GetFileName(file));

                if (cache != null)
                {
                    if (cache.Recurse)
                    {
                        string subDir = STEM.Sys.IO.Path.GetDirectoryName(file.Replace(STEM.Sys.IO.Path.FirstTokenOfPath(file), STEM.Sys.IO.Path.FirstTokenOfPath(cache.Path))).Substring(cache.Path.Length).Trim(Path.DirectorySeparatorChar);

                        vcFile = Path.Combine(Path.Combine(VersionCache, subDir), STEM.Sys.IO.Path.GetFileName(vcFile));

                        if (!Directory.Exists(STEM.Sys.IO.Path.GetDirectoryName(vcFile)))
                            Directory.CreateDirectory(STEM.Sys.IO.Path.GetDirectoryName(vcFile));
                    }
                }

                if (!vcFile.Equals(file, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (File.Exists(vcFile))
                    {
                        try
                        {
                            using (new FileStream(vcFile, FileMode.Open, FileAccess.Read, FileShare.None)) { }
                            File.Copy(file, vcFile, true);
                        }
                        catch { }
                    }
                    else
                    {
                        File.Copy(file, vcFile, true);
                    }

                    if (STEM.Sys.IO.Path.GetDirectoryName(file).StartsWith(VersionCache, StringComparison.CurrentCultureIgnoreCase))
                        try
                        {
                            File.Delete(file);
                        }
                        catch { }

                    if ((STEM.Sys.IO.Path.GetDirectoryName(file).TrimEnd(Path.DirectorySeparatorChar)).Equals(AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar), StringComparison.CurrentCultureIgnoreCase))
                        try
                        {
                            File.Delete(file);
                        }
                        catch { }
                }

                if (renameSourceAssemblies)
                {
                    try
                    {
                        if (File.Exists(file))
                        {
                            if (!file.Equals(fileXformed, StringComparison.CurrentCultureIgnoreCase))
                            {
                                if (File.Exists(file) && File.Exists(fileXformed))
                                {
                                    if (File.GetLastWriteTimeUtc(file) >= File.GetLastWriteTimeUtc(fileXformed))
                                    {
                                        Guid myExt = Guid.NewGuid();

                                        try
                                        {
                                            while (File.Exists(file))
                                                try
                                                {
                                                    if (!File.Exists(file + "." + myExt.ToString()))
                                                        File.Move(file, file + "." + myExt.ToString());
                                                    break;
                                                }
                                                catch { }
                                        }
                                        catch { }

                                        while (File.Exists(file + "." + myExt.ToString()))
                                        {
                                            try
                                            {
                                                try
                                                {
                                                    if (File.Exists(fileXformed))
                                                        File.Delete(fileXformed);
                                                }
                                                catch
                                                {
                                                    if (File.Exists(fileXformed))
                                                        File.Delete(file + "." + myExt.ToString());
                                                }

                                                if (File.Exists(file + "." + myExt.ToString()))
                                                    File.Move(file + "." + myExt.ToString(), fileXformed);
                                            }
                                            catch { }
                                        }
                                    }
                                    else
                                    {
                                        while (File.Exists(file) && File.Exists(fileXformed))
                                            try
                                            {
                                                File.Delete(file);
                                            }
                                            catch { }
                                    }
                                }
                                else
                                {
                                    while (File.Exists(file) && !File.Exists(fileXformed))
                                        try
                                        {
                                            File.Move(file, fileXformed);
                                        }
                                        catch { }
                                }
                            }
                        }
                    }
                    catch { }
                }

                try
                {
                    if (File.Exists(vcFile))
                    {
                        if (!isAsm)
                            lock (_CachedAssemblies)
                            {
                                try
                                {
                                    _Evaluated[vcFile] = File.GetLastWriteTimeUtc(vcFile);
                                }
                                catch { }

                                try
                                {
                                    _Evaluated[file] = File.GetLastWriteTimeUtc(file);
                                }
                                catch { }

                                return STEM.Sys.IO.Path.GetFileName(vcFile);
                            }

                        lock (_CachedAssemblies)
                        {
                            if (!_Attempts.ContainsKey(file))
                                _Attempts[file] = 0;
                            else
                                _Attempts[file] = _Attempts[file] + 1;

                            _LastAttempt[file] = DateTime.UtcNow;
                        }

                        try
                        {
                            //if (xform != null && xform.StartsWith("System.", StringComparison.OrdinalIgnoreCase))
                            //    throw new Exception(file + " will not be loaded.");

                            Assembly asm = VersionManagerALC.LoadFromFile(vcFile, xform, null);

                            if (asm == null)
                                throw new Exception(file + " could not be loaded.");

                            AssemblyName aName = new AssemblyName(asm.FullName);

                            lock (_CachedAssemblies)
                            {
                                _CachedAssemblies[vcFile] = asm;

                                lock (_AssemblyByName)
                                    _AssemblyByName[aName] = asm;

                                try
                                {
                                    _Evaluated[vcFile] = File.GetLastWriteTimeUtc(vcFile);
                                }
                                catch { }

                                if (renameSourceAssemblies)
                                {
                                    try
                                    {
                                        _Evaluated[fileXformed] = File.GetLastWriteTimeUtc(fileXformed);
                                    }
                                    catch { }
                                }
                                else
                                {
                                    try
                                    {
                                        _Evaluated[file] = File.GetLastWriteTimeUtc(file);
                                    }
                                    catch { }
                                }

                                _Attempts.Remove(file);
                                _LastAttempt.Remove(file);
                            }

                            if (onAssemblyLoaded != null)
                                foreach (AssemblyLoaded d in onAssemblyLoaded.GetInvocationList())
                                    try
                                    {
                                        d(vcFile, asm);
                                    }
                                    catch (Exception ex)
                                    {
                                        STEM.Sys.EventLog.WriteEntry("VersionManager:onAssemblyLoaded", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                                    }
                        }
                        catch (Exception ex)
                        {
                            STEM.Sys.EventLog.WriteEntry("VersionManager.Cache", new Exception("Could not load " + vcFile, ex).ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                        }
                    }
                }
                catch { }

                return STEM.Sys.IO.Path.GetFileName(vcFile);
            }
            else
            {
                try
                {
                    lock (_CachedAssemblies)
                        _Evaluated[file] = File.GetLastWriteTimeUtc(file);
                }
                catch { }

                return STEM.Sys.IO.Path.GetFileName(file);
            }
        }
    }
}
