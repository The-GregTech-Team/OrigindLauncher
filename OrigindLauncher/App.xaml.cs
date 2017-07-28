using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using OrigindLauncher.Resources.Utils;
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

            File.AppendAllText("crashreport.txt", e.Exception.SerializeException());

            Dispatcher.Invoke(() => { new ExceptionHandlerWindow(e.Exception).ShowDialog(); });
            e.Handled = true;

        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

        }

        private void TaskSchedulerOnUnobservedTaskException(object sender,
            UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            File.AppendAllText("crashreport.txt", unobservedTaskExceptionEventArgs.Exception.SerializeException());
            //File.AppendAllText("crashreport.txt", unobservedTaskExceptionEventArgs.Exception.InnerExceptions.FirstOrDefault().SerializeException());

            Dispatcher.Invoke(() =>
            {
                new ExceptionHandlerWindow(unobservedTaskExceptionEventArgs.Exception.InnerException).ShowDialog();
            });
            unobservedTaskExceptionEventArgs.SetObserved();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            File.WriteAllText("crashreport.txt", ((Exception) e.ExceptionObject).SerializeException());

            Dispatcher.Invoke(() => { new ExceptionHandlerWindow((Exception) e.ExceptionObject).ShowDialog(); });
        }
    }
}