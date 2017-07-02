using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrigindLauncher.UI;

namespace OrigindLauncher
{
    public static class AsyncErrorHandler
    {
        public static void HandleException(Exception exception)
        {
            new ExceptionHandlerWindow(exception).Show();

        }
    }
}