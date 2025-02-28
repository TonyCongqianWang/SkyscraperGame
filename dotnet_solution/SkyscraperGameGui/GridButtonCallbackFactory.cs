using SkyscraperGameEngine;
using System.Windows.Controls;
using System.Windows;

namespace SkyscraperGameGui;

class GridButtonCallbackFactory(MainWindow mainWindow, GameEngine gameEngine)
{
    public Action CreateCallback(Grid cellGrid, (int, int) position)
    {
        return () => CreateAndShowDialog(cellGrid, position);
    }

    private void CreateAndShowDialog(Grid cellGrid, (int, int) position)
    {
        void digitCallback(int digit)
        {
            gameEngine.TryInsertValue(position, (byte)digit);
            mainWindow.Focus();
            mainWindow.RenderGame();
        }
        CellDialog cellDialog = new(cellGrid, digitCallback)
        {
            Owner = mainWindow,
            WindowStartupLocation = WindowStartupLocation.Manual
        };
        Grid gameGrid = mainWindow.GameGrid;
        Point gridCenter = gameGrid.PointToScreen(
            new Point(gameGrid.ActualWidth / 2, gameGrid.ActualHeight / 2));
        cellDialog.Loaded += (s, e) =>
        {
            cellDialog.Left = gridCenter.X - (cellDialog.ActualWidth / 2);
            cellDialog.Top = gridCenter.Y - (cellDialog.ActualHeight / 2);
        };
        cellDialog.ShowDialog();
    }
}
