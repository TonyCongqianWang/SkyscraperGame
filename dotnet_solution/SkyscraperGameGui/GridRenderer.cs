using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SkyscraperGameGui;

class GridRenderer
{
    private int gridSize = 0;

    public void Render(Grid gameGrid, GameModel model)
    {
        if (gridSize != model.Size)
        {
            gridSize = model.Size;
            CreateGrid(gameGrid, model.Size);
        }

        for (int i = 0; i < model.Size; i++)
        {
            var topTextBlock = (TextBlock)gameGrid.FindName($"constr_box_top_{i}");
            if (topTextBlock != null)
            {
                topTextBlock.Text = model.TopValues[i].ToString();
                if (model.TopValuesCheckStatus[i])
                    topTextBlock.Foreground = Brushes.Green;
                else
                    topTextBlock.Foreground = Brushes.Orange;
            }

            var bottomTextBlock = (TextBlock)gameGrid.FindName($"constr_box_bottom_{i}");
            if (bottomTextBlock != null)
            {
                bottomTextBlock.Text = model.BottomValues[i].ToString();
                if (model.BottomValuesCheckStatus[i])
                    bottomTextBlock.Foreground = Brushes.Green;
                else
                    bottomTextBlock.Foreground = Brushes.Orange;
            }

            var leftTextBlock = (TextBlock)gameGrid.FindName($"constr_box_left_{i}");
            if (leftTextBlock != null)
            {
                leftTextBlock.Text = model.LeftValues[i].ToString();
                if (model.LeftValuesCheckStatus[i])
                    leftTextBlock.Foreground = Brushes.Green;
                else
                    leftTextBlock.Foreground = Brushes.Orange;
            }

            var rightTextBlock = (TextBlock)gameGrid.FindName($"constr_box_right_{i}");
            if (rightTextBlock != null)
            {
                rightTextBlock.Text = model.RightValues[i].ToString();
                if (model.RightValuesCheckStatus[i])
                    rightTextBlock.Foreground = Brushes.Green;
                else
                    rightTextBlock.Foreground = Brushes.Orange;
            }
        }

        for (int i = 0; i < model.Size; i++)
        {
            for (int j = 0; j < model.Size; j++)
            {
                var textBox = (TextBox)gameGrid.FindName($"grid_cell_value_box_{i}_{j}");
                if (textBox != null)
                {
                    textBox.Text = model.GridValues[i, j] > 0 ? model.GridValues[i, j].ToString() : "";
                    textBox.Background = Brushes.White;
                }

                var subGrid = (Grid)gameGrid.FindName($"grid_cellset_box{i}_{j}");
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
                                textBlock.Text = number <= model.Size ? number.ToString() : "";
                                if (model.GridValues[i, j] > 0 || number > model.Size)
                                    textBlock.Opacity = 0;
                                else if (!model.GridValueValidities[i, j][number - 1])
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
        (int x, int y) = model.LastSetIndex;
        if (x >= 0 && y >= 0)
        {
            var lastSetTextBox = (TextBox)gameGrid.FindName($"grid_cell_value_box_{x}_{y}");
            if (lastSetTextBox != null)
            {
                lastSetTextBox.Background = Brushes.LightGray;
            }
        }
    }

    private static void CreateGrid(Grid gameGrid, int size)
    {
        int outer_size = size + 2;

        gameGrid.RowDefinitions.Clear();
        gameGrid.ColumnDefinitions.Clear();
        gameGrid.Children.Clear();

        NameScope.SetNameScope(gameGrid, new NameScope());

        for (int i = 0; i < outer_size; i++)
        {
            gameGrid.RowDefinitions.Add(new RowDefinition());
            gameGrid.ColumnDefinitions.Add(new ColumnDefinition());
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
                    gameGrid.Children.Add(textBlock);
                    Grid.SetRow(textBlock, i);
                    Grid.SetColumn(textBlock, j);
                    gameGrid.RegisterName(textBlock.Name, textBlock);

                    Button button = new()
                    {
                        Name = $"constr_button{position}",
                        Opacity = 0,
                        Background = Brushes.Transparent
                    };
                    button.Click += (sender, e) => { /* Event handler code */ };
                    gameGrid.Children.Add(button);
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                    gameGrid.RegisterName(button.Name, button);
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
                gameGrid.RegisterName(subGrid.Name, subGrid);

                double cellSize = Math.Min(gameGrid.Width, gameGrid.Height) / size;
                TextBox textBox = new()
                {
                    Name = $"grid_cell_value_box_{i}_{j}",
                    Text = "",
                    TextAlignment = TextAlignment.Center,
                    FontSize = 24,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(2),
                    BorderThickness = new Thickness(0),
                    Background = Brushes.Transparent,
                    Width = cellSize * 0.8,
                    Height = cellSize * 0.8
                };
                gameGrid.Children.Add(textBox);
                Grid.SetRow(textBox, i + 1);
                Grid.SetColumn(textBox, j + 1);
                gameGrid.RegisterName(textBox.Name, textBox);

                Button button = new()
                {
                    Name = $"grid_cell_button_{i}_{j}",
                    Opacity = 0,
                    Background = Brushes.Transparent
                };
                button.Click += (sender, e) => { /* Event handler code */ };
                innerGrid.Children.Add(button);
                Grid.SetRow(button, i);
                Grid.SetColumn(button, j);
                gameGrid.RegisterName(button.Name, button);
            }
        }

        Border innerGridBorder = new()
        {
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(2),
            Child = innerGrid
        };

        gameGrid.Children.Add(innerGridBorder);
        Grid.SetRow(innerGridBorder, 1);
        Grid.SetColumn(innerGridBorder, 1);
        Grid.SetRowSpan(innerGridBorder, size);
        Grid.SetColumnSpan(innerGridBorder, size);
    }
}
