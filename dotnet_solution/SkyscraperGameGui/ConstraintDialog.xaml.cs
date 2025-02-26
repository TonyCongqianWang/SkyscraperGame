using System.Windows;
using System.Windows.Input;

namespace SkyscraperGameGui
{
    public partial class ConstraintDialog : Window
    {
        public ConstraintDialog()
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
