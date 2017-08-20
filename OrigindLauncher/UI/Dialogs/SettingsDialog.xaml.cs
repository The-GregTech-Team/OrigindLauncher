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