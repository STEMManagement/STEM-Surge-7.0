using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.ServiceProcess;
using System.IO;

namespace STEM.SurgeService
{
    public partial class SurgeService : ServiceBase
    {
        public SurgeService()
        {
            InitializeComponent();
        }

        public void Start(string[] args)
        {
            if (_ControlObj == null)
            {
                _ControlObj = Activator.CreateInstance(GetControlObjectType("STEM.Surge.Control"));

                MethodInfo methodInfo = _ControlObj.GetType().GetMethod("Open");
                methodInfo.Invoke(_ControlObj, new object[] { new List<string>(new string[] { Path.Combine(System.Environment.CurrentDirectory, "SurgeService.cfg") }) });
            }
        }

        protected override void OnStart(string[] args)
        {
            Start(args);
        }

        protected override void OnStop()
        {
            if (_ControlObj != null)
            {
                MethodInfo methodInfo = _ControlObj.GetType().GetMethod("Close");
                methodInfo.Invoke(_ControlObj, null);
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
                            Assembly.LoadFile(file);
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
                            Assembly.LoadFile(file);
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
                            Assembly.LoadFile(file);
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

                            Assembly a = Assembly.LoadFile(file);

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
