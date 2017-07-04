using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using GoodTimeStudio.ServerPinger;
using MaterialDesignThemes.Wpf;
using OrigindLauncher.Resources.Client;
using OrigindLauncher.Resources.Configs;
using OrigindLauncher.Resources.Server;
using OrigindLauncher.UI;
using OrigindLauncher.UI.Code;
using OrigindLauncher.UI.Dialogs;
using Application = System.Windows.Application;

namespace OrigindLauncher
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private double _defaultLeft;
        private double _defaultTop;


        public MainWindow()
        {
            InitializeComponent();
            WelcomeMessage.Text += " " + Config.Instance.PlayerAccount.Username;
            try
            {
                var result = ServerInfoGetter.GetServerInfo();
                ServerMessage.Text += " " + result.players.online;
                ServerMessage.ToolTip = string.Join("\r\n", result.players.sample.Select(p => p.name));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            _defaultLeft = Left;
            _defaultTop = Top;
        }

        private async void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;

            // 刷新登录状态
            var status = await Task.Run(() => Config.Instance.PlayerAccount.UpdateLoginStatus());
            if (status != LoginStatus.Successful)
            {
                MainSnackbar.MessageQueue.Enqueue("登录状态刷新失败.");
                StartButton.IsEnabled = true;
                return;
            }

            // 检测更新状态
            bool updateStatus;
            try
            {
                updateStatus = ClientManager.CheckUpdate();
            }
            catch (Exception exception)
            {
                MainSnackbar.MessageQueue.Enqueue("更新检测失败." + exception);
                StartButton.IsEnabled = true;
                return;
            }

            // 等待更新
            if (updateStatus)
            {
                await UpdateClient();
            }

            // 启动游戏
            var gm = new GameManager();
            gm.OnError += result => { Dispatcher.Invoke(() => MainSnackbar.MessageQueue.Enqueue(result.Exception)); };
            gm.OnGameExit += (handle, i) =>
            {
                Environment.Exit(0);
            };
            gm.OnGameLog += (handle, s) =>
            {

            };

            // 
            if (File.Exists(ClientManager.GetGameStorageDirectory("mods") + @"\OrigindLauncherHelper.jar"))
            {
                if (Config.Instance.LaunchProgress)
                {
                    gm.OnGameLog += (lh, log) =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            LaunchProgress.OnGameLog(log);
                        });
                    };
                    LaunchProgress.Begin();
                }
            }
            else
            {
                await DialogHost.Show(new MessageDialog { Message = { Text = "你的客户端没有安装 OrigindLauncherHelper." } }, "RootDialog");
            }

            gm.Run();

            // 退出
            StartButton.IsEnabled = true;
            this.Flyout(() =>
            {
                this.Hide();
                //TODO
            });
        }

        private async Task<bool> UpdateClient()
        {
            MainSnackbar.MessageQueue.Enqueue("正在更新客户端");
            var dm = new DownloadManager();

            await Task.Run(() =>
            {
                var ar = new AutoResetEvent(false);

                var dl = ClientManager.Update(s => { Dispatcher.Invoke(() => dm.downloadFileCompleted(s)); },
                    s =>
                    {
                        Dispatcher.Invoke(() =>
                    {
                            dm.downloadProgressChanged(s);
                    });
                    },
                    args => { Dispatcher.Invoke(() => dm.onError(args)); }, () =>
                    {
                        Dispatcher.Invoke(() => dm.allDone());
                        ar.Set();
                    });
                Dispatcher.Invoke(() => dm.allfiles = dl);
                Dispatcher.Invoke(() => dm.Init());
                Dispatcher.Invoke(() => dm.Show());

                ar.WaitOne();
            });

            return dm.Result;
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.FlyoutAndClose(() => { Application.Current.Shutdown(); });
        }

        private void OpenDMinecraft(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer", Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\" + ClientManager.GameStorageDirectory);
        }

        private async void Options(object sender, RoutedEventArgs e)
        {
            await DialogHost.Show(new SettingsDialog(), "RootDialog");
        }

        private void Move(object sender, MouseButtonEventArgs e)
        {
            DragMove();

        }
    }
}