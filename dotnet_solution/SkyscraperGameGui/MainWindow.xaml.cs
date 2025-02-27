using System.Security.Cryptography;
using System.Text;
using System.Windows;
using SkyscraperGameEngine;

namespace SkyscraperGameGui;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    readonly GridRenderer renderer = new();
    readonly InfoRenderer infoRenderer;
    readonly GameEngine gameEngine = new();
    readonly MD5 md5 = MD5.Create();

    public MainWindow()
    {
        InitializeComponent();
        infoRenderer = new(PuzzleStatusLabel,
                           CurrentDepthLabel,
                           SolvingTimeLabel,
                           MovesValuesLabel);
        gameEngine.GetState();
        RenderNewGame();
    }
    public int OpenCellDialog()
    {
        Point gridCenter = GameGrid.PointToScreen(new Point(GameGrid.ActualWidth / 2, GameGrid.ActualHeight / 2));

        CellDialog cellDialog = new()
        {
            Owner = this,
            WindowStartupLocation = WindowStartupLocation.Manual
        };
        cellDialog.Loaded += (s, e) =>
        {
            cellDialog.Left = gridCenter.X - (cellDialog.ActualWidth / 2);
            cellDialog.Top = gridCenter.Y - (cellDialog.ActualHeight / 2);
        };
        cellDialog.ShowDialog();
        return -1;
    }

    private void NewGameButton_Click(object sender, RoutedEventArgs e)
    {
        NewGameDialog dialog = new()
        {
            Owner = this,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };
        dialog.ShowDialog();
        if (dialog.DialogResult == true)
        {
            byte[] seedBytes = Encoding.UTF8.GetBytes(RngSeedBox.Text);
            int seed = Math.Abs(BitConverter.ToInt32(md5.ComputeHash(seedBytes), 0));
            if (RngSeedBox.Text == "")
                seed = -1;
            if (!double.TryParse(GridFillPercentBox.Text, out double gridFillPercent))
                gridFillPercent = 0;
            if (!double.TryParse(ConstrFillPercentBox.Text, out double constrFillPercent))
                constrFillPercent = 100;
            InstanceGenerationOptions options = new()
            {
                Size = 9,
                RandomSeed = seed,
                GridFillRate = gridFillPercent / 100,
                ConstraintFillRate = constrFillPercent / 100,
                AllowInfeasible = AllowInFeasibleCheckbox.IsChecked == true
            };
            gameEngine.StartNewGame(options);
            RenderNewGame();
        }
    }

    private void UnsetButton_Click(object sender, RoutedEventArgs e)
    {
        gameEngine.TryUndoLast();
        GameStateModel gameStateModel = new(gameEngine.GetState());
        infoRenderer.UpdateInfo(gameStateModel);
    }

    private void RenderNewGame()
    {
        GameStateModel gameStateModel = new(gameEngine.GetState());
        infoRenderer.NewGame(gameStateModel);
        renderer.Render(GameGrid, gameStateModel);
    }
}
