using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using MaterialDesignThemes.Wpf;
using OrigindLauncher.Resources.Configs;
using OrigindLauncher.Resources.Utils;
using UserControl = System.Windows.Controls.UserControl;

namespace OrigindLauncher.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for SettingsDialog.xaml
    /// </summary>
    public partial class SettingsDialog : UserControl
    {
        public SettingsDialog()
        {
            InitializeComponent();
            MemorySlider.Maximum = GC.GetTotalMemory(false) / 1024;
            MemorySlider.Value = Config.Instance.MaxMemory;
            foreach (var javapath in JavaHelper.FindJava())
            {
                ComboBoxChooseJava.Items.Add(javapath);
            }
            if (Config.Instance.JavaPath != null)
            {
                ComboBoxChooseJava.Text = Config.Instance.JavaPath;
            }

            Args.Text = Config.Instance.JavaArguments;
            DisableHardwareSpeedupToggleButton.IsChecked = Config.Instance.DisableHardwareSpeedup;

        }

        private void Cancal(object sender, RoutedEventArgs e) => Save();

        private void SaveAndClose(object sender, RoutedEventArgs e) => Save();

        private void Save()
        {
            Config.Instance.JavaPath = ComboBoxChooseJava.Text;
            Task.Run(() => Config.Save());
            DialogHost.CloseDialogCommand.Execute(this, this);
        }

        private void MemorySlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MemoryText.Text = (int)e.NewValue + "M";
            Config.Instance.MaxMemory = (int)MemorySlider.Value;
        }

        private void ChooseJava(object sender, RoutedEventArgs e)
        {
            var selecter = new OpenFileDialog {Filter = @"javaw.exe|javaw.exe"};
            selecter.ShowDialog();
            if (File.Exists(selecter.FileName))
            {
                Config.Instance.JavaPath = selecter.FileName;
            }
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
    }
}
