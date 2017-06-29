using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace OrigindLauncher.Resources.Utils
{
    public static class JavaHelper
    {
        public static IEnumerable<string> FindJava()
        {
            var rootReg = Environment.Is64BitOperatingSystem
                ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE")
                : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE");

            return rootReg == null
                ? new string[0]
                : FindJavaInternal(rootReg).Union(FindJavaInternal(rootReg.OpenSubKey("Wow6432Node")));
        }

        private static IEnumerable<string> FindJavaInternal(RegistryKey registry)
        {
            try
            {
                var registryKey = registry.OpenSubKey("JavaSoft");
                if (registryKey == null || (registry = registryKey.OpenSubKey("Java Runtime Environment")) == null)
                    return new string[0];
                return from ver in registry.GetSubKeyNames()
                    select registry.OpenSubKey(ver)
                    into command
                    where command != null
                    select command.GetValue("JavaHome")
                    into javaHomes
                    where javaHomes != null
                    select javaHomes.ToString()
                    into str
                    where !string.IsNullOrWhiteSpace(str)
                    select str + @"\bin\java.exe";
            }
            catch
            {
                return new string[0];
            }
        }
    }
}