using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using OrigindLauncher.Resources.Configs;
using OrigindLauncher.Resources.Server;
using OrigindLauncher.UI;
using RestSharp;

namespace OrigindLauncher.Resources.Core
{
    public static class AutoUpdater
    {
        public static bool HasUpdate => GetVersion() != Config.LauncherVersion;

        private static int GetVersion()
        {
            var rc = new RestClient(Definitions.OrigindServerUrl);
            var result = rc.Get(RestRequestFactory.Create(Definitions.Rest.LauncherVersion));
            return int.Parse(result.Content);
        }

        public static void Update()
        {
            var temppath = $"{Path.GetTempPath()}{Guid.NewGuid():D}.exe";
            var currentLauncherPath = Process.GetCurrentProcess().MainModule.FileName;
            File.Copy(currentLauncherPath, temppath);
            Process.Start(temppath, $"Update \"{currentLauncherPath}\" {Process.GetCurrentProcess().Id}");
            Environment.Exit(0);
        }

        internal static void UpdateInternal(string currentLauncherPath, string processid, Application app)
        {
            try
            {
                Process.GetProcessById(int.Parse(processid)).WaitForExit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            if (File.Exists(currentLauncherPath))
                File.Delete(currentLauncherPath);

            app.Run(new AutoUpdaterWindow(currentLauncherPath));
        }
    }
}