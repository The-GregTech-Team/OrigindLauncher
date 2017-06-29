using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
using OrigindLauncher.Resources;
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
            if (Directory.Exists(ClientManager.GameStorageDirectory))
            {
                var choose = new ChooseDialog("要删除当前客户端吗?", "Origind Launcher检测到当前存在一个客户端.\r\n" +
                                                            "如果你想继续, 这个客户端会被删除.", "删除");
                await DialogHost.Show(choose, "SetupWindowDialogHost");

                if (choose.Result)
                {
                    Snackbar.MessageQueue.Enqueue("正在删除客户端..可能需要一点时间.");
                    await Task.Run(() =>
                    {
                        Directory.Delete(ClientManager.GameStorageDirectory, true);
                        if (File.Exists(Definitions.ClientJsonPath)) File.Delete(Definitions.ClientJsonPath);
                    });
                }
                else
                {
                    Window.GetWindow(this).FlyoutAndClose(() => { Application.Current.Shutdown(); });
                }
            }

            Transitioner.MoveNextCommand.Execute(this, this);
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            Step1.IsLogin = true;
            Transitioner.MoveNextCommand.Execute(this, this);
        }
    }
}