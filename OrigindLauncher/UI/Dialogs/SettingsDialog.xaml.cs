using System.IO;
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
            MemorySlider.Maximum = 8192;
            MemorySlider.Value = Config.Instance.MaxMemory;

            foreach (var javapath in JavaHelper.FindJava())
                ComboBoxChooseJava.Items.Add(javapath);
            if (Config.Instance.JavaPath != null)
                ComboBoxChooseJava.Text = Config.Instance.JavaPath;

            Args.Text = Config.Instance.JavaArguments;
            DisableHardwareSpeedupToggleButton.IsChecked = Config.Instance.DisableHardwareSpeedup;
            EnableLaunchProgress.IsChecked = Config.Instance.LaunchProgress;
        }

        private void Cancal(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void SaveAndClose(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void Save()
        {
            Config.Instance.JavaPath = ComboBoxChooseJava.Text;
            Task.Run(() => Config.Save());
            DialogHost.CloseDialogCommand.Execute(this, this);
        }

        private void MemorySlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MemoryText.Text = (int) e.NewValue + "M";
            Config.Instance.MaxMemory = (int) MemorySlider.Value;
        }

        private void ChooseJava(object sender, RoutedEventArgs e)
        {
            var selecter = new OpenFileDialog {Filter = @"javaw.exe|javaw.exe"};
            selecter.ShowDialog();
            if (File.Exists(selecter.FileName))
                Config.Instance.JavaPath = selecter.FileName;
        }

        private void Args_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Config.Instance.JavaArguments = Args.Text;
        }

        private void DisableHardwareSpeedupToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (DisableHardwareSpeedupToggleButton.IsChecked != null)
                Config.Instance.DisableHardwareSpeedup = DisableHardwareSpeedupToggleButton.IsChecked.Value;
        }

        private void EnableLaunchProgress_OnClick(object sender, RoutedEventArgs e)
        {
            if (EnableLaunchProgress.IsChecked != null)
                Config.Instance.LaunchProgress = EnableLaunchProgress.IsChecked.Value;
        }

        private void ForceUpdate(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Origind Launcher检测到当前存在一个客户端.\r\n" +
                                           "如果你想继续, 这个客户端会的一些内容, 比如截图, 小地图可能会被删除, 请先备份.");
            ClientManager.CurrentInfo = ClientManager.MakeClientInfo();
        }

        private void Submit(object sender, RoutedEventArgs e)
        {
            var text = OpinionTextBox.Text;
            Task.Run(() => MessageUploadManager.Suggests($"{Config.Instance.PlayerAccount.Username}:{text}"));
        }
    }
}