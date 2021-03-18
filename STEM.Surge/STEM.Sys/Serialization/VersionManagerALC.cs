using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace STEM.Sys.Serialization
{
    public class VersionManagerALC : System.Runtime.LoaderStub.AssemblyLoadContext
    {
        protected override Assembly Load(AssemblyName assemblyName)
        {
            Assembly asm = _AssemblyMap.Values.FirstOrDefault(i => i.GetName().ToString() == assemblyName.ToString());
            if (asm != null)
                return asm;

            return base.LoadFromAssemblyName(assemblyName);
        }

        static List<VersionManagerALC> _ALCs = new List<VersionManagerALC>();
        static Dictionary<string, VersionManagerALC> _ContextMap = new Dictionary<string, VersionManagerALC>();
        Dictionary<string, Assembly> _AssemblyMap = new Dictionary<string, Assembly>();

        public static Assembly LoadFromFile(string path, string xform, VersionManagerALC context)
        {
            lock (_ALCs)
            {
                try
                {
                    AssemblyName asmName = AssemblyName.GetAssemblyName(path);

                    if (_ContextMap.ContainsKey(xform))
                        return _ContextMap[xform]._AssemblyMap[xform];

                    VersionManagerALC c = _ALCs.FirstOrDefault(i => i == context);
                    if (c != null)
                    {
                        try
                        {
                            Assembly assembly = c.LoadFromAssemblyPath(path);

                            if (assembly != null)
                            {
                                c._AssemblyMap[xform] = assembly;
                                _ContextMap[xform] = c;
                            }

                            return assembly;
                        }
                        catch //(Exception ex)
                        {
                            //STEM.Sys.EventLog.WriteEntry("VersionManagerALC:LoadFromFile", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                        }

                        return null;
                    }
                    else
                    {
                        foreach (VersionManagerALC i in _ALCs)
                        {
                            Assembly assembly = LoadFromFile(path, xform, i);

                            if (assembly != null)
                                return assembly;
                        }
                    }

                    VersionManagerALC newContext = new VersionManagerALC();
                    _ALCs.Add(newContext);
                    return LoadFromFile(path, xform, newContext);
                }
                catch { }

                return null;
            }
        }
    }
}
