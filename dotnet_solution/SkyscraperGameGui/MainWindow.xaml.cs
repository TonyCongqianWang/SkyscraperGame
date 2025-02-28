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

    public void RenderGame(bool resetTimer=false)
    {
        GameStateViewModel gameStateModel = gameEngine.GetState();
        infoRenderer.RenderInfo(gameStateModel, resetTimer);
        renderer.Render(GameGrid, gameStateModel);
    }

    private void CheckAllButton_Click(object sender, RoutedEventArgs e)
    {
        constraintCheckHandler.ExecuteCheckAll();
    }
}
