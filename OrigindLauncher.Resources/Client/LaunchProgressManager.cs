using System;
using System.Diagnostics;
using System.Threading.Tasks;
using KMCCC.Launcher;
using NAudio.Wave;
using OrigindLauncher.Resources.Configs;
using OrigindLauncher.Resources.Sound;
using OrigindLauncher.Resources.Utils;
using OrigindLauncher.UI;
using OrigindLauncher.UI.Code;

namespace OrigindLauncher.Resources.Client
{
    public class LaunchProgressManager
    {
        private readonly LaunchProgressWindow _launchProgressWindow = new LaunchProgressWindow();

        public void OnGameLog(string log)
        {
            Task.Run(() =>
            {
                if (log.Contains("LLHMessage")) // This for OrigindHelper
                {
                    var message = log.Split('%')[1];
                    var statusp = message.Split('&');
                    var statusw = statusp[0];
                    if (!double.TryParse(statusp[1], out var progress))
                    {
                        progress = 0.5;
                    }
                    string modname = string.Empty, progressname;
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
                    var progressText = $"{progressname} {modname} 完成";
                    _launchProgressWindow.Dispatcher.Invoke(() => _launchProgressWindow.Process(progressText, progress));
                }
                else if (log.EndsWith("GuiMainMenu Loaded"))
                {
                    _launchProgressWindow.Dispatcher.Invoke(() => _launchProgressWindow.Done());

                    if (Config.Instance.PlayGameLoadedSound)
                        Task.Run(async () =>
                        {
                            await Task.Delay(TimeSpan.FromSeconds(10));
                            var sound = EmbedResourceReader.GetStream("OrigindLauncher.Sounds.InterruptOne.ogg");
                            SoundPlayer.PlaySound(sound);
                        });
                }
                else
                {
                    _launchProgressWindow.Dispatcher.Invoke(() => _launchProgressWindow.AddLog(log));
                }
            });

        }

        private static string Translate(string progressName)
        {
            switch (progressName)
            {
                case @"construction":
                    return "构建";
                case @"pre_initialization":
                    return "第一次初始化";
                case @"initialization":
                    return "第二次初始化";
                case @"post_initialization":
                    return "第三次初始化";
                case @"completed":
                    return "加载";
                case @"reloading_resource_packs":
                    return "加载资源包";
                default:
                    return progressName;
            }
        }

        public void Begin(LaunchHandle lh)
        {
            _launchProgressWindow.LaunchHandle = lh;
            try
            {
                var processHandle = lh.GetPrivateField<Process>("Process");
                _launchProgressWindow.ProcessHandle = processHandle;
                if (Config.Instance.UseBoost)
                {
                    processHandle.ProcessorAffinity = (IntPtr)((1 << Environment.ProcessorCount) - 2);
                    processHandle.PriorityClass = ProcessPriorityClass.RealTime;
                }

                //processHandle.PriorityBoostEnabled = true;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
            _launchProgressWindow.Begin();
            _launchProgressWindow.Show();
        }

        public void Close()
        {
            _launchProgressWindow.FlyoutAndClose();
            _launchProgressWindow.AnimeTimer.Stop();
        }
    }
}