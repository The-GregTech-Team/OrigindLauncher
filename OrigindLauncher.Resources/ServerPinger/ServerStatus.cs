using Newtonsoft.Json;

namespace GoodTimeStudio.ServerPinger
{
    /*
     * http://wiki.vg/Server_List_Ping
     */
    [JsonObject]
    public class ServerStatus
    {
        public string description;
        public string favicon;
        public ModInfo modinfo;
        public ServerPlayers players;
        [JsonIgnore] public string ServerAddress;

        [JsonIgnore] public string ServerName;
        [JsonIgnore] public int ServerPort;
        [JsonIgnore] public PingVersion ServerVersion;

        public ServerVersion version;
        //TO-DO: mod list
    }
}