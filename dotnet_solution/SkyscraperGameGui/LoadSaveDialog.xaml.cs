using SkyscraperGameEngine;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace SkyscraperGameGui;

partial class LoadSaveDialog : Window
{
    readonly PuzzlesQueue queue;
    readonly NewGameHandler gameHandler;

    [GeneratedRegex("[^0-9]+")]
    private static partial Regex numberRegex();

    internal LoadSaveDialog(PuzzlesQueue queue, NewGameHandler gameHandler)
    {
        this.queue = queue;
        this.gameHandler = gameHandler;
        InitializeComponent();
        FillTextBoxes();
    }

    private void FillTextBoxes()
    {
        currentPuzzleTextbox.Text = queue.CurrentPuzzleString;
        currentPositionTextbox.Text = queue.CurrentPositionString;
        StringBuilder stringBuilder = new();
        foreach (var str in queue.UpconingPuzzleStrings)
        {
            stringBuilder.AppendLine(str);
        }
        queueTextbox.Text = stringBuilder.ToString();
    }

    private void ValidateCountBoxInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = numberRegex().IsMatch(e.Text);
    }

    private void GenerateButton_Click(object sender, RoutedEventArgs e)
    {
        GameInterface game = new();
        if (int.TryParse(CountTextBox.Text, out int count))
        {
            for (int i = 0; i < count; i++)
            {
                queueTextbox.Text += gameHandler.SendGenerateNewGameRequest() + Environment.NewLine;
            }
        }
    }

    private void ConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        var puzzleStrings = queueTextbox.Text.ReplaceLineEndings(Environment.NewLine).Split(Environment.NewLine);
        queue.ReplaceQueue(puzzleStrings);
        Close();
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
