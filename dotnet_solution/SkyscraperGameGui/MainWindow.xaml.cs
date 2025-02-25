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
        var gameModel = new GameModel(9);
        gameModel.GridValues[0, 0] = 1;
        gameModel.LastSetIndex = (0, 0);
        UpdateGameGrid(gameModel);
    }

    private void InitializeGameGrid(int size)
    {
        int outer_size = size + 2;

        GameGrid.RowDefinitions.Clear();
        GameGrid.ColumnDefinitions.Clear();
        GameGrid.Children.Clear();

        NameScope.SetNameScope(GameGrid, new NameScope());

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
                    GameGrid.RegisterName(textBlock.Name, textBlock);

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
                    GameGrid.RegisterName(button.Name, button);
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
                    Name = $"grid_cellset_box{i}_{j}",
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
                GameGrid.RegisterName(subGrid.Name, subGrid);

                double cellSize = Math.Min(GameGrid.ActualWidth, GameGrid.ActualHeight) / size;
                TextBox textBox = new()
                {
                    Name = $"grid_cell_value_box_{i}_{j}",
                    Text = "",
                    FontSize = 24,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(2),
                    BorderThickness = new Thickness(0),
                    Background = Brushes.Transparent,
                    Width = cellSize * 0.8,
                    Height = cellSize * 0.8
                };
                GameGrid.Children.Add(textBox);
                Grid.SetRow(textBox, i + 1);
                Grid.SetColumn(textBox, j + 1);
                GameGrid.RegisterName(textBox.Name, textBox);

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
                GameGrid.RegisterName(button.Name, button);
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

    private void UpdateGameGrid(GameModel gameModel)
    {
        for (int i = 0; i < gameModel.Size; i++)
        {
            var topTextBlock = (TextBlock)GameGrid.FindName($"constr_box_top_{i}");
            if (topTextBlock != null)
                topTextBlock.Text = gameModel.TopValues[i].ToString();

            var bottomTextBlock = (TextBlock)GameGrid.FindName($"constr_box_bottom_{i}");
            if (bottomTextBlock != null)
                bottomTextBlock.Text = gameModel.BottomValues[i].ToString();

            var leftTextBlock = (TextBlock)GameGrid.FindName($"constr_box_left_{i}");
            if (leftTextBlock != null)
                leftTextBlock.Text = gameModel.LeftValues[i].ToString();

            var rightTextBlock = (TextBlock)GameGrid.FindName($"constr_box_right_{i}");
            if (rightTextBlock != null)
                rightTextBlock.Text = gameModel.RightValues[i].ToString();
        }

        for (int i = 0; i < gameModel.Size; i++)
        {
            for (int j = 0; j < gameModel.Size; j++)
            {
                var textBox = (TextBox)GameGrid.FindName($"grid_cell_value_box_{i}_{j}");
                if (textBox != null)
                {
                    textBox.Text = gameModel.GridValues[i, j] > 0 ? gameModel.GridValues[i, j].ToString() : "";
                    textBox.Background = Brushes.White;
                }

                var subGrid = (Grid)GameGrid.FindName($"grid_cellset_box{i}_{j}");
                if (subGrid != null)
                {
                    int number = 1;
                    for (int k = 0; k < 3; k++)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            var textBlock = subGrid.Children
                                .Cast<UIElement>()
                                .FirstOrDefault(e => Grid.GetRow(e) == k && Grid.GetColumn(e) == l)
                                as TextBlock;

                            if (textBlock != null)
                            {
                                textBlock.Opacity = 1;
                                textBlock.Text = number <= gameModel.Size ? number.ToString() : "";
                                if (gameModel.GridValues[i, j] > 0)
                                    textBlock.Opacity = 0;
                                else if (!gameModel.PossibleValues[i, j].Contains((byte)number))
                                    textBlock.Foreground = Brushes.Gray;
                                else
                                    textBlock.Foreground = Brushes.Black;
                                number++;
                            }
                        }
                    }
                }
            }
        }

        (int x, int y) = gameModel.LastSetIndex;
        if (x >= 0 && y >= 0)
        {
            var lastSetTextBox = (TextBox)GameGrid.FindName($"grid_cell_value_box_{x}_{y}");
            if (lastSetTextBox != null)
            {
                lastSetTextBox.Background = Brushes.LightGray;
            }
        }
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
