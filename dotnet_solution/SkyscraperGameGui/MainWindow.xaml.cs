using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace SkyscraperGameGui;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        InitializeGameGrid(9);
    }

    private void InitializeGameGrid(int size)
    {
        int outer_size = size + 2;

        GameGrid.RowDefinitions.Clear();
        GameGrid.ColumnDefinitions.Clear();
        GameGrid.Children.Clear();

        for (int i = 0; i < outer_size; i++)
        {
            GameGrid.RowDefinitions.Add(new RowDefinition());
            GameGrid.ColumnDefinitions.Add(new ColumnDefinition());
        }

        for (int i = 0; i < outer_size; i++)
        {
            for (int j = 0; j < outer_size; j++)
            {
                if ((i == 0 || i == outer_size - 1 || j == 0 || j == outer_size - 1)
                    && !((i == 0 || i == outer_size - 1) && (j == 0 || j == outer_size - 1)))
                {
                    string position = "";
                    if (i == 0)
                        position = $"_top_{j - 1}";
                    else if (i == outer_size - 1)
                        position = $"_bottom_{j - 1}";
                    else if (j == 0)
                        position = $"_left_{i - 1}";
                    else if (j == outer_size - 1)
                        position = $"_right_{i - 1}";
                    TextBlock textBlock = new()
                    {
                        Name = $"constr_box{position}",
                        Text = "0",
                        FontSize = 20,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(2)
                    };
                    GameGrid.Children.Add(textBlock);
                    Grid.SetRow(textBlock, i);
                    Grid.SetColumn(textBlock, j);

                    Button button = new()
                    {
                        Name = $"constr_button{position}",
                        Opacity = 0,
                        Background = Brushes.Transparent
                    };
                    button.Click += CellButton_Click;
                    GameGrid.Children.Add(button);
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                }
            }
        }

        int subGridSize = (int)Math.Ceiling(Math.Sqrt(size));
        Grid innerGrid = new()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Margin = new Thickness(0),
            ShowGridLines = false
        };

        for (int i = 0; i < size; i++)
        {
            innerGrid.RowDefinitions.Add(new RowDefinition());
            innerGrid.ColumnDefinitions.Add(new ColumnDefinition());
        }

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Grid subGrid = new()
                {
                    Name = $"grid_cell_box_{i}_{j}",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5)
                };

                for (int k = 0; k < 3; k++)
                {
                    subGrid.RowDefinitions.Add(new RowDefinition());
                    subGrid.ColumnDefinitions.Add(new ColumnDefinition());
                }

                int number = 1;
                for (int k = 0; k < subGridSize; k++)
                {
                    for (int l = 0; l < subGridSize; l++)
                    {
                        TextBlock textBlock = new()
                        {
                            Text = number <= size ? number.ToString() : "",
                            FontSize = 12,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(2)
                        };
                        subGrid.Children.Add(textBlock);
                        Grid.SetRow(textBlock, k);
                        Grid.SetColumn(textBlock, l);
                        number++;
                    }
                }

                innerGrid.Children.Add(subGrid);
                Grid.SetRow(subGrid, i);
                Grid.SetColumn(subGrid, j);

                Button button = new()
                {
                    Name = $"grid_cell_button_{i}_{j}",
                    Opacity = 0,
                    Background = Brushes.Transparent
                };
                button.Click += CellButton_Click;
                innerGrid.Children.Add(button);
                Grid.SetRow(button, i);
                Grid.SetColumn(button, j);
            }
        }

        Border innerGridBorder = new()
        {
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(2),
            Child = innerGrid
        };

        GameGrid.Children.Add(innerGridBorder);
        Grid.SetRow(innerGridBorder, 1);
        Grid.SetColumn(innerGridBorder, 1);
        Grid.SetRowSpan(innerGridBorder, size);
        Grid.SetColumnSpan(innerGridBorder, size);
    }

    private void CellButton_Click(object sender, RoutedEventArgs e)
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
