using GoodTimeStudio.ServerPinger;

namespace OrigindLauncher
{
    public static class ServerInfoGetter
    {
        public static ServerStatus GetServerInfo()
        {
            var spinger = new ServerPinger("Origind", "origind.320.io", 25565, PingVersion.MC_Current);
            var result = spinger.GetStatus();
            result.Wait();
            return result.Result;
        }
    }
}