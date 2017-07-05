using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using OrigindLauncher.Resources;
using OrigindLauncher.UI.Code;

namespace OrigindLauncher.UI
{
    /// <summary>
    ///     Interaction logic for AutoUpdaterWindow.xaml
    /// </summary>
    public partial class AutoUpdaterWindow : Window
    {
        public AutoUpdaterWindow(string currentLauncherPath)
        {
            CurrentLauncherPath = currentLauncherPath;
            InitializeComponent();
            MainProgressBar.Maximum = 1;

            Task.Run(() =>
            {
                var wc = new WebClient();
                wc.DownloadProgressChanged += (sender, args) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        MainProgressBar.IsIndeterminate = false;
                        MainProgressBar.Value = args.BytesReceived / (double) args.TotalBytesToReceive;
                    });
                };
                wc.DownloadFileTaskAsync(new Uri($"{Definitions.OrigindServerUrl}/{Definitions.Rest.LauncherDownload}"),
                    CurrentLauncherPath).Wait();
                WcOnDownloadFileCompleted();
            });
        }

        public string CurrentLauncherPath { get; }

        private void WcOnDownloadFileCompleted()
        {
            Dispatcher.Invoke(() => this.FlyoutAndClose());
            Process.Start(new ProcessStartInfo(CurrentLauncherPath) {UseShellExecute = false});
            Environment.Exit(0);
        }
    }
}