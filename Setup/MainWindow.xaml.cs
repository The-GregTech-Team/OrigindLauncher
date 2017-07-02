using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IWshRuntimeLibrary;
using Path = System.IO.Path;

namespace Setup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ClientLocation.Text =
                $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Origind\";
            
        }

        private async void Next(object sender, RoutedEventArgs e)
        {
            try
            {
                Buttona.IsEnabled = false;
                if (!ClientLocation.Text.EndsWith(@"\"))
                {
                    ClientLocation.Text += @"\";
                }
                var clientLocationText = Path.GetDirectoryName(ClientLocation.Text);
                try
                {
                    Directory.CreateDirectory(clientLocationText);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }

                if (!Directory.Exists(clientLocationText))
                {
                    MainSnackbar.MessageQueue.Enqueue("选择的文件夹不存在.");
                    Buttona.IsEnabled = true;
                    return;
                }

                var launchersavepath = $@"{clientLocationText}\OrigindLauncher.exe";
                var launcherdownloadpath = "http://origind.320.io/api/Launcher/Download";

                var dl = new WebClient();
                await dl.DownloadFileTaskAsync(new Uri(launcherdownloadpath), launchersavepath);
                Process.Start(launchersavepath);

                if (DesktopLinkToggleButton.IsChecked == true)
                {
                    try
                    {
                        var shortcut = (IWshShortcut)new WshShell().CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Origind.lnk");
                        shortcut.TargetPath = launchersavepath;
                        shortcut.WindowStyle = 1;
                        //shortcut.IconLocation = 
                        shortcut.Save();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                }
                Environment.Exit(0);
            }
            catch (Exception exception)
            {
                MainSnackbar.MessageQueue.Enqueue(exception.Message);
                Buttona.IsEnabled = true;

            }
        }

        private void SelectFolder(object sender, RoutedEventArgs e)
        {
            var f = new FolderBrowserDialog();
            f.ShowDialog();
            if (string.IsNullOrWhiteSpace(f.SelectedPath)) return;
            ClientLocation.Text = f.SelectedPath;
        }

        private void Move(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
