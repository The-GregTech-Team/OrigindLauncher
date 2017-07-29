using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using OrigindLauncher.Resources.Configs;
using OrigindLauncher.Resources.UI;

namespace OrigindLauncher.UI.Dialogs
{
    /// <summary>
    ///     Interaction logic for ThemeDialog.xaml
    /// </summary>
    public partial class ThemeDialog : UserControl
    {
        public ThemeDialog()
        {
            InitializeComponent();
        }

        private void SaveAndClose(object sender, RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(this, this);
            ThemeManager.SavePalette();
        }

        private void ChangeLightDark(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeLightDark();
            var window = (LauncherWindow) Window.GetWindow(this);
            window.InitForTheme();
        }
    }

    public class PaletteSelectorViewModel
    {
        public PaletteSelectorViewModel()
        {
            Swatches = new SwatchesProvider().Swatches;
        }

        public ICommand ToggleBaseCommand { get; } = new AnotherCommandImplementation(o => ApplyBase((bool) o));

        public IEnumerable<Swatch> Swatches { get; }

        public ICommand ApplyPrimaryCommand { get; } = new AnotherCommandImplementation(o => ApplyPrimary((Swatch) o));

        public ICommand ApplyAccentCommand { get; } = new AnotherCommandImplementation(o => ApplyAccent((Swatch) o));

        private static void ApplyBase(bool isDark)
        {
            new PaletteHelper().SetLightDark(isDark);
        }

        private static void ApplyPrimary(Swatch swatch)
        {
            new PaletteHelper().ReplacePrimaryColor(swatch);
            Config.Instance.ThemeConfig.PrimaryName = swatch.Name;
        }

        private static void ApplyAccent(Swatch swatch)
        {
            new PaletteHelper().ReplaceAccentColor(swatch);
            Config.Instance.ThemeConfig.AccentName = swatch.Name;
        }
    }

    public class AnotherCommandImplementation : ICommand
    {
        private readonly Func<object, bool> _canExecute;
        private readonly Action<object> _execute;

        public AnotherCommandImplementation(Action<object> execute) : this(execute, null)
        {
        }

        public AnotherCommandImplementation(Action<object> execute, Func<object, bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? (x => true);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Refresh()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}