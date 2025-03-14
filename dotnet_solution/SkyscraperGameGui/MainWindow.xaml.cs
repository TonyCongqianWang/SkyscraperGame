using SkyscraperGameEngine;
using System.Windows;
using System.Windows.Input;

namespace SkyscraperGameGui;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    readonly PuzzlesQueue queue = new();
    readonly GridRenderer renderer;
    readonly InfoRenderer infoRenderer;
    readonly GameInterface gameEngine = new();
    readonly NewGameHandler newGameHandler;
    readonly ConstraintCheckHandler constraintCheckHandler;

    private static LoadSaveDialog? loadSaveDialogInstance;

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
        newGameHandler = new(gameEngine,
                             queue,
                             GridSizeBox,
                             GridFillPercentBox,
                             ConstrFillPercentBox,
                             AllowInFeasibleCheckbox);
        newGameHandler.SendStartNewGameRequest();
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
            newGameHandler.SendStartNewGameRequest();
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
        GameObservation gameStateModel = gameEngine.GetState();
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
        dialog.ShowDialog();
    }

    private void LoadSaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (loadSaveDialogInstance == null)
        {
            loadSaveDialogInstance = new LoadSaveDialog(queue, newGameHandler)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };
            loadSaveDialogInstance.Closed += (s, args) =>
            {
                loadSaveDialogInstance = null;
                Focus();
            };
            loadSaveDialogInstance.Show();
        }
        else
        {
            loadSaveDialogInstance.Activate();
        }
    }
}
