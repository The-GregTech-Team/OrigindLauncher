using KMCCC.Launcher;
using OrigindLauncher.UI;

namespace OrigindLauncher.Resources.Client
{
    public class LaunchProgressManager
    {
        private readonly LaunchProgressWindow _launchProgressWindow = new LaunchProgressWindow();

        public void OnGameLog(string log)
        {
            if (log.Contains("LLHMessage"))
            {
                var message = log.Split('%')[1];
                var statusp = message.Split('&');
                var statusw = statusp[0];
                var progress = double.Parse(statusp[1]);
                string modname = string.Empty, progressname = string.Empty;
                if (statusw == "reloading_resource_packs")
                {
                    progressname = statusw;
                }
                else
                {
                    var temp = statusw.Split('*');
                    progressname = temp[0];
                    modname = temp[1];
                }

                progressname = Translate(progressname);
                var progresstext = $"{progressname} {modname}";
                _launchProgressWindow.Dispatcher.Invoke(() => _launchProgressWindow.Process(progresstext, progress));
            }
            else if (log.EndsWith("GuiMainMenu Loaded"))
            {
                _launchProgressWindow.Dispatcher.Invoke(() => _launchProgressWindow.Done());
            }
            else
            {
                _launchProgressWindow.Dispatcher.Invoke(() => _launchProgressWindow.AddLog(log));
            }
        }

        private static string Translate(string progressname)
        {
            switch (progressname)
            {
                case "construction":
                    return "构建";
                case "pre_initialization":
                    return "预初始化";
                case "initialization":
                    return "初始化";
                case "post_initialization":
                    return "后初始化";
                case "completed":
                    return "完成";
                case "reloading_resource_packs":
                    return "加载资源包";
                default:
                    return progressname;
            }
        }

        public void Begin(LaunchHandle lh)
        {
            _launchProgressWindow.LaunchHandle = lh;
            _launchProgressWindow.Begin();
            _launchProgressWindow.Show();
        }
    }
}