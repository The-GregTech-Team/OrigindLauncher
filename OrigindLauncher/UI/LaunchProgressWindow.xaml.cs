using System;
using System.Diagnostics;
using System.Drawing;
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
using OrigindLauncher.Resources.Screen;
using OrigindLauncher.Resources.Server;
using OrigindLauncher.UI.Code;
using Brush = System.Windows.Media.Brush;
using Image = System.Drawing.Image;

namespace OrigindLauncher.UI
{
    /// <summary>
    ///     Interaction logic for LaunchProgressWindow.xaml
    /// </summary>
    public partial class LaunchProgressWindow : Window
    {
        public static LaunchProgressWindow Instance;

        public readonly DispatcherTimer AnimeTimer = new DispatcherTimer(TimeSpan.FromSeconds(1),
            DispatcherPriority.Normal, UpdateAnime, Dispatcher.CurrentDispatcher);

        private TimeSpan _prevCpuTime = TimeSpan.Zero;

        private DateTime _startTime;

        public LaunchProgressWindow()
        {
            InitializeComponent();
            AnimeTimer.Stop();
            Instance = this;
        }

        public LaunchHandle LaunchHandle { get; set; }
        public Process ProcessHandle { get; set; }

        private static void UpdateAnime(object sender, EventArgs e)
        {
            var ts = DateTime.Now - Instance._startTime;
            var pt = GetProcessTime();

            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                try
                {
                    var da = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(300)));
                    Instance.ProcessHandle.Refresh();
                    Instance.Add1sAnimeText.BeginAnimation(OpacityProperty, da);
                    Instance.GameUseCpu.Text = $"{pt:F2} %";
                    Instance.GameUseMem.Text = $"{(Instance.ProcessHandle.WorkingSet64 / 1024.0 / 1024.0):F2} M";
                    
                }
                catch (Exception exception)
                {
                    Trace.WriteLine(exception);
                }
            });
            Instance.LoadTime.Text = $"{ts.Hours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}";
        }

        private static double GetProcessTime()
        {
            try
            {
                var curTime = Instance.ProcessHandle.TotalProcessorTime;
                var value = (curTime - Instance._prevCpuTime).TotalMilliseconds / 1000.0 * 100;
                Instance._prevCpuTime = curTime;
                return value;

            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return 0;
            }

        }
        /*
        public static Image ScreenCapture()
        {
            try
            {
                return new ScreenCapture().CaptureWindow(Instance.ProcessHandle.MainWindowHandle);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return new Bitmap(10, 10);
            }
        }
        */
        public void Begin()
        {
            AnimeTimer.Start();
            GameMem.Text = Config.Instance.MaxMemory + "M";
            _startTime = DateTime.Now;
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
            StatusProgressBar.AnimeToValueAsPercent(1);
        }

        public void Process(string text, double progress)
        {
            LoadingText.Text = text;
            StatusProgressBar.IsIndeterminate = false;
            StatusProgressBar.AnimeToValueAsPercent(progress);
        }

        private void LaunchProgressWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
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