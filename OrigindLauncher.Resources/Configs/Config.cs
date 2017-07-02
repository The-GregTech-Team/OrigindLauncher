using System.IO;
using System.Linq;
using OrigindLauncher.Resources.Json;
using OrigindLauncher.Resources.Server;
using OrigindLauncher.Resources.Utils;

namespace OrigindLauncher.Resources.Configs
{
    public class Config
    {
        public static Config Instance;


        static Config()
        {
            Instance = File.Exists(Definitions.ConfigJsonPath)
                ? File.ReadAllText(Definitions.ConfigJsonPath).JsonCast<Config>()
                : new Config();
        }

        public string JavaPath { get; set; } = JavaHelper.FindJava().FirstOrDefault();
        public string JavaArguments { get; set; } = "-XX:+AggressiveOpts -XX:+UseCompressedOops";
        public int MaxMemory { get; set; } = 2048;
        public Account PlayerAccount { get; set; } = new Account(null, null, null);
        public int LauncherVersion { get; } = 
            97
            ;

        public bool DisableHardwareSpeedup { get; set; } = false;

        public static void Save()
        {
            File.WriteAllText(Definitions.ConfigJsonPath, Instance.ToJsonString());
        }
    }
}
