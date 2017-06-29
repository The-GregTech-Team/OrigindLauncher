using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using OrigindLauncher.Resources.Client;
using OrigindLauncher.Resources.Configs;
using OrigindLauncher.Resources.Server;
using OrigindLauncher.UI;
using OrigindLauncher.UI.Code;

namespace OrigindLauncher
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            WelcomeMessage.Text += Config.Instance.PlayerAccount.Username;
        }

        private async void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;

            var status = await Task.Run(() => Config.Instance.PlayerAccount.UpdateLoginStatus());
            if (status != LoginStatus.Successful)
            {
                MainSnackbar.MessageQueue.Enqueue("登录状态刷新失败.");
                StartButton.IsEnabled = true;
                return;
            }
            if (ClientManager.CheckUpdate())
            {
                MainSnackbar.MessageQueue.Enqueue("正在更新客户端");
                var dm = new DownloadManager();

                await Task.Run(() =>
                {
                    var ar = new AutoResetEvent(false);

                    var dl = ClientManager.Update(s => { Dispatcher.Invoke(() => dm.downloadFileCompleted(s)); },
                        s => { Dispatcher.Invoke(() => dm.downloadProgressChanged(s)); },
                        args => { Dispatcher.Invoke(() => dm.onError(args)); }, () =>
                        {
                            Dispatcher.Invoke(() => dm.allDone());

                            ar.Set();
                        });
                    Dispatcher.Invoke(() => dm.allfiles = dl);
                    Dispatcher.Invoke(() => dm.Init());
                    Dispatcher.Invoke(() => dm.Show());

                    ar.WaitOne();
                    while (File.Exists(ClientManager.AssetsDownloadPath))
                        Thread.Sleep(100);
                });
            }
            var gm = new GameManager();
            gm.OnError += result => { Dispatcher.Invoke(() => MainSnackbar.MessageQueue.Enqueue(result.Exception)); };
            gm.OnGameExit += (handle, i) => { };
            gm.OnGameLog += (handle, s) => { };

            gm.Run();
            StartButton.IsEnabled = true;

        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.FlyoutAndClose(() => { Application.Current.Shutdown(); });
        }

        private void OpenDMinecraft(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer", ClientManager.GameStorageDirectory);
        }
    }
}