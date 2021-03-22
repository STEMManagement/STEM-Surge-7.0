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
using System.Diagnostics;
using System.ComponentModel;
using System.Xml.Serialization;
using STEM.Sys.Threading;

namespace STEM.Sys
{
    public class Control
    {
        static ThreadPool ThreadPool = new ThreadPool(Environment.ProcessorCount, false);
        object _ObjectLock = new object();

        [XmlIgnore]
        [Browsable(false)]
        public bool ControlOpen
        {
            get;
            private set;
        }

        static readonly bool _IsWindows = false;
        static readonly bool _IsX64 = true;

        static Control()
        {
            _IsWindows = false;
            try
            {
                _IsWindows = System.Runtime.InteropServices.RuntimeInformation.OSDescription.StartsWith("Microsoft Windows", StringComparison.InvariantCultureIgnoreCase);
            }
            catch { }

            _IsX64 = true;
            try
            {
                _IsX64 = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString().StartsWith("X64", StringComparison.InvariantCultureIgnoreCase);
            }
            catch { }
        }

        public static bool IsWindows
        {
            get
            {
                return _IsWindows;
            }
        }

        public static bool IsX64
        {
            get
            {
                return _IsX64;
            }
        }

        public Control()
        {
            ControlOpen = false;

            System.AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            System.Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            bool isSandbox = STEM.Sys.IO.Path.GetFileName(STEM.Sys.IO.Path.GetDirectoryName(System.Environment.CurrentDirectory)).Equals("Sandboxes", StringComparison.InvariantCultureIgnoreCase);

            if (!isSandbox)
                CloseChildren();
            
            STEM.Sys.Serialization.VersionManager.Initialize(new List<string>(), true, !isSandbox);

            ThreadPool.BeginAsync(new System.Threading.ThreadStart(CheckInstall), TimeSpan.FromSeconds(3));
        }

        public static Dictionary<string, List<string>> ParseArgs(List<string> args)
        {
            Dictionary<string, List<string>> ret = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);

            if (args != null)
            {
                string full = String.Join(" ", args).Trim();

                while (full.Trim().Length > 0)
                {
                    string key = "-";
                    if (full.StartsWith("-", StringComparison.InvariantCultureIgnoreCase))
                    {
                        key = full.Substring(0, full.IndexOf(' '));
                        full = full.Substring(key.Length).Trim() + " ";
                    }

                    if (!ret.ContainsKey(key))
                        ret[key] = new List<string>();

                    while (full.Trim().Length > 0 && !full.Trim().StartsWith("-", StringComparison.InvariantCultureIgnoreCase))
                    {
                        string value = null;

                        if (full.StartsWith("'", StringComparison.InvariantCultureIgnoreCase))
                        {
                            value = full.Substring(1, full.IndexOf('\'', 1) - 1);
                            full = full.Substring(value.Length + 2).Trim() + " ";
                        }
                        else if (full.StartsWith("\"", StringComparison.InvariantCultureIgnoreCase))
                        {
                            value = full.Substring(1, full.IndexOf('\"', 1) - 1);
                            full = full.Substring(value.Length + 2).Trim() + " ";
                        }
                        else
                        {
                            value = full.Substring(0, full.IndexOf(' '));
                            full = full.Substring(value.Length).Trim() + " ";
                        }

                        if (value != null)
                            ret[key].Add(value);
                    }
                }
            }

            return ret;
        }

