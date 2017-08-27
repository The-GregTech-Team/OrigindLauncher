using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using OrigindLauncher.Resources.Server;
using OrigindLauncher.UI.Code;
using OrigindLauncher.UI.Dialogs;

namespace OrigindLauncher.UI
{
    /// <summary>
    /// Interaction logic for LoginVerifyWindow.xaml
    /// </summary>
    public partial class LoginVerifyWindow : Window
    {
        public LoginVerifyWindow()
        {
            InitializeComponent();
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                Dispatcher.Invoke(() => this.FlyoutAndClose());
            });
        }

        private void Yes(object sender, RoutedEventArgs e)
        {
            LoginManager.LoginVerify(true);
            this.FlyoutAndClose();
        }

        private async void No(object sender, RoutedEventArgs e)
        {
            var chooseDialog = new ChooseDialog("你要取消这次登录吗？", "这将会封禁被登录者的IP.", "是的, 登录账号的人不是我.", "取消");
            await DialogHost.Show(chooseDialog, "LoginVerify");
            if (chooseDialog.Result)
            {
                LoginManager.LoginVerify(false);
                this.FlyoutAndClose();
            }
        }
    }
}
