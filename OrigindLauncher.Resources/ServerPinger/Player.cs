using System.Collections.Generic;

namespace GoodTimeStudio.ServerPinger
{
    public class Player
    {
        public string id;
        public string name;

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