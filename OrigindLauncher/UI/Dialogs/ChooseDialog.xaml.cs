using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace OrigindLauncher.UI.Dialogs
{
    /// <summary>
    ///     Interaction logic for ChooseDialog.xaml
    /// </summary>
    public partial class ChooseDialog : UserControl
    {
        public ChooseDialog(string title, string message, string acceptText = "确认", string refuseText = "取消")
        {
            InitializeComponent();
            Message.Text = message;
            Title.Text = title;
            AcceptButton.Content = acceptText;
            RefuseButton.Content = refuseText;
        }

        public bool Result { get; private set; }

        private void Accept(object sender, RoutedEventArgs e)
        {
            Result = true;
            Close();
        }

        private void Cancal(object sender, RoutedEventArgs e)
        {
            Result = false;
            Close();
        }

        private void Close()
        {
            //await Task.Delay(200);
            DialogHost.CloseDialogCommand.Execute(this, this);
        }
    }
}