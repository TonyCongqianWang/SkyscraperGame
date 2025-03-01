using SkyscraperGameEngine;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SkyscraperGameGui;

class GridRenderer(
    GridButtonCallbackFactory cellDialogCallbackFactory,
    ConstraintCheckHandler constraintDialogCallbackFactory)
{
    private int gridSize = 0;

    public void Render(Grid gameGrid, GameStateViewModel model)
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
                var topButton = (Button)gameGrid.FindName($"constr_button_top_{i}");
                if (model.TopValues[i] > 0)
                {
                    topButton?.SetValue(UIElement.IsEnabledProperty, true);
                    topTextBlock.Text = model.TopValues[i].ToString();
                }
                else
                {
                    topButton?.SetValue(UIElement.IsEnabledProperty, false);
                    topTextBlock.Text = "";
                }
                if (!model.TopValueNeedsCheckArray[i])
                    topTextBlock.Foreground = Brushes.LimeGreen;
                else
                    topTextBlock.Foreground = Brushes.Orange;
            }

            var bottomTextBlock = (TextBlock)gameGrid.FindName($"constr_box_bottom_{i}");
            if (bottomTextBlock != null)
            {
                var bottomButton = (Button)gameGrid.FindName($"constr_button_bottom_{i}");
                if (model.BottomValues[i] > 0)
                {
                    bottomButton?.SetValue(UIElement.IsEnabledProperty, true);
                    bottomTextBlock.Text = model.BottomValues[i].ToString();
                }
                else
                {
                    bottomButton?.SetValue(UIElement.IsEnabledProperty, false);
                    bottomTextBlock.Text = "";
                }
                if (!model.BottomValueNeedsCheckArray[i])
                    bottomTextBlock.Foreground = Brushes.LimeGreen;
                else
                    bottomTextBlock.Foreground = Brushes.Orange;
            }

            var leftTextBlock = (TextBlock)gameGrid.FindName($"constr_box_left_{i}");
            if (leftTextBlock != null)
            {
                var leftButton = (Button)gameGrid.FindName($"constr_button_left_{i}");
                if (model.LeftValues[i] > 0)
                {
                    leftButton?.SetValue(UIElement.IsEnabledProperty, true);
                    leftTextBlock.Text = model.LeftValues[i].ToString();
                }
                else
                {
                    leftButton?.SetValue(UIElement.IsEnabledProperty, false);
                    leftTextBlock.Text = "";
                }
                if (!model.LeftValueNeedsCheckArray[i])
                    leftTextBlock.Foreground = Brushes.LimeGreen;
                else
                    leftTextBlock.Foreground = Brushes.Orange;
            }

            var rightTextBlock = (TextBlock)gameGrid.FindName($"constr_box_right_{i}");
            if (rightTextBlock != null)
            {
                var rightButton = (Button)gameGrid.FindName($"constr_button_right_{i}");
                if (model.RightValues[i] > 0)
                {
                    rightButton?.SetValue(UIElement.IsEnabledProperty, true);
                    rightTextBlock.Text = model.RightValues[i].ToString();
                }
                else
                {
                    rightButton?.SetValue(UIElement.IsEnabledProperty, false);
                    rightTextBlock.Text = "";
                }
                if (!model.RightValueNeedsCheckArray[i])
                    rightTextBlock.Foreground = Brushes.LimeGreen;
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
                    var cellButton = (Button)gameGrid.FindName($"grid_cell_button_{i}_{j}");
                    if (model.GridValues[i, j] > 0)
                    {
                        cellButton?.SetValue(UIElement.IsEnabledProperty, false);
                        textBox.Text = model.GridValues[i, j].ToString();
                    }
                    else
                    {
                        cellButton?.SetValue(UIElement.IsEnabledProperty, true);
                        textBox.Text = "";
                    }
                    if (model.IsInfeasible)
                        cellButton?.SetValue(UIElement.IsEnabledProperty, false);
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
                            if (subGrid.Children
                                .Cast<UIElement>()
                                .FirstOrDefault(e =>
                                    Grid.GetRow(e) == k && Grid.GetColumn(e) == l)
                                is TextBlock textBlock)
                            {
                                textBlock.Opacity = 1;
                                textBlock.Text = number <= model.Size ? number.ToString() : "";
                                if (model.GridValues[i, j] > 0 || number > model.Size)
                                    textBlock.Opacity = 0;
                                else if (!model.ValidInsertionsArray[i, j, number - 1])
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
                lastSetTextBox.Background = Brushes.Beige;
                if (model.IsInfeasible)
                    lastSetTextBox.Background = Brushes.DarkGray;
            }
        }
    }

    private void CreateGrid(Grid gameGrid, int size)
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
                    Action callback = constraintDialogCallbackFactory
                        .CreateButtonCallback(position, size);
                    button.Click += (sender, e) => { callback(); };
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
                Action callback = cellDialogCallbackFactory.CreateCallback(subGrid, (i, j));
                button.Click += (sender, e) => { callback(); };
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
