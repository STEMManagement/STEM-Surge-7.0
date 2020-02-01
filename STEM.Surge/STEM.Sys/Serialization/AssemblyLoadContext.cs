using System;
using System.Reflection;

namespace System.Runtime.LoaderStub
{    
    public class AssemblyLoadContext
    {
        object _ALC = null;

        public AssemblyLoadContext()
        {
            if (!Assembly.GetEntryAssembly().GetCustomAttribute<System.Runtime.Versioning.TargetFrameworkAttribute>().FrameworkName.StartsWith(".NetFramework", StringComparison.InvariantCultureIgnoreCase))
            {
                _ALC = Activator.CreateInstance(Type.GetType("System.Runtime.LoaderStub._AssemblyLoadContext"));
            }
        }

        protected virtual Assembly Load(AssemblyName assemblyName)
        {
            return null;
        }

        public Assembly LoadFromAssemblyName(AssemblyName assemblyName)
        {
            if (_ALC == null)
                return null;
            else
                return _ALC.GetType().GetMethod("LoadFromAssemblyNameStub").Invoke(_ALC, new object[] { assemblyName }) as Assembly;
        }

        public Assembly LoadFromAssemblyPath(string path)
        {
            if (_ALC == null)
                return Assembly.LoadFile(path);
            else
                return _ALC.GetType().GetMethod("LoadFromAssemblyPath").Invoke(_ALC, new object[] { path }) as Assembly;
        }
    }

    public class _AssemblyLoadContext : System.Runtime.Loader.AssemblyLoadContext
    {
        public Assembly LoadFromAssemblyNameStub(AssemblyName assemblyName)
        {
            return System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}
