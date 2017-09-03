using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using OrigindLauncher.Resources.Configs;
using OrigindLauncher.Resources.FileSystem;
using OrigindLauncher.Resources.Server;
using OrigindLauncher.Resources.String;
using OrigindLauncher.Resources.Utils;
using OrigindLauncher.UI.Code;
using OrigindLauncher.UI.Dialogs;

namespace OrigindLauncher.UI.StartupSteps
{
    /// <summary>
    ///     Interaction logic for Step1.xaml
    /// </summary>
    public partial class Step1 : UserControl
    {
        public Step1()
        {
            InitializeComponent();
        }

        public static bool IsLogin { get; set; }

        private async void Next(object sender, RoutedEventArgs e)
        {
            var username = Username.Text;
            if (string.IsNullOrWhiteSpace(username) || !new Regex(@"^[a-zA-Z0-9_]+$").Match(username).Success)
            {
                Snackbar.MessageQueue.Enqueue("用户名只能包含字母,数字,以及下划线.");
                return;
            }

            if (IsLogin)
            {
                var account = new Account(username, SHA512Computer.Compute(Password.Password), "注册");
                var status = await Task.Run(() => account.Login());
                if (status != LoginStatus.Successful)
                {
                    Snackbar.MessageQueue.Enqueue("密码不正确或服务器错误.");
                    return;
                }
                Config.Instance.PlayerAccount = account;
                try
                {
                    await Task.Run(() => Config.Instance.DisableHardwareSpeedup = RenderInfoGetter.IsIntelVideoCard);
                }
                catch (Exception exception)
                {
                    Trace.WriteLine(exception);
                }
            }
            else
            {
                var status = await Task.Run(() => AccountManager.UserExists(username));

                if (status != LoginStatus.NotFound)
                {
                    Snackbar.MessageQueue.Enqueue("用户已存在或服务器错误.");
                    return;
                }

                if (username.Length < 3)
                {
                    Snackbar.MessageQueue.Enqueue("用户名太短啦!");
                    return;
                }

                if (Password.Password.Length < 5)
                {
                    Snackbar.MessageQueue.Enqueue("密码太短啦!");
                    return;
                }

                var account = new Account(username, SHA512Computer.Compute(Password.Password), "注册");
                var result = await Task.Run(() => account.Register());
                if (result != RegisterStatus.Successful)
                {
                    Snackbar.MessageQueue.Enqueue("注册失败 请重试?");
                    return;
                }
                Config.Instance.PlayerAccount = account;
                try
                {
                    await Task.Run(() => Config.Instance.DisableHardwareSpeedup = RenderInfoGetter.IsIntelVideoCard);
                }
                catch (Exception exception)
                {
                    Trace.WriteLine(exception);
                }
            }
            if (!DirectoryHelper.IsCurrentDirectoryWritable)
                await Dispatcher.Invoke(async () =>
                {
                    var chooseDialog = new ChooseDialog("要让这个文件夹有写入权限吗?", "Origind Launcher 可能没有文件夹的写入权限.", "添加写入权限");
                    await DialogHost.Show(chooseDialog, "SetupWindowDialogHost");
                    if (chooseDialog.Result) DirectoryHelper.AddCurrentDirectoryWritePermission();
                });

            Config.Save();
            Dispatcher.Invoke(() => Window.GetWindow(this).FlyoutAndClose(() =>
            {
                //if (Directory.Exists(ClientManager.GameStorageDirectory))
                //{
                //    ClientManager.CurrentInfo = ClientManager.MakeClientInfo();
                //    ClientManager.Save();
                //}

                Process.Start(Process.GetCurrentProcess().MainModule.FileName);
                Application.Current.Shutdown();
            }));
        }
    }
}