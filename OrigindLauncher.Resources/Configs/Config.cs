using System.IO;
using System.Linq;
using KMCCC.Launcher;
using OrigindLauncher.Resources.Json;
using OrigindLauncher.Resources.Server;
using OrigindLauncher.Resources.UI;
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

        public ThemeConfig ThemeConfig { get; set; } = new ThemeConfig();

        public string UpdatePath { get; set; } = $"{Definitions.OrigindServerUrl}/{Definitions.Rest.ClientJson}";

        public bool DisableHardwareSpeedup { get; set; } = false;

        public bool PlayGameLoadedSound { get; set; } = true;

        public bool EnableDebug { get; set; } = false;

        public static int LauncherVersion { get; } =
            343
            ;

        public static string[] Admins { get; } = {"Cyl18", "EMROF"}; // Just For Fun

        public static void Save()
        {
            File.WriteAllText(Definitions.ConfigJsonPath, Instance.ToJsonString());
        }

        #region GameLauncher

        public string JavaPath { get; set; } = JavaFinder.FindJava().FirstOrDefault();

        public string JavaArguments { get; set; } =
            "-XX:+AggressiveOpts -XX:+UseCompressedOops -XX:MetaspaceSize=256m -XX:MaxMetaspaceSize=1024m";

        public int MaxMemory { get; set; } = 2048;

        public Account PlayerAccount { get; set; } = new Account(null, null, null);

        public bool LaunchProgress { get; set; } = true;
        public bool UseBoost { get; set; } = false;
        public bool UseAdmin { get; set; } = false;

        #endregion
    }
}
