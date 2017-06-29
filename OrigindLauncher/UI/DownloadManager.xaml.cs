using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using OrigindLauncher.Resources.Client;
using OrigindLauncher.Resources.Web;
using OrigindLauncher.UI.Code;

namespace OrigindLauncher.UI
{
    /// <summary>
    ///     Interaction logic for DownloadManager.xaml
    /// </summary>
    public partial class DownloadManager : Window
    {
        public DownloadStatusInfo allfiles;
        private Dictionary<string, ProgressBar> progressBars;

        private Dictionary<string, TextBlock> textBlocks;

        public DownloadManager()
        {
            InitializeComponent();
        }

        public void Init()
        {
            textBlocks = new Dictionary<string, TextBlock>(allfiles.FileNameList.Count);
            progressBars = new Dictionary<string, ProgressBar>(allfiles.FileNameList.Count);
            foreach (var file in allfiles.FileNameList)
            {
                var textBlock = new TextBlock {Margin = new Thickness(4, 0, 0, 8), Text = $"下载中 {file}"};
                MainPanel.Children.Add(textBlock);
                textBlocks.Add(file, textBlock);
                var progressBar = new ProgressBar
                {
                    Margin = new Thickness(4, 0, 0, 12),
                    IsIndeterminate = true,
                    Maximum = 1
                };
                MainPanel.Children.Add(progressBar);
                progressBars.Add(file, progressBar);
            }
        }

        public void allDone()
        {
            this.FlyoutAndClose();
        }

        public void onError(OnErrorEventArgs onErrorEventArgs)
        {
            if (textBlocks.ContainsKey(onErrorEventArgs.FileLocation))
                textBlocks[onErrorEventArgs.FileLocation].Text = $"错误 {onErrorEventArgs.FileLocation}";
        }

        public void downloadProgressChanged(DownloadProgressChangeEventArgs downloadProgressChangeEventArgs)
        {
            if (progressBars.ContainsKey(downloadProgressChangeEventArgs.FileLocation))
            {
                var r = downloadProgressChangeEventArgs.BytesReceived /
                        (double) downloadProgressChangeEventArgs.TotalBytesToReceive;
                if (r >= 0 && r <= 1)
                {
                    progressBars[downloadProgressChangeEventArgs.FileLocation].IsIndeterminate = false;
                    progressBars[downloadProgressChangeEventArgs.FileLocation].AnimeToValueAsPercent(r);
                }
            }
        }

        public void downloadFileCompleted(CompletedEventArgs completedEventArgs)
        {
            if (textBlocks.ContainsKey(completedEventArgs.FileLocation))
                MainPanel.Children.Remove(textBlocks[completedEventArgs.FileLocation]);
            if (progressBars.ContainsKey(completedEventArgs.FileLocation))
                MainPanel.Children.Remove(progressBars[completedEventArgs.FileLocation]);
        }
    }
}