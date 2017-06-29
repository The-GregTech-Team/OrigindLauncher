using System;
using System.Windows;
using System.Windows.Threading;
using OrigindLauncher.UI;

namespace OrigindLauncher
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            new ExceptionHandlerWindow(e.Exception).ShowDialog();
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            new ExceptionHandlerWindow((Exception) e.ExceptionObject).ShowDialog();
        }
    }
}