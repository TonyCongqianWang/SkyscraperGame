using System.IO;
using System.Windows;
using System.Windows.Input;

namespace SkyscraperGameGui;

public partial class HelpDialog : Window
{
    public HelpDialog()
    {
        InitializeComponent();
        LoadText();
    }

    private void LoadText()
    {
        string helpTextFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SkyscraperGameHelpText.txt");
        if (File.Exists(helpTextFile))
        {
            box.Text = File.ReadAllText(helpTextFile);
        }
        else
        {
            MessageBox.Show("Help file missing.", "Missing File", MessageBoxButton.OK, MessageBoxImage.Error);
        }
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