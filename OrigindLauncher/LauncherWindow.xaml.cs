﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using MaterialDesignThemes.Wpf;
using OrigindLauncher.Resources.Client;
using OrigindLauncher.Resources.Configs;
using OrigindLauncher.Resources.Core;
using OrigindLauncher.Resources.Server;
using OrigindLauncher.Resources.UI;
using OrigindLauncher.Resources.Utils;
using OrigindLauncher.UI.Code;
using OrigindLauncher.UI.Dialogs;

namespace OrigindLauncher
{
    /// <summary>
    ///     Interaction logic for LauncherWindow.xaml
    /// </summary>
    public partial class LauncherWindow
    {
        private static readonly Regex CheckUrlRegex =
                new Regex(
                    "((http|ftp|https):\\/\\/)?[\\w\\-_]+(\\.[\\w\\-_]+)+([\\w\\-\\.,@?^=%&amp;:/~\\+#]*[\\w\\-\\@?^=%&amp;/~\\+#])?")
            ;

        public LauncherWindow()
        {
            InitializeComponent();

            Trace.WriteLine("Initialzing Launch Window.");
            InitTheme();
            Trace.WriteLine("Done: Theme load.");

            //            WelcomeMessage.Text += $" {Config.Instance.PlayerAccount.Username}";
            //            TitleTextBlock.Text +=
            //                $" {Config.LauncherVersion}{(Config.Admins.Any(u => u == Config.Instance.PlayerAccount.Username) ? " 管理" : string.Empty)}";

            try
            {
                var result1 = ServerInfoGetter.GetServerInfoAsync();
                result1.Wait(2000); // 避免由服务器错误引起的无限等待
                if (result1.IsCompleted)
                {
                    Trace.WriteLine("Done: Server info get.");
                    var result = result1.Result;
                    ServerMessage.Text = $"服务器在线人数 {result.players.online}";
                    ServerMessage.ToolTip = result.players.sample != null ? string.Join("\r\n", result.players.sample.Select(p => p.name)) : "现在没有人..点这里来刷新!";
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }

#if !DEBUG
            try
            {
                if (AutoUpdater.HasUpdate)
                {
                    var version = AutoUpdater.GetVersion();
                    if (version - Config.LauncherVersion > 10) // 开玩笑
                    {
                        AutoUpdater.Update();
                        return;
                    }
                    MainSnackbar.MessageQueue.Enqueue($"启动器有更新啦！ {version}", "立即更新", AutoUpdater.Update);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
#endif
            Trace.WriteLine("Auto update check done.");

            Task.Run(() =>
            {
                Thread.Sleep(300);
                Dispatcher.Invoke(() =>
                {
                    var doubleAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(1)));
                    MainCard.BeginAnimation(OpacityProperty, doubleAnimation);
                    MainTransitioner.SelectedIndex = 1;
                });
            });

            Trace.WriteLine("Launch Window loaded.");
        }

        private static string[] Before { get; set; }

        public void InitTheme()
        {
            try
            {
                var num = Config.Instance.ThemeConfig.IsDark ? "2" : "1";
                var imageSource = ImageSourceGetter.GetImageSource($"/Images/Background{num}.png");
                BgCache.Source = imageSource; // Fix a bug
                Bg.ImageSource = imageSource;
                Bg2.Source = ImageSourceGetter.GetImageSource($"/Images/flat{num}.png");
            }
            catch (Exception e)
            {
                MainSnackbar.MessageQueue.Enqueue("警告: 加载主题时出现问题. 启用调试模式来看到更多.");
                Trace.WriteLine(e);
            }
        }

        private async void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            EnsureUpdatePathExists();
            //await UpdateUpdatePathAsync(); 其他服务器的兼容
            //foreach (Window currentWindow in Application.Current.Windows) 更好的方案
            //    currentWindow.Flyout();

            if (Config.Instance.PlayerAccount.Login() == LoginStatus.NotFound)
            {
                MainSnackbar.MessageQueue.Enqueue("登录失败.", "查看详情", () => Dispatcher.Invoke(async () =>
                {
                    var chooseDialog = new ChooseDialog("要重新注册吗？",
                        "很抱歉, 因为我们的技术删库跑路, 所有的玩家数据库都没了. 按下重新注册来用你当前的账号重新注册.", "重新注册");
                    await DialogHost.Show(chooseDialog, "RootDialog");
                    if (chooseDialog.Result)
                        Config.Instance.PlayerAccount.Register();
                }));
                return;
            }
            StartButton.IsEnabled = false;

            if (!Config.Instance.DisableUpdateCheck)
            {
                // 检测更新状态
                if (!CheckUpdate(out var updateStatus))
                {
                    OnLaunchError("更新检测失败");
                    return;
                }

                // 等待更新
                if (updateStatus)
                {
                    var result = await UpdateClientAsync();
                    if (!result)
                    {
                        OnLaunchError("更新失败.");
                        return;
                    }
                }
            }

            BeginCrashReportDetector();
            KMCCCBugFix();

            // 启动游戏
            var gameManager = new GameManager();
            var lpm = new LaunchProgressManager();

            gameManager.OnGameExit += (handle, i) =>
            {
                async Task Callback()
                {
                    LoginManager.Stop();
                    this.Show();
                    lpm.Close();
                    await CheckCrashAsync();
                }
                Dispatcher.Invoke(Callback);
            };

            gameManager.OnGameLog += (lh, log) => lpm.OnGameLog(log);
            // 游戏状态
            var lh1 = gameManager.Run();
            if (!lh1.Success)
            {
                MessageUploadManager.CrashReport(
                    new UploadData($"游戏启动时异常: {lh1.ErrorMessage} {lh1.Exception?.SerializeException()}"));
                OnLaunchError($"游戏启动时异常: {lh1.ErrorMessage}");
                return;
            }

            if (Config.Instance.LaunchProgress)
                lpm.Begin(lh1.Handle);
            LoginManager.Start();

            // 退出
            StartButton.IsEnabled = true;
            Hide();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            MainTransitioner.SelectedIndex = 0;
            Task.Run(() =>
            {
                Thread.Sleep(500);
                Dispatcher.Invoke(() => Application.Current.Shutdown());
            });
        }

        private void OpenDMinecraft(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer",
                ClientManager.GameStorageDirectory);
        }

        private async void Options(object sender, RoutedEventArgs e)
        {
            await DialogHost.Show(new SettingsDialog(), "RootDialog").ConfigureAwait(false);
        }

        private void Move(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private async void ServerMessage_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var result = await ServerInfoGetter.GetServerInfoAsync();
                ServerMessage.Text = $"服务器在线人数: {result.players.online}";
                ServerMessage.ToolTip = result.players.sample != null ? string.Join("\r\n", result.players.sample.Select(p => p.name)) : "现在没有人..点这里来刷新!";
            }
            catch (Exception e1)
            {
                Trace.WriteLine(e1);
            }
        }

        private /*async*/ void InitEnvironment(object sender, RoutedEventArgs e)
        {
            // await UpdateUpdatePathAsync();
        }

        private void SwitchUser(object sender, RoutedEventArgs e)
        {
            Process.Start(Process.GetCurrentProcess().MainModule.FileName, "Setup");
            this.Flyout(() => Application.Current.Shutdown());
        }

        private async void Theme(object sender, RoutedEventArgs e)
        {
            await DialogHost.Show(new ThemeDialog(), "RootDialog");
        }
    }
}