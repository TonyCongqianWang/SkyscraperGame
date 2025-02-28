using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

using SkyscraperGameEngine;

namespace SkyscraperGameGui;

class InfoRenderer
{
    private readonly Label puzzleStatusLabel;
    private readonly Label currentDepthLabel;
    private readonly Label solvingTimeLabel;
    private readonly TextBlock movesValues;
    private readonly DispatcherTimer timer;
    private readonly Stopwatch stopwatch = new();

    public InfoRenderer(Label puzzleStatusLabel, Label currentDepthLabel, Label solvingTimeLabel, TextBlock movesValues)
    {
        this.puzzleStatusLabel = puzzleStatusLabel;
        this.currentDepthLabel = currentDepthLabel;
        this.solvingTimeLabel = solvingTimeLabel;
        this.movesValues = movesValues;
        timer = new()
        {
            Interval = TimeSpan.FromMilliseconds(1000),
        };
        timer.Tick += UpdateTime;
        timer.Start();
    }

    public void RenderInfo(GameStateViewModel gameModel, bool resetTimer=false)
    {
        if (resetTimer)
            StartTimer();
        UInt128 inserts = gameModel.NumInserts;
        UInt128 checks = gameModel.NumChecks;
        UInt128 unsets = gameModel.NumUnsets;
        if (gameModel.IsSolved)
        {
            puzzleStatusLabel.Foreground = new SolidColorBrush(Colors.LimeGreen);
            puzzleStatusLabel.Content = "Puzzle Solved!";
            StopTimer();
        }
        else if (gameModel.IsInfeasible)
        {
            puzzleStatusLabel.Foreground = new SolidColorBrush(Colors.Red);
            if (gameModel.CurrentDepth == 0)
            {
                StopTimer();
                puzzleStatusLabel.Content = "Puzzle Infeasible!";
            }
            else
                puzzleStatusLabel.Content = "Infeasible Node...";
        }
        else
        {
            puzzleStatusLabel.Content = "In Progress...";
        }
        currentDepthLabel.Content = gameModel.CurrentDepth.ToString();

        movesValues.Text = $"{inserts}\n{checks}\n{unsets}\n{inserts + checks + unsets}";
    }

    private void UpdateTime(object? sender, EventArgs e)
    {
        TimeSpan elapsed = stopwatch.Elapsed;
        string format = @"hh\:mm\:ss";
        if (!stopwatch.IsRunning)
            format += @"\.ff";
        solvingTimeLabel.Dispatcher.Invoke(() =>
        {
            solvingTimeLabel.Content = elapsed.ToString(format);
        });
    }

    private void StartTimer()
    {
        stopwatch.Restart();
        solvingTimeLabel.Foreground = new SolidColorBrush(Colors.Black);
    }

    private void StopTimer()
    {
        stopwatch.Stop();
        TimeSpan elapsed = stopwatch.Elapsed;
        solvingTimeLabel.Content = elapsed.ToString(@"hh\:mm\:ss\.ff");
        solvingTimeLabel.Foreground = new SolidColorBrush(Colors.LimeGreen);
    }
}
