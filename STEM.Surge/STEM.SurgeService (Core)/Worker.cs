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
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Loader;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace STEM.SurgeService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_ControlObj == null)
                {
                    _ControlObj = Activator.CreateInstance(GetControlObjectType("STEM.Surge.Control"));

                    MethodInfo methodInfo = _ControlObj.GetType().GetMethod("Open");
                    methodInfo.Invoke(_ControlObj, new object[] { new List<string>(new string[] { Path.Combine(System.Environment.CurrentDirectory, "SurgeService.cfg") }) });
                }

                await Task.Delay(1000, stoppingToken);
            }
        }

        object _ControlObj = null;

        Type GetControlObjectType(string tgt)
        {
            foreach (string file in Directory.GetFiles(System.Environment.CurrentDirectory))
            {
                try
                {
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(file);

                    if (!string.IsNullOrEmpty(fvi.OriginalFilename) &&
                        !System.IO.Path.GetFileName(fvi.OriginalFilename).StartsWith("STEM.Auth", StringComparison.InvariantCultureIgnoreCase) &&
                        System.IO.Path.GetFileName(fvi.OriginalFilename).EndsWith(".DLL", StringComparison.InvariantCultureIgnoreCase))
                    {
                        string fn = Path.Combine(System.Environment.CurrentDirectory, System.IO.Path.GetFileName(fvi.OriginalFilename));

                        if (!File.Exists(fn))
                        {
                            File.Move(file, fn);
                        }
                        else if (!fn.Equals(file, StringComparison.InvariantCultureIgnoreCase))
                            if (File.GetLastWriteTimeUtc(fn) >= File.GetLastWriteTimeUtc(file))
                            {
                                File.Delete(file);
                            }
                            else
                            {
                                File.Delete(fn);
                                File.Move(file, fn);
                            }
                    }
                }
                catch { }
            }

            foreach (string file in Directory.GetFiles(System.Environment.CurrentDirectory, "STEM.Sys.Internal*.dll"))
            {
                try
                {
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(file);

                    if (!string.IsNullOrEmpty(fvi.OriginalFilename) &&
                        System.IO.Path.GetFileName(fvi.OriginalFilename).EndsWith(".DLL", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (System.IO.Path.GetFileNameWithoutExtension(fvi.OriginalFilename).ToUpper().StartsWith("STEM.SYS.INTERNAL"))
                        {
                            AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
                            break;
                        }
                    }
                }
                catch { }
            }

            foreach (string file in Directory.GetFiles(System.Environment.CurrentDirectory, "STEM.Sys*.dll"))
            {
                try
                {
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(file);

                    if (!string.IsNullOrEmpty(fvi.OriginalFilename) &&
                        System.IO.Path.GetFileName(fvi.OriginalFilename).EndsWith(".DLL", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (!System.IO.Path.GetFileNameWithoutExtension(fvi.OriginalFilename).ToUpper().StartsWith("STEM.SYS.INTERNAL") &&
                            System.IO.Path.GetFileNameWithoutExtension(fvi.OriginalFilename).ToUpper().StartsWith("STEM.SYS"))
                        {
                            AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
                            break;
                        }
                    }
                }
                catch { }
            }

            foreach (string file in Directory.GetFiles(System.Environment.CurrentDirectory, "STEM.Surge*.dll"))
            {
                try
                {
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(file);

                    if (!string.IsNullOrEmpty(fvi.OriginalFilename) &&
                        System.IO.Path.GetFileName(fvi.OriginalFilename).EndsWith(".DLL", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (!System.IO.Path.GetFileNameWithoutExtension(fvi.OriginalFilename).ToUpper().StartsWith("STEM.SURGE.INTERNAL") &&
                            System.IO.Path.GetFileNameWithoutExtension(fvi.OriginalFilename).ToUpper().StartsWith("STEM.SURGE"))
                        {
                            AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
                            break;
                        }
                    }
                }
                catch { }
            }

            Type control = null;
            foreach (string file in Directory.GetFiles(System.Environment.CurrentDirectory, "STEM.Surge.Internal*.dll"))
            {
                try
                {
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(file);

                    if (!string.IsNullOrEmpty(fvi.OriginalFilename) &&
                        System.IO.Path.GetFileName(fvi.OriginalFilename).EndsWith(".DLL", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (System.IO.Path.GetFileNameWithoutExtension(fvi.OriginalFilename).ToUpper().StartsWith("STEM.SURGE.INTERNAL"))
                        {
                            Assembly asm = null;

                            Assembly a = AssemblyLoadContext.Default.LoadFromAssemblyPath(file);

                            if ("STEM.SURGE.CONTROL" == tgt.ToUpper())
                                asm = a;

                            if (asm != null && control == null)
                                foreach (Type t in asm.GetTypes())
                                    if (t.FullName.Equals(tgt, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        control = t;
                                        break;
                                    }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string s = ex.ToString();
                }
            }

            if (control == null)
                throw new Exception(tgt + " not found.");

            return control;
        }
    }
}
