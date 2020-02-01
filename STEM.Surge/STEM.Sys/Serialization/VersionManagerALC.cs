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
            if (_AssemblyMap.ContainsKey(assemblyName))
            {
                return _AssemblyMap[assemblyName];
            }

            return base.LoadFromAssemblyName(assemblyName);
        }

        static List<VersionManagerALC> _ALCs = new List<VersionManagerALC>();
        static Dictionary<AssemblyName, VersionManagerALC> _ContextMap = new Dictionary<AssemblyName, VersionManagerALC>();
        Dictionary<AssemblyName, Assembly> _AssemblyMap = new Dictionary<AssemblyName, Assembly>();

        public static Assembly LoadFromFile(string path, VersionManagerALC context)
        {
            lock (_ALCs)
            {
                try
                {
                    AssemblyName asmName = AssemblyName.GetAssemblyName(path);

                    if (_ContextMap.ContainsKey(asmName))
                        return _ContextMap[asmName]._AssemblyMap[asmName];

                    VersionManagerALC c = _ALCs.FirstOrDefault(i => i == context);
                    if (c != null)
                    {
                        try
                        {
                            Assembly assembly = c.LoadFromAssemblyPath(path);
                            c._AssemblyMap[assembly.GetName()] = assembly;
                            _ContextMap[asmName] = c;

                            return assembly;
                        }
                        catch { }

                        return null;
                    }
                    else
                    {
                        foreach (VersionManagerALC i in _ALCs)
                        {
                            Assembly assembly = LoadFromFile(path, i);

                            if (assembly != null)
                                return assembly;
                        }
                    }

                    VersionManagerALC newContext = new VersionManagerALC();
                    _ALCs.Add(newContext);
                    return LoadFromFile(path, newContext);
                }
                catch { }

                return null;
            }
        }
    }
}
