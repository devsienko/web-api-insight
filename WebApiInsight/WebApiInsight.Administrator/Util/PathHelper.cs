using System;
using System.IO;
using System.Reflection;

namespace WebApiInsight.Administrator
{
    public static class PathHelper
    {
        public static string GetAssemblyLocation()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var codebase = new Uri(assembly.CodeBase);
            var path = Path.GetDirectoryName(codebase.LocalPath);
            var result = Directory.GetParent(path).FullName;
            return result;
        }
    }
}