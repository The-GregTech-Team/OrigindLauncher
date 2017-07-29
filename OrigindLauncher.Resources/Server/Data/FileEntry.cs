namespace OrigindLauncher.Resources.Server.Data
{
    public class FileEntry
    {
        public readonly string Hash; // SHA128

        public readonly string Path;

        public FileEntry(string path, string hash)
        {
            Path = path;
            Hash = hash;
        }

        public override bool Equals(object obj)
        {
            return ((FileEntry) obj)?.Path == Path;
        }

        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }
    }
}