using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
using OrigindLauncher.Resources.Client;
using OrigindLauncher.UI.Code;
using OrigindLauncher.UI.Dialogs;

namespace OrigindLauncher.UI.StartupSteps
{
    /// <summary>
    ///     Interaction logic for Step0.xaml
    /// </summary>
    public partial class Step0 : UserControl
    {
        public Step0()
        {
            InitializeComponent();
        }

        private async void Register(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(ClientManager.GameStorageDirectory) && !File.Exists("client.json"))
            {
                var messageDialog = new MessageDialog
                {
                    Message =
                    {
                        Text = "Origind Launcher 检测到当前存在一个客户端.\r\n" +
                               "如果你想继续, 这个客户端会的一些内容, 比如截图, 小地图可能会被删除, 请先备份."
                    }
                };

                await DialogHost.Show(messageDialog, "SetupWindowDialogHost");
            }

            Transitioner.MoveNextCommand.Execute(this, this);
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            Step1.IsLogin = true;
            Transitioner.MoveNextCommand.Execute(this, this);
        }

        private void CloseIt(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Flyout(() => Environment.Exit(0));
        }
    }
}