using System;
using System.IO;
using System.Linq;
using OrigindLauncher.Resources;
using OrigindLauncher.UI;

namespace OrigindLauncher
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Any(s => s == "Setup") || !File.Exists(Definitions.ConfigJsonPath))
            {
                var app1 = new App();
                app1.InitializeComponent();
                app1.Run(new SetupWindow());
                return;
            }


            var app = new App();
            app.InitializeComponent();
            app.Run(new MainWindow());
        }
    }
}