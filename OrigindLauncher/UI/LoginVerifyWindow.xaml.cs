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
                await Task.Delay(TimeSpan.FromSeconds(1));
                Dispatcher.Invoke(() =>
                {
                    MainTransitioner.SelectedIndex++;
                });
                await Task.Delay(TimeSpan.FromSeconds(1));
                var verify = LoginManager.LoginVerify(true);
                Dispatcher.Invoke(() =>
                {
                    if (verify)
                    {
                        MainTextBlock.Text = "验证完成！ 祝你游戏愉快";
                        AnimeBar.FadeOut();
                        SuccessIcon.FadeIn();
                    }
                    else
                    {
                        MainTextBlock.Text = "很抱歉, 验证失败";
                        AnimeBar.FadeOut();
                        ErrorIcon.FadeIn();
                    }
                });
                await Task.Delay(TimeSpan.FromSeconds(3));

                Dispatcher.Invoke(() =>
                {
                    MainTransitioner.SelectedIndex++;
                    
                });
                await Task.Delay(TimeSpan.FromSeconds(3));
                Dispatcher.Invoke(this.Close);
            });
        }

    }
}