        public static Dictionary<string, List<string>> ParseArgs(string[] args)
        {
            if (args != null)
                return ParseArgs(args.ToList());
            else
                return new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);
        }

        public virtual string OpenUseArgs()
        {
            return ("");
        }

        public virtual bool Open(List<string> args)
        {
            lock (_ObjectLock)
            {
                if (ControlOpen)
                    return false;
                
                ControlOpen = true;
            }

            return true;
        }

        public virtual bool Close()
        {
            lock (_ObjectLock)
            {
                if (!ControlOpen)
                    return false;

                ControlOpen = false;
                ThreadPool.EndAsync(new System.Threading.ThreadStart(CheckInstall));
            }

            return true;
        }

        public void CheckInstall()
        {
            try
            {
                bool sysFound = false;
                bool restart = false;

                foreach (string file in Directory.GetFiles(System.Environment.CurrentDirectory))
                {
                    try
                    {
                        FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(file);

                        if (!string.IsNullOrEmpty(fvi.OriginalFilename) &&
                            !STEM.Sys.IO.Path.GetFileName(fvi.OriginalFilename).StartsWith("STEM.Auth", StringComparison.InvariantCultureIgnoreCase) &&
                            !STEM.Sys.IO.Path.GetFileName(fvi.OriginalFilename).StartsWith("STEM.SurgeService", StringComparison.InvariantCultureIgnoreCase) &&
                            STEM.Sys.IO.Path.GetFileName(fvi.OriginalFilename).StartsWith("STEM.", StringComparison.InvariantCultureIgnoreCase) &&
                            STEM.Sys.IO.Path.GetFileName(fvi.OriginalFilename).EndsWith(".DLL", StringComparison.InvariantCultureIgnoreCase))
                        {
                            string fn = Path.Combine(System.Environment.CurrentDirectory, STEM.Sys.IO.Path.GetFileName(fvi.OriginalFilename));

                            if (!File.Exists(fn))
                            {
                                File.Move(file, fn);
                            }
                            else if (!fn.Equals(file, StringComparison.InvariantCultureIgnoreCase) && File.GetLastWriteTimeUtc(fn) >= File.GetLastWriteTimeUtc(file))
                            {
                                File.Delete(file);
                            }
                            else if (!fn.Equals(file, StringComparison.InvariantCultureIgnoreCase))
                            {
                                restart = true;
                            }
                            
                            if (!fn.Equals(file, StringComparison.InvariantCultureIgnoreCase) && STEM.Sys.IO.Path.GetFileName(fvi.OriginalFilename).StartsWith("STEM.Sys.7", StringComparison.InvariantCultureIgnoreCase))
                                sysFound = true;
                        }
                    }
                    catch { }
                }

                if (restart && sysFound)
                    Restart(this);
            }
            catch { }
        }
        
        public static void Restart(Control sender)
        {
            if (!_IsWindows)
            {
                try
                {
                    string processName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

                    if (sender != null)
                        try
                        {
                            sender.Restarting();
                        }
                        catch { }

                    ProcessStartInfo si = new ProcessStartInfo();
                    si.CreateNoWindow = true;
                    si.UseShellExecute = true;

                    string r = Path.Combine(System.Environment.CurrentDirectory, processName);
                    
                    si.FileName = r;
                    si.Arguments = "-r";

                    Process.Start(si);

                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
                catch (Exception ex)
                {
                    STEM.Sys.EventLog.WriteEntry(System.Reflection.Assembly.GetEntryAssembly().GetName().Name + ".STEM.Sys.Control.Restart", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                }
            }
            else
            {
                try
                {
                    string exeFile = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                    string exeBase = Path.GetFileNameWithoutExtension(exeFile);

                    string r = Path.Combine(System.Environment.CurrentDirectory, exeBase + ".R.exe");

                    if (File.Exists(r))
                        File.Delete(r);

                    File.Copy(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, r);

                    if (sender != null)
                        try
                        {
                            sender.Restarting();
                        }
                        catch { }
                    
                    ProcessStartInfo si = new ProcessStartInfo();
                    si.CreateNoWindow = true;
                    si.UseShellExecute = true;

                    si.FileName = r;
                    si.Arguments = "-r";

                    Process.Start(si);

                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
                catch (Exception ex)
                {
                    STEM.Sys.EventLog.WriteEntry(System.Reflection.Assembly.GetEntryAssembly().GetName().Name + ".STEM.Sys.Control.Restart", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                }
            }
        }

        public static void CloseChildren()
        {
            try
            {
                List<string> tops = Directory.GetDirectories(System.Environment.CurrentDirectory, "*", SearchOption.TopDirectoryOnly).ToList();

                foreach (string top in tops)
                {
                    bool hadExe = false;

                    try
                    {
                        if (Directory.Exists(top))
                        {
                            List<string> dirs = Directory.GetDirectories(top, "*", SearchOption.TopDirectoryOnly).ToList();
                            foreach (string dir in dirs)
                            {
                                if (Directory.Exists(dir))
                                {
                                    string exeName = Directory.GetFiles(dir, "*.exe").FirstOrDefault();

                                    if (exeName != null)
                                    {
                                        hadExe = true;

                                        foreach (System.Diagnostics.Process proc in System.Diagnostics.Process.GetProcessesByName(Path.GetFileNameWithoutExtension(exeName)))
                                        {
                                            try
                                            {
                                                if (proc.MainModule.FileName.Equals(exeName, StringComparison.InvariantCultureIgnoreCase))
                                                    proc.Kill();
                                                else
                                                    continue;
                                            }
                                            catch { }
                                        }

                                        try
                                        {
                                            STEM.Sys.IO.Directory.STEM_Delete(dir, false);
                                        }
                                        catch { }
                                    }

                                    if (hadExe)
                                        try
                                        {
                                            STEM.Sys.IO.Directory.STEM_Delete(top, false);
                                        }
                                        catch { }
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            STEM.Sys.EventLog.WriteEntry(System.Reflection.Assembly.GetEntryAssembly().GetName().Name + ".STEM.Sys.Control", ((Exception)e.ExceptionObject).ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
        }

        protected virtual void Restarting()
        {
        }
    }
}
