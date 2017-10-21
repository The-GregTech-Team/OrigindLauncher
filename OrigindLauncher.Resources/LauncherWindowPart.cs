using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MaterialDesignThemes.Wpf;
using OrigindLauncher.Resources;
using OrigindLauncher.Resources.Client;
using OrigindLauncher.Resources.Configs;
using OrigindLauncher.Resources.FileSystem;
using OrigindLauncher.Resources.Server;
using OrigindLauncher.UI.Dialogs;

namespace OrigindLauncher
{
    public partial class LauncherWindow
    {
        private static void EnsureUpdatePathExists()
        {
            if (string.IsNullOrWhiteSpace(Config.Instance.UpdatePath) ||
                CheckUrlRegex.IsMatch(Config.Instance.UpdatePath))
                Config.Instance.UpdatePath = $"{Definitions.OrigindServerUrl}/{Definitions.Rest.ClientJson}";
        }

        private void OnLaunchError(string reason)
        {
            MainSnackbar.MessageQueue.Enqueue(reason);
            StartButton.IsEnabled = true;
        }

        private static void KMCCCBugFix()
        {
            try
            {
                var dict = ClientManager.GetGameStorageDirectory("$natives");
                if (Directory.Exists(dict))
                {
                    Directory.Delete(dict, recursive: true);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        private async Task CheckCrashAsync()
        {
            var after = GetReportFiles().Except(Before).ToList();
            if (after.Count != 0)
                await Dispatcher.Invoke(async () =>
                {
                    var chooseDialog = new ChooseDialog("要上传崩溃报告喵?", "Origind Launcher 检测到了一个 Minecraft 崩溃报告.",
                        "上传");
                    await DialogHost.Show(chooseDialog, "RootDialog");
                    if (chooseDialog.Result)
                        MessageUploadManager.CrashReport(new UploadData(
                            $"游戏运行时异常: {File.ReadAllText(after.FirstOrDefault())}"));
                });
        }

        private bool CheckUpdate(out bool updateStatus)
        {
            try
            {
                updateStatus = ClientManager.CheckUpdate();
                return true;
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);
                updateStatus = false;
                return false;
            }
        }

        private static void BeginCrashReportDetector()
        {
            var files = GetReportFiles();
            Before = files;
        }

        private static string[] GetReportFiles()
        {
            var path = ClientManager.GetGameStorageDirectory("crash-reports");
            DirectoryHelper.EnsureDirectoryExists(path);
            return Directory.GetFiles(path);
        }

        private static async Task UpdateUpdatePathAsync() // 兼容其它的服务器
        {
            while (!CheckUrlRegex.IsMatch(Config.Instance.UpdatePath))
            {
                var input = new InputDialog { Title = { Text = "输入客户端更新地址." } };
                await DialogHost.Show(input, "RootDialog");
                var text = input.InputBox.Text;
                if (CheckUrlRegex.IsMatch(text))
                    Config.Instance.UpdatePath = text;
                Config.Save();
            }
        }

        private async Task<bool> UpdateClientAsync()
        {
            MainSnackbar.MessageQueue.Enqueue("正在更新客户端");

            return await ClientManager.UpdateAsync();
        }

        private void VerifyOnBackground(object sender, RoutedEventArgs e)
        {
            //MainSnackbar.MessageQueue.Enqueue("抱歉, 我们暂时禁用了这个功能.");
            LoginManager.Start();
        }
    }
}