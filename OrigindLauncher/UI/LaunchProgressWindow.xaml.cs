using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using KMCCC.Launcher;
using OrigindLauncher.Resources.Configs;
using OrigindLauncher.Resources.Server;
using OrigindLauncher.UI.Code;

namespace OrigindLauncher.UI
{
    /// <summary>
    ///     Interaction logic for LaunchProgressWindow.xaml
    /// </summary>
    public partial class LaunchProgressWindow : Window
    {
        private static LaunchProgressWindow _instance;

        public readonly DispatcherTimer AnimeTimer = new DispatcherTimer(TimeSpan.FromSeconds(1),
            DispatcherPriority.Normal, UpdateAnime, Dispatcher.CurrentDispatcher);

        public TimeSpan PrevCpuTime = TimeSpan.Zero;

        public DateTime StartTime;

        public LaunchProgressWindow()
        {
            InitializeComponent();
            AnimeTimer.Stop();
            _instance = this;
        }

        public LaunchHandle LaunchHandle { get; set; }
        public Process ProcessHandle { get; set; }

        private static void UpdateAnime(object sender, EventArgs e)
        {
            var ts = DateTime.Now - _instance.StartTime;
            var pt = GetProcessTime();

            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                try
                {
                    var da = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(300)));
                    _instance.ProcessHandle.Refresh();
                    _instance.Add1sAnimeText.BeginAnimation(OpacityProperty, da);
                    _instance.GameUseCpu.Text = $"{pt:F2} %";
                    _instance.GameUseMem.Text = $"{_instance.ProcessHandle.WorkingSet64 / 1024.0 / 1024.0:F2} M";
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            });
            _instance.LoadTime.Text = $"{ts.Hours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}";
        }

        private static double GetProcessTime()
        {
            try
            {
                var curTime = _instance.ProcessHandle.TotalProcessorTime;
                var value = (curTime - _instance.PrevCpuTime).TotalMilliseconds / 1000.0 / Environment.ProcessorCount * 100;
                _instance.PrevCpuTime = curTime;
                return value;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
            
        }

        public void Begin()
        {
            AnimeTimer.Start();
            GameMem.Text = Config.Instance.MaxMemory + "M";
            StartTime = DateTime.Now;
        }

        public void AddLog(string log)
        {
            if (LogList.Items.Count > 200)
                LogList.Items.RemoveAt(0);

            LogList.Items.Add(new ListBoxItem { Foreground = TranslateToBrush(log), Content = log });
            LogList.SelectedIndex = LogList.Items.Count - 1;
        }


        private Brush TranslateToBrush(string a)
        {
            var sub = a.Length > 50 ? a.Substring(0, 50) : a;
            if (sub.Contains("INFO")) return new SolidColorBrush(Colors.Teal);
            if (sub.Contains("DEBUG")) return new SolidColorBrush(Colors.DeepSkyBlue);
            if (sub.Contains("WARN")) return new SolidColorBrush(Colors.GreenYellow);
            if (sub.Contains("ERROR")) return new SolidColorBrush(Colors.Red);
            return new SolidColorBrush(Colors.Teal);
        }
        public void Done()
        {
            LoadingText.Text = "加载完成.";
            StatusProgressBar.Value = 1;
        }

        public void Process(string text, double progress)
        {
            LoadingText.Text = text;
            StatusProgressBar.IsIndeterminate = false;
            StatusProgressBar.AnimeToValueAsPercent(progress);
        }

        private void LaunchProgressWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Flyout(Hide);
            AnimeTimer.Stop();
        }

        private void CloseGame(object sender, RoutedEventArgs e)
        {
            this.Flyout(Hide);
            AnimeTimer.Stop();
            LaunchHandle?.Kill();
        }

        private void Min(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Ref(object sender, RoutedEventArgs e)
        {
            Task.Run(() => Config.Instance.PlayerAccount.UpdateLoginStatus());
        }
    }
}