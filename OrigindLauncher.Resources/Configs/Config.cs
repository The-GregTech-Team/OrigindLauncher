using System.IO;
using System.Linq;
using OrigindLauncher.Resources.Json;
using OrigindLauncher.Resources.Server;
using OrigindLauncher.Resources.Utils;

namespace OrigindLauncher.Resources.Configs
{
    public class Config
    {
        public static readonly Config Instance;

        static Config()
        {
            Instance = File.Exists(Definitions.ConfigJsonPath)
                ? File.ReadAllText(Definitions.ConfigJsonPath).JsonCast<Config>()
                : new Config();
        }

        public string JavaPath { get; set; } = JavaHelper.FindJava().FirstOrDefault();

        public string JavaArguments { get; set; } =
            "-XX:+AggressiveOpts -XX:+UseCompressedOops -XX:MetaspaceSize=256m -XX:MaxMetaspaceSize=1024m -XX:MaxGCPauseMillis=40";

        public int MaxMemory { get; set; } = 2048;
        public Account PlayerAccount { get; set; } = new Account(null, null, null);

        public static int LauncherVersion { get; } =
            169;

        public bool DisableHardwareSpeedup { get; set; } = false;
        public bool LaunchProgress { get; set; } = true;

        public static void Save()
        {
            File.WriteAllText(Definitions.ConfigJsonPath, Instance.ToJsonString());
        }
    }
}