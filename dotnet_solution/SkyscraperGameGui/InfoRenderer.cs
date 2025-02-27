using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace SkyscraperGameGui;

class InfoRenderer
{
    private readonly Label puzzleStatusLabel;
    private readonly Label currentDepthLabel;
    private readonly Label solvingTimeLabel;
    private readonly TextBlock movesValues;
    private readonly DispatcherTimer timer;
    private DateTime startTime = DateTime.Now;

    public InfoRenderer(Label puzzleStatusLabel, Label currentDepthLabel, Label solvingTimeLabel, TextBlock movesValues)
    {
        this.puzzleStatusLabel = puzzleStatusLabel;
        this.currentDepthLabel = currentDepthLabel;
        this.solvingTimeLabel = solvingTimeLabel;
        this.movesValues = movesValues;
        timer = new()
        {
            Interval = TimeSpan.FromSeconds(1),
        };
        timer.Tick += UpdateTime;
    }

    public void NewGame(GameStateModel gameModel)
    {
        StartTimer();
        UpdateInfo(gameModel);
    }

    public void UpdateInfo(GameStateModel gameModel)
    {
        UInt128 inserts = gameModel.NumInserts;
        UInt128 checks = gameModel.NumChecks;
        UInt128 unsets = gameModel.NumUnsets;
        if (gameModel.IsSolved)
        {
            puzzleStatusLabel.Content = "Puzzle Solved!";
            StopTimer();
        }
        else if (gameModel.IsInfeasible)
        {
            if (gameModel.CurrentDepth == 0)
            {
                StopTimer();
                puzzleStatusLabel.Content = "Puzzle Infeasible!";
            }
            else
                puzzleStatusLabel.Content = "Infeasible State...";
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
        TimeSpan elapsed = DateTime.Now - startTime;
        solvingTimeLabel.Dispatcher.Invoke(() =>
        {
            solvingTimeLabel.Content = elapsed.ToString(@"hh\:mm\:ss");
        });
    }

    private void StartTimer()
    {
        startTime = DateTime.Now;
        timer.Start();
        solvingTimeLabel.Foreground = new SolidColorBrush(Colors.Black);
    }

    private void StopTimer()
    {
        timer.Stop();
        TimeSpan elapsed = DateTime.Now - startTime;
        solvingTimeLabel.Content = elapsed.ToString(@"hh\:mm\:ss\.ff");
        solvingTimeLabel.Foreground = new SolidColorBrush(Colors.Green);
    }
}
