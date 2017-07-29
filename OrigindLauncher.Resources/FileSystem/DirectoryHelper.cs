using System;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Security.AccessControl;

namespace OrigindLauncher.Resources.FileSystem
{
    public static class DirectoryHelper
    {
        public static bool IsCurrentDirectoryWritable
        {
            get
            {
                try
                {
                    File.WriteAllText("test.txt", @"nothing");
                    File.Delete("test.txt");
                    return true;
                }
                catch (UnauthorizedAccessException)
                {
                    return false;
                }
                catch (SecurityException)
                {
                    return false;
                }
                catch (DirectoryNotFoundException)
                {
                    return false;
                }
                catch (IOException)
                {
                    return false;
                }
            }
        }

        public static void EnsureDirectoryExists(string directoryName)
        {
            if (!Directory.Exists(directoryName)) Directory.CreateDirectory(directoryName);
        }

        public static void AddCurrentDirectoryWritePermission()
        {
            Process.Start(
                new ProcessStartInfo(Process.GetCurrentProcess().MainModule.FileName,
                    "AddPermissions") {Verb = "runas"})?.WaitForExit();
        }

        internal static void AddCurrentDirectoryWritePermissionInternal()
        {
            var info = new DirectoryInfo(".");
            var control = info.GetAccessControl();
            control.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl,
                AccessControlType.Allow));
            info.SetAccessControl(control);
        }
    }
}