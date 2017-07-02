using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using OrigindLauncher.Resources.Server;
using OrigindLauncher.Resources.Utils;
using OrigindLauncher.UI.Code;
using OrigindLauncher.UI.Dialogs;

namespace OrigindLauncher.UI
{
    /// <summary>
    ///     Interaction logic for ExceptionHandlerWindow.xaml
    /// </summary>
    public partial class ExceptionHandlerWindow : Window
    {
        private readonly string _exceptionString;

        public ExceptionHandlerWindow(Exception exception)
        {
            _exceptionString = exception.SerializeException();
            InitializeComponent();
            ExcetionMessage.Text = exception.Message;
        }

        private void ExceptionHandlerWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ShowMoreButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                MessageBox.Show(_exceptionString);
                 DialogHost.Show(new MessageDialog { Message = { Text = _exceptionString } }, "ErrorRootDialog");

             });
        }

        private void ContinueButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (AutoRestartButton.IsChecked == true)
                    Process.Start(Process.GetCurrentProcess().MainModule.FileName);
                this.Flyout(() =>
                {
                    Application.Current.Shutdown();
                });
            });
            
        }

        private void SubmitButton_OnClick(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                var flag = MessageUploadManager.CrashReport(_exceptionString);
                Dispatcher.Invoke(() => { MessageSnackbar.MessageQueue.Enqueue(flag ? "成功上传错误报告." : "上传失败."); });
            });
        }
    }
}