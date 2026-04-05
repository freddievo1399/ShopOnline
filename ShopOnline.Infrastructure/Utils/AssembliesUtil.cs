using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Infrastructure.Utils;

public static class AssembliesUtil
{
    private static List<Assembly>? allAssemblies = null;
    public static IEnumerable<Assembly> GetAssemblies()
    {
        if (allAssemblies == null)
        {
            var modules = new List<Assembly>();
            var abc = Assembly.GetExecutingAssembly();
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw (new Exception("Not fould"));
            var files = Directory.GetFiles(path, "*.dll");

            foreach (string dll in files.Where(x => Path.GetFileName(x).StartsWith("ShopOnline")|| Path.GetFileName(x).StartsWith("WebApp")))
            {
                if (modules.Any(t => t.GetName().Name == dll))
                    continue;

                modules.Add(Assembly.LoadFile(dll));
            }

            allAssemblies = modules;
        }

        return allAssemblies;
    }
    public static IEnumerable<Assembly> GetAssembliesBlazor()
    {
        if (allAssemblies == null)
        {
            GetAssemblies();
        }

        var Assemblies = allAssemblies?.Where(x => x.ManifestModule.Name.EndsWith(".Blazor.dll")|| x.ManifestModule.Name.StartsWith("WebApp")).ToList() ?? new List<Assembly>();

        return Assemblies;
    }
    public static IEnumerable<Assembly> GetAssembliesServer()
    {
        if (allAssemblies == null)
        {
            GetAssemblies();
        }

        var Assemblies = allAssemblies?.Where(x => x.ManifestModule.Name.EndsWith(".Server.dll")).ToList() ?? new List<Assembly>();
        return Assemblies;
    }
    public static IEnumerable<TypeInfo> GetTypes<T>(this IEnumerable<Assembly> assemblies)
    {
        return assemblies.SelectMany(a => a.DefinedTypes.Where(x => x.GetInterfaces().Contains(typeof(T))));
    }
    public static IEnumerable<T> GetInstances<T>(this IEnumerable<Assembly> assemblies)
    {
        if (allAssemblies == null)
        {
            GetAssemblies();
        }
        ;
        var instances = new List<T>();

        foreach (Type implementation in assemblies.GetTypes<T>())
        {
            if (implementation.GetTypeInfo().IsAbstract)
                continue;

            var instance = Activator.CreateInstance(implementation);
            if (instance == null)
            {
                continue;
            }
            instances.Add((T)instance);
        }

        return instances;
    }
}

