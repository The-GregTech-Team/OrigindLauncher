namespace OrigindLauncher.Resources.Server.Data
{
    public class FileEntry
    {
        public string Hash; // SHA128

        public string Path;

        public FileEntry(string path, string hash)
        {
            Path = path;
            Hash = hash;
        }
    }
}