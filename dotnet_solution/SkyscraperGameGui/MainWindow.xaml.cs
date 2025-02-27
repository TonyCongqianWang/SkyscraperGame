using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using SkyscraperGameEngine;

namespace SkyscraperGameGui;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    readonly GridRenderer renderer;
    readonly InfoRenderer infoRenderer;
    readonly GameEngine gameEngine = new();
    readonly MD5 md5 = MD5.Create();

    public MainWindow()
    {
        InitializeComponent();
        renderer = new(CreateCellDialogCallback);
        infoRenderer = new(PuzzleStatusLabel,
                           CurrentDepthLabel,
                           SolvingTimeLabel,
                           MovesValuesLabel);
        gameEngine.GetState();
        RenderNewGame();
    }
    public Action CreateCellDialogCallback(Grid cellGrid, (int, int) position)
    {
        return () =>
        {
            Point gridCenter = GameGrid.PointToScreen(new Point(GameGrid.ActualWidth / 2, GameGrid.ActualHeight / 2));
            Action<int> digitCallback = (digit) =>
            {
                gameEngine.TryInsertValue(position, (byte)digit);
                GameStateModel gameStateModel = new(gameEngine.GetState());
                RenderUpdate(gameStateModel);
            };
            CellDialog cellDialog = new(cellGrid, digitCallback)
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
        };
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
            InstanceGenerationOptions options = new()
            {
                Size = 4,
                RandomSeed = seed,
                AllowInfeasible = AllowInFeasibleCheckbox.IsChecked == true
            };
            if (double.TryParse(GridFillPercentBox.Text, out double gridFillPercent))
                options.GridFillRate = gridFillPercent / 100;
            if (double.TryParse(ConstrFillPercentBox.Text, out double constrFillPercent))
                options.ConstraintFillRate = gridFillPercent / 100;
            gameEngine.StartNewGame(options);
            RenderNewGame();
        }
    }

    private void UnsetButton_Click(object sender, RoutedEventArgs e)
    {
        gameEngine.TryUndoLast();
        GameStateModel gameStateModel = new(gameEngine.GetState());
        RenderUpdate(gameStateModel);
    }

    private void RenderUpdate(GameStateModel gameStateModel)
    {
        infoRenderer.UpdateInfo(gameStateModel);
        renderer.Render(GameGrid, gameStateModel);
    }

    private void RenderNewGame()
    {
        GameStateModel gameStateModel = new(gameEngine.GetState());
        infoRenderer.NewGame(gameStateModel);
        renderer.Render(GameGrid, gameStateModel);
    }
}
