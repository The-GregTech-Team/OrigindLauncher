using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
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
        static Program()
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName));
        }

        [STAThread]
        private static void Main(string[] args)
        {
            if (Config.Instance.DisableHardwareSpeedup)
                RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

            if (args.Length == 3 && args.Any(s => s == "Update"))
            {
                var app = new App();
                app.InitializeComponent();
                AutoUpdater.UpdateInternal(args[1], args[2], app);
                return;
            }

            using (var mutex = new Mutex(false,
                $"Global\\OrigindLauncher_{Process.GetCurrentProcess().MainModule.FileName.GetHashCode()}"))
            {
                if (!mutex.WaitOne(4000, false))
                {
                    MessageBox.Show("有一个 Origind Launcher 进程正在运行. 这个进程将会退出.");
                    return;
                }
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


                if (args.Any(s => s == "Setup") || !File.Exists(Definitions.ConfigJsonPath))
                {
                    var app1 = new App();
                    app1.InitializeComponent();
                    app1.Run(new SetupWindow());
                }
                else
                {
                    var app = new App();
                    app.InitializeComponent();
                    app.Run(new MainWindow());
                }
            }
        }
    }
}