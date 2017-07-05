using System.Collections.Generic;

namespace GoodTimeStudio.ServerPinger
{
    public enum PingVersion
    {
        MC_Current,
        MC_16,
        MC_14_15,
        MC_Beta18_13,
        MC_PE
    }

    public class ServerVersion
    {
        public string name;
        public int protocol;
    }


    public class Description
    {
        public string text;
        //TO-DO: extra (动态ping插件)
    }

    public class Mod
    {
        public string modid;
        public string version;
    }

    public class ModInfo
    {
        public List<Mod> modlist;
        public string type;
    }
}