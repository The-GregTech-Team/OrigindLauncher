using Newtonsoft.Json;
using System.Collections.Generic;

namespace GoodTimeStudio.ServerPinger
{
    /*
     * http://wiki.vg/Server_List_Ping
     */
    [JsonObject]
    public class ServerStatus
    {

        [JsonIgnore] public string ServerName;
        [JsonIgnore] public string ServerAddress;
        [JsonIgnore] public int ServerPort;
        [JsonIgnore] public PingVersion ServerVersion;

        public ServerVersion version;
        public ServerPlayers players;
        public string description;
        public string favicon;
        public ModInfo modinfo;
        //TO-DO: mod list

    }
}
