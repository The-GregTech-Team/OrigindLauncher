using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using OrigindLauncher.Resources.Client;
using OrigindLauncher.Resources.Server;
using OrigindLauncher.Resources.Server.Data;
using OrigindLauncher.Resources.String;
using OrigindLauncher.Resources.Utils;

namespace OrigindLauncher.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for GameUpdatingDialog.xaml
    /// </summary>
    public partial class GameUpdatingDialog
    {
        private UpdateInfo UpdateInfo { get; }
        private Dictionary<FileEntry, TextBlock> _deleteDictionary = new Dictionary<FileEntry, TextBlock>();
        private Dictionary<DownloadInfo, TextBlock> _downloadTbDictionary = new Dictionary<DownloadInfo, TextBlock>();
        private Dictionary<DownloadInfo, ProgressBar> _downloadProgressDictionary = new Dictionary<DownloadInfo, ProgressBar>();

        public bool Result { get; private set; }

        public GameUpdatingDialog(UpdateInfo updateInfo)
        {
            UpdateInfo = updateInfo;
            InitializeComponent();
            foreach (var deleteEntry in updateInfo.FilesToDelete)
            {
                var textblock = new TextBlock { Text = $"删除 {deleteEntry.Path}", Margin = new Thickness(4) };
                UpdatePanel.Children.Add(textblock);
                _deleteDictionary.Add(deleteEntry, textblock);
            }

            foreach (var downloadEntry in updateInfo.FilesToDownload)
            {
                var textBlock = new TextBlock { Text = $"下载 {downloadEntry.Path}" };
                var progressBar = new ProgressBar { Maximum = 1 };
                _downloadTbDictionary.Add(downloadEntry, textBlock);
                _downloadProgressDictionary.Add(downloadEntry, progressBar);
                UpdatePanel.Children.Add(textBlock);
                UpdatePanel.Children.Add(progressBar);
            }
        }

        private bool isRunning = true;
        private async void Confirm(object sender, RoutedEventArgs e)
        {
            ConfirmBtn.IsEnabled = false;
            await Task.Run(() =>
            {
                Parallel.ForEach(UpdateInfo.FilesToDelete, deleteEntry =>
                {
                    if (!isRunning) return;

                    try
                    {
                        if (File.Exists(deleteEntry.Path))
                            File.Delete(deleteEntry.Path);

                        var current =
                            ClientManager.CurrentInfo.Files.Find(f => f.GetHashCode() == deleteEntry.GetHashCode());
                        ClientManager.CurrentInfo.Files.Remove(current);
                        Dispatcher.Invoke(() => UpdatePanel.Children.Remove(_deleteDictionary[deleteEntry]));
                    }
                    catch (Exception exception)
                    {
                        MessageUploadManager.CrashReport(new UploadData($"更新器V2发生异常 {exception.SerializeException()}"));
                        isRunning = false;
                    }

                });

                Parallel.ForEach(UpdateInfo.FilesToDownload, downloadEntry =>
                {
                    if (!isRunning) return;
                    try
                    {
                        var wc = new WebClient();
                        var path = ClientManager.GetGameStorageDirectory(downloadEntry.Path);
                        if (File.Exists(path))
                        {
                            if (SHA128Helper.Compute(File.Open(path, FileMode.Open))== downloadEntry.Hash)
                                goto finish;

                            File.Delete(path);
                        }

                        wc.DownloadProgressChanged += (o, args) => Dispatcher.Invoke(() =>
                        {
                            var progress = _downloadProgressDictionary[downloadEntry];
                            progress.Value = args.BytesReceived / (double)args.TotalBytesToReceive;
                        });
                        wc.DownloadFileTaskAsync(downloadEntry.Url, path).Wait();

                        ClientManager.CurrentInfo.Files.Add(new FileEntry(downloadEntry.Path, downloadEntry.Hash));
                        finish:
                        Dispatcher.Invoke(() =>
                        {
                            UpdatePanel.Children.Remove(_downloadProgressDictionary[downloadEntry]);
                            UpdatePanel.Children.Remove(_downloadTbDictionary[downloadEntry]);
                        });
                    }
                    catch (Exception exception)
                    {
                        MessageUploadManager.CrashReport(new UploadData($"更新器V2发生异常 {exception.SerializeException()}"));
                        isRunning = false;
                    }
                });
                ClientManager.Save();
                Result = isRunning;
                
                Close();
            });
        }

        private void Close()
        {
            DialogHost.CloseDialogCommand.Execute(this, this);
        }
    }
}
