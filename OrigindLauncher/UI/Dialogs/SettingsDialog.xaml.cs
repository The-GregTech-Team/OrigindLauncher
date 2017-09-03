using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MaterialDesignThemes.Wpf;
using OrigindLauncher.Resources.Client;
using OrigindLauncher.Resources.Configs;
using OrigindLauncher.Resources.Server;
using OrigindLauncher.Resources.Utils;
using UserControl = System.Windows.Controls.UserControl;

namespace OrigindLauncher.UI.Dialogs
{
    /// <summary>
    ///     Interaction logic for SettingsDialog.xaml
    /// </summary>
    public partial class SettingsDialog : UserControl
    {
        public SettingsDialog()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            MemorySlider.Maximum = 8192;
            MemorySlider.Value = Config.Instance.MaxMemory;

            foreach (var javapath in JavaFinder.FindJava())
                ComboBoxChooseJava.Items.Add(javapath);
            if (Config.Instance.JavaPath != null)
                ComboBoxChooseJava.Text = Config.Instance.JavaPath;
            Args.Text = Config.Instance.JavaArguments;
            DisableHardwareSpeedupToggleButton.IsChecked = Config.Instance.DisableHardwareSpeedup;
            EnableLaunchProgress.IsChecked = Config.Instance.LaunchProgress;
            UseGameBoost.IsChecked = Config.Instance.UseBoost;
            UseAdmin.IsChecked = Config.Instance.UseAdmin;
            UseScreenShare.IsChecked = Config.Instance.AllowScreenshotShare;
            UseDebug.IsChecked = Config.Instance.EnableDebug;

            foreach (var font in GetFonts())
            {
                ComboBoxChooseFont.Items.Add(font);
            }

            var path = ClientManager.GetGameStorageDirectory("config/betterfonts.cfg");
            try
            {
                var line = File.ReadAllLines(path).FirstOrDefault(t => t.StartsWith("font.name"));
                var name = line.Split('=')[1];
                ComboBoxChooseFont.Items.Add(UnicodeToString(name));
                ComboBoxChooseFont.Text = UnicodeToString(name);

            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        public static IEnumerable GetFonts()
        {
            return new InstalledFontCollection().Families.Select(font => font.Name).ToList();
        }

        public string StringToUnicode(string s)
        {
            var charbuffers = s.ToCharArray();
            var sb = new StringBuilder();
            foreach (var buffer in charbuffers.Select(t => Encoding.Unicode.GetBytes(t.ToString())))
                sb.Append($"\\u{buffer[1]:X2}{buffer[0]:X2}");
            return sb.ToString();
        }

        public string UnicodeToString(string value)
        {
            return Regex.Replace(
                value,
                @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m => ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString());
        }

        private void Cancal(object sender, RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(this, this);
        }

        private void SaveAndClose(object sender, RoutedEventArgs e)
        {
            Save();
            Config.Instance.JavaPath = ComboBoxChooseJava.Text;
            Task.Run(() => Config.Save());
            DialogHost.CloseDialogCommand.Execute(this, this);
        }

        private void Save()
        {
            Config.Instance.MaxMemory = (int)MemorySlider.Value;
            Config.Instance.JavaArguments = Args.Text;
            if (DisableHardwareSpeedupToggleButton.IsChecked != null)
                Config.Instance.DisableHardwareSpeedup = DisableHardwareSpeedupToggleButton.IsChecked.Value;
            if (EnableLaunchProgress.IsChecked != null)
                Config.Instance.LaunchProgress = EnableLaunchProgress.IsChecked.Value;
            if (UseGameBoost.IsChecked != null)
                Config.Instance.UseBoost = UseGameBoost.IsChecked.Value;
            if (UseAdmin.IsChecked != null)
                Config.Instance.UseAdmin = UseAdmin.IsChecked.Value;
            if (UseScreenShare.IsChecked != null)
                Config.Instance.AllowScreenshotShare = UseScreenShare.IsChecked.Value;
            if (UseDebug.IsChecked != null)
                Config.Instance.EnableDebug = UseDebug.IsChecked.Value;

            var path = ClientManager.GetGameStorageDirectory("config/betterfonts.cfg");
            try
            {
                var lines = File.ReadAllLines(path);
                for (var index = 0; index < lines.Length; index++)
                {
                    var line = lines[index];
                    if (line.StartsWith("font.name"))
                    {
                        lines[index] = "font.name=" + StringToUnicode(ComboBoxChooseFont.Text);
                        break;
                    }
                }
                File.WriteAllLines(path, lines);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }


        private void MemorySlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MemoryText.Text = (int)e.NewValue + "M";

        }

        private void ChooseJava(object sender, RoutedEventArgs e)
        {
            var selecter = new OpenFileDialog { Filter = @"javaw.exe|javaw.exe" };
            selecter.ShowDialog();
            if (File.Exists(selecter.FileName))
                Config.Instance.JavaPath = selecter.FileName;
        }

        private void ForceUpdate(object sender, RoutedEventArgs e)
        {
            ClientManager.CurrentInfo = ClientManager.MakeClientInfo();
        }

        private void Submit(object sender, RoutedEventArgs e)
        {
            var text = OpinionTextBox.Text;
            Task.Run(() => MessageUploadManager.Suggests($"{Config.Instance.PlayerAccount.Username}:{text}"));
        }
    }
}