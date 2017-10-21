using System;
using System.Linq;
using KMCCC.Authentication;
using KMCCC.Launcher;
using OrigindLauncher.Resources.Configs;

namespace OrigindLauncher.Resources.Client
{
    public class GameManager
    {
        public static bool IsRunning { get; private set; }
        public event Action<LaunchHandle, int> OnGameExit;
        public event Action<LaunchHandle, string> OnGameLog;
        public event Action<LaunchResult> OnError;

        public LaunchResult Run()
        {
            var launchercore =
                LauncherCore.Create(new LauncherCoreCreationOption(javaPath: Config.Instance.JavaPath));

            launchercore.GameLog += OnGameLog;
            launchercore.GameExit += (handle, i) =>
            {
                OnGameExit?.Invoke(handle, i);
                IsRunning = false;
            };
            var launchOptions = new LaunchOptions
            {
                Version = launchercore.GetVersion(Definitions.ClientName),
                Authenticator = new OfflineAuthenticator(Config.Instance.PlayerAccount.Username),
                Mode = LaunchMode.BmclMode,
                MaxMemory = Config.Instance.MaxMemory
            };

            var result = launchercore.Launch(launchOptions, x =>
            {
                if (Config.Instance.JavaArguments.Contains("G1GC"))
                    x.CGCEnabled = false;
                x.AdvencedArguments.Add(Config.Instance.JavaArguments);
            });
            IsRunning = true;

            if (!result.Success)
            {
                OnError?.Invoke(result);
                IsRunning = false;
            }


            return result;
            /*
            result.Handle.GetPrivateField<Process>(nameof(Process)).Exited += (sender, args) =>
            {
                IsRunning = false;
                OnGameExit?.Invoke();
            };
            */
        }
        
    }
}