﻿using System.Windows;
using System.Windows.Input;

namespace OrigindLauncher.UI
{
    /// <summary>
    ///     Interaction logic for SetupWindow.xaml
    /// </summary>
    public partial class SetupWindow : Window
    {
        public SetupWindow()
        {
            InitializeComponent();
        }

        private void Move(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}