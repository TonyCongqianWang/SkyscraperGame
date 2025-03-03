using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace SkyscraperGameGui;

public partial class LoadSaveDialog : Window
{
    [GeneratedRegex("[^0-9]+")]
    private static partial Regex numberRegex();

    public LoadSaveDialog()
    {
        InitializeComponent();
    }

    private void ValidateCountBoxInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = numberRegex().IsMatch(e.Text);
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
            Close();
    }
}
