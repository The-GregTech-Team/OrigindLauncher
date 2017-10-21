using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace OrigindLauncher.Resources.Utils
{
    public static class JavaFinder
    {
        public static IEnumerable<string> FindJava()
        {
            RegistryKey rootReg;
            if (Environment.Is64BitOperatingSystem)
                rootReg = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                    .OpenSubKey("SOFTWARE");
            else
                rootReg = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
                    .OpenSubKey("SOFTWARE");

            return rootReg == null
                ? new string[0]
                : FindJavaInternal(rootReg).Union(FindJavaInternal(rootReg.OpenSubKey("Wow6432Node")));
        }

        private static IEnumerable<string> FindJavaInternal(RegistryKey registry)
        {
            try
            {
                var registryKey = registry.OpenSubKey("JavaSoft");
                registry = registryKey?.OpenSubKey("Java Runtime Environment");
                if (registryKey == null || registry == null)
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
                    where str.Contains("8")
                    select str + @"\bin\javaw.exe";
            }
            catch (Exception)
            {
                return new string[0];
            }
        }
    }
}