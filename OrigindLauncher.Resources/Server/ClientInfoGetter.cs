using System.Net;
using System.Text;
using OrigindLauncher.Resources.Configs;
using OrigindLauncher.Resources.Json;
using OrigindLauncher.Resources.Server.Data;

namespace OrigindLauncher.Resources.Server
{
    public static class ClientInfoGetter
    {
        public static ClientInfo Get()
        {
            var wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            return wc.DownloadString(Config.Instance.UpdatePath).JsonCast<ClientInfo>();
        }
    }
}