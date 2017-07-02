using System.Collections.Generic;

namespace GoodTimeStudio.ServerPinger
{
    public class Player
    {
        public string name;
        public string id;

        public Player(string name, string id)
        {
            this.name = name;
            this.id = id;
        }
    }

    public class ServerPlayers
    {
        public int max;
        public int online;
        public List<Player> sample;
    }
}
