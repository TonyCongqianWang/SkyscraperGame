using System.Windows;

namespace SkyscraperGameGui;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    GridRenderer renderer = new();

    public MainWindow()
    {
        InitializeComponent();
        var gameModel = new GameModel(9);
        renderer.Render(GameGrid, gameModel);
        gameModel.GridValues[0, 0] = 1;
        gameModel.LastSetIndex = (0, 0);
        renderer.Render(GameGrid, gameModel);
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
        return (-1);
    }

    private void NewGameButton_Click(object sender, RoutedEventArgs e)
    {
        NewGameDialog dialog = new()
        {
            Owner = this,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };
        dialog.ShowDialog();
    }

    private void UnsetButton_Click(object sender, RoutedEventArgs e)
    {
    }
}
