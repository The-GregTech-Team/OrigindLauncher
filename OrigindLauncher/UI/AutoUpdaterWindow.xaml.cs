using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OrigindLauncher.Resources;
using OrigindLauncher.UI.Code;

namespace OrigindLauncher.UI
{
    /// <summary>
    /// Interaction logic for AutoUpdaterWindow.xaml
    /// </summary>
    public partial class AutoUpdaterWindow : Window
    {
        public string CurrentLauncherPath { get; }

        public AutoUpdaterWindow(string currentLauncherPath)
        {
            CurrentLauncherPath = currentLauncherPath;
            InitializeComponent();

            Task.Run(() =>
            {
                var wc = new WebClient();

                wc.DownloadFileTaskAsync(new Uri($"{Definitions.OrigindServerUrl}/{Definitions.Rest.LauncherDownload}"),
                    CurrentLauncherPath).Wait();
                WcOnDownloadFileCompleted();
            });

        }

        private void WcOnDownloadFileCompleted()
        {
            
            this.Dispatcher.Invoke(() => this.FlyoutAndClose());
            while (!File.Exists(CurrentLauncherPath)) Thread.Sleep(100);
            Process.Start(new ProcessStartInfo(CurrentLauncherPath) { UseShellExecute = false });
            Environment.Exit(0);
        }
    }
}
