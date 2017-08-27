using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using OrigindLauncher.Resources;
using OrigindLauncher.Resources.Client;
using OrigindLauncher.Resources.Configs;
using OrigindLauncher.Resources.FileSystem;
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
            try
            {
                InitForTheme();

                WelcomeMessage.Text += " " + Config.Instance.PlayerAccount.Username;
                TitleTextBlock.Text += " " + Config.LauncherVersion +
                                       (Config.Admins.Any(u => u == Config.Instance.PlayerAccount.Username)
                                           ? " Admin"
                                           : string.Empty);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }
            
            try
            {
                var result1 = ServerInfoGetter.GetServerInfoAsync();
                result1.Wait(1000); // 避免由服务器错误引起的无限等待
                if (result1.IsCompleted)
                {
                    var result = result1.Result;
                    ServerMessage.Text += " " + result.players.online;
                    ServerMessage.ToolTip = string.Join("\r\n", result.players.sample.Select(p => p.name));
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static string[] Before { get; set; }

        public void InitForTheme()
        {
            var num = Config.Instance.ThemeConfig.IsDark ? "2" : "1";
            var imageSource = ImageSourceGetter.GetImageSource($"/Images/Background{num}.png");
            BgCache.Source = imageSource; // Fix a bug
            Bg.ImageSource = imageSource;
            Bg2.Source = ImageSourceGetter.GetImageSource($"/Images/flat{num}.png");
        }

        private async void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            EnsureUpdatePathExists();
            //await UpdateUpdatePathAsync(); 其他服务器的兼容

#if !DEBUG
            
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
#endif

            BeginCrashReportDetector();
            KMCCCBugFix();

            // 启动游戏
            var gm = new GameManager();
            //gm.OnError += result => Dispatcher.Invoke(() => MainSnackbar.MessageQueue.Enqueue(result.Exception));
            var lpm = new LaunchProgressManager();

            gm.OnGameExit += (handle, i) => Dispatcher.Invoke(async () =>
            {
                LoginManager.Stop();
                this.Show();
                lpm.Close();
                await CheckCrashAsync();
            });

            gm.OnGameLog += (lh, log) => lpm.OnGameLog(log);

            // 游戏状态
            var lh1 = gm.Run();

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
            this.FlyoutAndClose(() => Application.Current.Shutdown());
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
                ServerMessage.Text = "服务器在线人数: " + result.players.online;
                ServerMessage.ToolTip = string.Join("\r\n", result.players.sample.Select(p => p.name));
            }
            catch (Exception e1)
            {
                Console.WriteLine(e1);
            }
        }

        private /*async*/ void InitEnvironment(object sender, RoutedEventArgs e)
        {
            // await UpdateUpdatePathAsync();
        }

        private void SwitchUser(object sender, RoutedEventArgs e)
        {
            Process.Start(Process.GetCurrentProcess().MainModule.FileName, "Setup");
            this.Flyout(() => Environment.Exit(0));
        }

        private async void Theme(object sender, RoutedEventArgs e)
        {
            await DialogHost.Show(new ThemeDialog(), "RootDialog").ConfigureAwait(false);
        }
    }
}