using System;
using System.Diagnostics;
using OrigindLauncher.Resources.Utils;
using OrigindLauncher.UI;

namespace OrigindLauncher
{
    public static class AsyncErrorHandler
    {
        public static void HandleException(Exception exception)
        {
            new ExceptionHandlerWindow(exception).Show();
            Trace.WriteLine(exception.SerializeException());
        }
    }
}