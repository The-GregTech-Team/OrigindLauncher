using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using OrigindLauncher.Resources;
using OrigindLauncher.Resources.Configs;
using OrigindLauncher.Resources.Core;
using OrigindLauncher.UI;

namespace OrigindLauncher
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            if (Config.Instance.DisableHardwareSpeedup)
                RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

            if (args.Any(s => s == "Setup") || !File.Exists(Definitions.ConfigJsonPath))
            {
                var app1 = new App();
                app1.InitializeComponent();
                app1.Run(new SetupWindow());
                return;
            }
            else if (args.Length == 3 && args.Any(s => s == "Update"))
            {
                var app = new App();
                app.InitializeComponent();
                AutoUpdater.UpdateInternal(args[1], args[2], app);
                return;
            }
            else
            {
#if !DEBUG
                try
                {
                    if (AutoUpdater.HasUpdate) AutoUpdater.Update();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
#endif

                var app = new App();
                app.InitializeComponent();
                app.Run(new MainWindow());
            }


        }
    }
}