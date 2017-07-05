using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using KMCCC.Launcher;
using OrigindLauncher.Resources.Configs;
using OrigindLauncher.UI.Code;

namespace OrigindLauncher.UI
{
    /// <summary>
    /// Interaction logic for LaunchProgressWindow.xaml
    /// </summary>
    public partial class LaunchProgressWindow : Window
    {
        public LaunchHandle LaunchHandle { get; set; }
        private static LaunchProgressWindow _instance;

        private readonly DispatcherTimer _animeTimer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, UpdateAnime, Dispatcher.CurrentDispatcher);
        public DateTime StartTime;

        private static void UpdateAnime(object sender, EventArgs e)
        {
            var ts = DateTime.Now - _instance.StartTime;
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                var da = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(300)));

                _instance.Add1sAnimeText.BeginAnimation(OpacityProperty, da);
            });
            _instance.LoadTime.Text = $"{ts.Hours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}";
        }

        public void Begin()
        {
            _animeTimer.Start();
            GameMem.Text = Config.Instance.MaxMemory + "M";
            StartTime = DateTime.Now;

        }

        public LaunchProgressWindow()
        {
            InitializeComponent();
            _animeTimer.Stop();
            _instance = this;
        }

        public void AddLog(string log)
        {
            if (LogList.Items.Count > 200)
            {
                LogList.Items.RemoveAt(0);
            }
            LogList.Items.Add(log);
            LogList.SelectedIndex = LogList.Items.Count - 1;
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
        }

        private void CloseGame(object sender, RoutedEventArgs e)
        {
            this.Flyout(Hide);
            LaunchHandle?.Kill();
        }

        private void Min(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }



    }
}
