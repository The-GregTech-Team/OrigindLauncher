using System;

namespace OrigindLauncher.Resources.Utils
{
    public static class ExceptionSerializer
    {
        public static string SerializeException(this Exception ex)
        {
            return $"{ex.Message}{ex.Source}{ex.StackTrace}";
        }
    }
}