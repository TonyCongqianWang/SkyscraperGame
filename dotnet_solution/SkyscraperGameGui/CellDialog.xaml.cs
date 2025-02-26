﻿using System.Windows;
using System.Windows.Input;

namespace SkyscraperGameGui
{
    public partial class CellDialog : Window
    {
        public CellDialog()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CancelButton_Click(sender, e);
            }
        }
    }
}
