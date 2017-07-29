using System.Collections.Generic;

namespace OrigindLauncher.Resources.Server.Data
{
    public class ClientInfo
    {
        public string BaseUrl;
        public List<FileEntry> Files = new List<FileEntry>();
        public int Version;
    }
}