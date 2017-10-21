using System;

namespace OrigindLauncher.Resources.Utils
{
    public static class ExceptionSerializer
    {
        public static string SerializeException(this Exception ex) => $"{ex}";
    }
}