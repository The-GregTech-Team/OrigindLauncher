using System;

namespace OrigindLauncher.Resources
{
    internal static class Definitions
    {
#if DEBUG
        public const string OrigindServerUrl = "http://127.0.0.1:5000";
#else
        public const string OrigindServerUrl = "http://origind.320.io";
#endif
        public const string ClientJsonPath = "client.json";
        public const string ConfigJsonPath = "config.json";
        public const string ClientName = "Origind";

        public static class Rest
        {
            public const string Register = "api/Accounts/Register";
            public const string Login = "api/Accounts/Login";
            [Obsolete]
            public const string LoginStatus = "api/Accounts/LoginStatus";
            public const string UserExists = "api/Accounts/UserExists";

            public const string CrashReport = "api/Upload/CrashReport";
            public const string Suggests = "api/Upload/Suggests";

            public const string ClientJson = "api/Client/ClientJson";

            public const string LauncherVersion = "api/Launcher/Version";
            public const string LauncherDownload = "api/Launcher/Download";

            public const string PullLoginVerify = "api/Player/PullLoginVerify";
            public const string LoginVerify = "api/Player/LoginVerify";
        }
    }
}