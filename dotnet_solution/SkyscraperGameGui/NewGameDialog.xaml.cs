using System.Windows;
using System.Windows.Input;

namespace SkyscraperGameGui
{
    public partial class NewGameDialog : Window
    {
        public NewGameDialog()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
