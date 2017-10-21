using System;
using System.Diagnostics;
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
            Trace.WriteLine(e.Exception);
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
            var serializedException = unobservedTaskExceptionEventArgs.Exception.SerializeException();
            File.AppendAllText("crashreport.txt", serializedException);
            Trace.WriteLine(serializedException);

            Dispatcher.Invoke(() =>
            {
                new ExceptionHandlerWindow(unobservedTaskExceptionEventArgs.Exception.InnerException).ShowDialog();
            });
            unobservedTaskExceptionEventArgs.SetObserved();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var serializedException = ((Exception)e.ExceptionObject).SerializeException();
            File.AppendAllText("crashreport.txt", serializedException);
            Trace.WriteLine(serializedException);
            Dispatcher.Invoke(() => { new ExceptionHandlerWindow((Exception) e.ExceptionObject).ShowDialog(); });
        }
    }
}