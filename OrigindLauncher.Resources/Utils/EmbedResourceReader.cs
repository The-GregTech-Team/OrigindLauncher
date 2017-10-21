using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace OrigindLauncher.Resources.Utils
{
    public static class EmbedResourceReader
    {
        public static string Read(string name)
        {
            var stream = GetStream(name);
            return stream == null ? null : new StreamReader(stream).ReadToEnd();
        }

        public static Stream GetStream(string name)
        {
            var currentAssembly = Assembly.GetCallingAssembly();
            return currentAssembly.GetManifestResourceStream(name);
        }

    }
}
