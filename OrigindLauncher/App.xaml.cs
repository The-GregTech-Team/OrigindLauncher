using System;
using System.Threading;
using System.Threading.Tasks;
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
            this.Dispatcher.Invoke(() =>
            {
                new ExceptionHandlerWindow(e.Exception).Show();

            });
            e.Handled = true;
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            this.Dispatcher.Invoke(() => { 
            new ExceptionHandlerWindow(unobservedTaskExceptionEventArgs.Exception.InnerException).Show();
            });
            unobservedTaskExceptionEventArgs.SetObserved();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

            this.Dispatcher.Invoke(() => {
            new ExceptionHandlerWindow((Exception)e.ExceptionObject).Show();
});

        }
    }
}