using SkyscraperGameEngine;
using System.Windows;
using System.Windows.Input;

namespace SkyscraperGameGui;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    readonly GridRenderer renderer;
    readonly InfoRenderer infoRenderer;
    readonly GameInterface gameEngine = new();
    readonly NewGameHandler newGameHandler;
    readonly ConstraintCheckHandler constraintCheckHandler;

    public MainWindow()
    {
        InitializeComponent();
        GridButtonCallbackFactory cellCallbackFactory = new(this, gameEngine);
        constraintCheckHandler = new(this, gameEngine);
        renderer = new(cellCallbackFactory, constraintCheckHandler);
        infoRenderer = new(PuzzleStatusLabel,
                           CurrentDepthLabel,
                           SolvingTimeLabel,
                           MovesValuesLabel);
        gameEngine.GetState();
        newGameHandler = new(gameEngine,
                             RngSeedBox,
                             GridSizeBox,
                             GridFillPercentBox,
                             ConstrFillPercentBox,
                             AllowInFeasibleCheckbox);
        RenderGame(resetTimer: true);
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
            newGameHandler.SendNewGameRequest();
            RenderGame(resetTimer: true);
        }
    }

    private void UnsetButton_Click(object sender, RoutedEventArgs e)
    {
        gameEngine.TryUndoLast();
        RenderGame();
    }

    public void RenderGame(bool resetTimer = false)
    {
        GameStateViewModel gameStateModel = gameEngine.GetState();
        infoRenderer.RenderInfo(gameStateModel, resetTimer);
        renderer.Render(GameGrid, gameStateModel);
    }

    private void CheckAllButton_Click(object sender, RoutedEventArgs e)
    {
        constraintCheckHandler.ExecuteCheckAll();
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Back || e.Key == Key.Delete)
        {
            UnsetButton_Click(sender, e);
        }
    }

    private void HelpButton_Click(object sender, RoutedEventArgs e)
    {
        HelpDialog dialog = new()
        {
            Owner = this,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };
        dialog.Show();
    }

    private void LoadSaveButton_Click(object sender, RoutedEventArgs e)
    {
        LoadSaveDialog dialog = new()
        {
            Owner = this,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };
        dialog.Show();
    }
}
