using System.IO;

namespace OrigindLauncher.Resources.FileSystem
{
    public static class DirectoryHelper
    {
        public static void EnsureDirectoryExists(string directoryName)
        {
            if (!Directory.Exists(directoryName)) Directory.CreateDirectory(directoryName);
        }
    }
}