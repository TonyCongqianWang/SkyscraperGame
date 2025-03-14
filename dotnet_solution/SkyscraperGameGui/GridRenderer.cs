using SkyscraperGameEngine;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SkyscraperGameGui;

class GridRenderer(
    GridButtonCallbackFactory cellDialogCallbackFactory,
    ConstraintCheckHandler constraintDialogCallbackFactory)
{
    private const int minSize = 4;
    private const int maxSize = 9;

    private GameGridViewModel gameGridViewModel = new();
    private bool showValidNumbers = true;
    private int puzzleSize = 0;

    public void Render(Grid gameGrid, GameObservation observation)
    {
        if (observation.Size < minSize || observation.Size > maxSize)
        {
            RenderInvalidGridPlaceholder(gameGrid, observation);
            return;
        }
        if (puzzleSize != observation.Size)
        {
            puzzleSize = observation.Size;
            CreateGrid(gameGrid);
        }

        for (int i = 0; i < puzzleSize; i++)
        {
            var topConstraintLabel = (Label)gameGrid.FindName($"constr_box_top_{i}");
            if (topConstraintLabel != null)
            {
                var topButton = (Button)gameGrid.FindName($"constr_button_top_{i}");
                if (observation.TopValues[i] > 0)
                {
                    topButton?.SetValue(UIElement.IsEnabledProperty, true);
                    topConstraintLabel.Content = observation.TopValues[i].ToString();
                }
                else
                {
                    topButton?.SetValue(UIElement.IsEnabledProperty, false);
                    topConstraintLabel.Content = "";
                }
                if (!observation.TopValueNeedsCheckArray[i])
                    topConstraintLabel.Foreground = Brushes.LimeGreen;
                else
                    topConstraintLabel.Foreground = Brushes.Orange;
            }

            var bottomConstraintLabel = (Label)gameGrid.FindName($"constr_box_bottom_{i}");
            if (bottomConstraintLabel != null)
            {
                var bottomButton = (Button)gameGrid.FindName($"constr_button_bottom_{i}");
                if (observation.BottomValues[i] > 0)
                {
                    bottomButton?.SetValue(UIElement.IsEnabledProperty, true);
                    bottomConstraintLabel.Content = observation.BottomValues[i].ToString();
                }
                else
                {
                    bottomButton?.SetValue(UIElement.IsEnabledProperty, false);
                    bottomConstraintLabel.Content = "";
                }
                if (!observation.BottomValueNeedsCheckArray[i])
                    bottomConstraintLabel.Foreground = Brushes.LimeGreen;
                else
                    bottomConstraintLabel.Foreground = Brushes.Orange;
            }

            var leftConstraintLabel = (Label)gameGrid.FindName($"constr_box_left_{i}");
            if (leftConstraintLabel != null)
            {
                var leftButton = (Button)gameGrid.FindName($"constr_button_left_{i}");
                if (observation.LeftValues[i] > 0)
                {
                    leftButton?.SetValue(UIElement.IsEnabledProperty, true);
                    leftConstraintLabel.Content = observation.LeftValues[i].ToString();
                }
                else
                {
                    leftButton?.SetValue(UIElement.IsEnabledProperty, false);
                    leftConstraintLabel.Content = "";
                }
                if (!observation.LeftValueNeedsCheckArray[i])
                    leftConstraintLabel.Foreground = Brushes.LimeGreen;
                else
                    leftConstraintLabel.Foreground = Brushes.Orange;
            }

            var rightContraintLabel = (Label)gameGrid.FindName($"constr_box_right_{i}");
            if (rightContraintLabel != null)
            {
                var rightButton = (Button)gameGrid.FindName($"constr_button_right_{i}");
                if (observation.RightValues[i] > 0)
                {
                    rightButton?.SetValue(UIElement.IsEnabledProperty, true);
                    rightContraintLabel.Content = observation.RightValues[i].ToString();
                }
                else
                {
                    rightButton?.SetValue(UIElement.IsEnabledProperty, false);
                    rightContraintLabel.Content = "";
                }
                if (!observation.RightValueNeedsCheckArray[i])
                    rightContraintLabel.Foreground = Brushes.LimeGreen;
                else
                    rightContraintLabel.Foreground = Brushes.Orange;
            }
        }

        for (int i = 0; i < puzzleSize; i++)
        {
            for (int j = 0; j < puzzleSize; j++)
            {
                var cellValueLabel = (Label)gameGrid.FindName($"grid_cell_value_box_{i}_{j}");
                if (cellValueLabel != null)
                {
                    var cellButton = (Button)gameGrid.FindName($"grid_cell_button_{i}_{j}");
                    if (observation.GridValues[i, j] > 0)
                    {
                        cellButton?.SetValue(UIElement.IsEnabledProperty, false);
                        cellValueLabel.Content = observation.GridValues[i, j].ToString();
                        cellValueLabel.Opacity = 1;
                    }
                    else
                    {
                        cellButton?.SetValue(UIElement.IsEnabledProperty, true);
                        cellValueLabel.Content = "";
                        cellValueLabel.Opacity = 0;
                    }
                    if (observation.IsInfeasible)
                        cellButton?.SetValue(UIElement.IsEnabledProperty, false);
                    cellValueLabel.Background = Brushes.White;
                }

                var subGrid = (Grid)gameGrid.FindName($"grid_cellset_box{i}_{j}");
                int subGridSize = (int)Math.Ceiling(Math.Sqrt(puzzleSize));
                if (subGrid != null)
                {
                    if (observation.GridValues[i, j] > 0 || !showValidNumbers)
                        subGrid.Opacity = 0;
                    else
                        subGrid.Opacity = 1;
                    int number = 1;
                    for (int k = 0; k < subGridSize; k++)
                    {
                        for (int l = 0; l < subGridSize && number <= puzzleSize; l++)
                        {
                            if (subGrid.Children
                                .Cast<UIElement>()
                                .FirstOrDefault(e =>
                                    Grid.GetRow(e) == k && Grid.GetColumn(e) == l)
                                is TextBox validNumberBox)
                            {
                                validNumberBox.Text = number <= puzzleSize ? number.ToString() : "";
                                if (!observation.ValidInsertionsArray[i, j, number - 1])
                                    validNumberBox.Foreground = Brushes.LightGray;
                                else
                                    validNumberBox.Foreground = Brushes.Black;
                                number++;
                            }
                        }
                    }
                }
            }
        }
        (int x, int y) = observation.LastSetIndex;
        if (x >= 0 && y >= 0)
        {
            var lastSetLabel = (Label)gameGrid.FindName($"grid_cell_value_box_{x}_{y}");
            if (lastSetLabel != null)
            {
                lastSetLabel.Background = Brushes.Beige;
                if (observation.IsInfeasible)
                    lastSetLabel.Background = Brushes.DarkGray;
            }
        }
    }

    private static void RenderInvalidGridPlaceholder(Grid gameGrid, GameObservation observation)
    {
        ClearGrid(gameGrid);
        gameGrid.Children.Add(new TextBlock
        {
            Text = $"Unable to display puzzle of size {observation.Size}.",
            FontSize = 30,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        });
    }

    private static void ClearGrid(Grid gameGrid)
    {
        gameGrid.RowDefinitions.Clear();
        gameGrid.ColumnDefinitions.Clear();
        gameGrid.Children.Clear();
    }

    private void CreateGrid(Grid gameGrid)
    {
        int totalGridSize = puzzleSize + 2;

        ClearGrid(gameGrid);

        NameScope.SetNameScope(gameGrid, new NameScope());

        for (int i = 0; i < totalGridSize; i++)
        {
            gameGrid.RowDefinitions.Add(new RowDefinition());
            gameGrid.ColumnDefinitions.Add(new ColumnDefinition());
        }

        AddPuzzleConstraintsUi(gameGrid);
        AddPuzzleGridUi(gameGrid);
    }

    private void AddPuzzleGridUi(Grid gameGrid)
    {
        Grid puzzleCellGrid = new()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Margin = new Thickness(0),
            ShowGridLines = false
        };

        for (int i = 0; i < puzzleSize; i++)
        {
            puzzleCellGrid.RowDefinitions.Add(new RowDefinition());
            puzzleCellGrid.ColumnDefinitions.Add(new ColumnDefinition());
        }

        for (int i = 0; i < puzzleSize; i++)
        {
            for (int j = 0; j < puzzleSize; j++)
            {
                AddCellUiElements(gameGrid, puzzleCellGrid, i, j);
            }
        }

        Border innerGridBorder = new()
        {
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(2),
            Child = puzzleCellGrid
        };

        gameGrid.Children.Add(innerGridBorder);
        Grid.SetRow(innerGridBorder, 1);
        Grid.SetColumn(innerGridBorder, 1);
        Grid.SetRowSpan(innerGridBorder, puzzleSize);
        Grid.SetColumnSpan(innerGridBorder, puzzleSize);
    }

    private void AddCellUiElements(Grid gameGrid, Grid innerGrid, int i, int j)
    {
        Grid subGrid = AddValidNumbersUi(gameGrid, puzzleSize, innerGrid, i, j);
        Action buttonCallback = cellDialogCallbackFactory.CreateCallback(subGrid, (i, j));
        AddGridValueUi(gameGrid, puzzleSize, i, j);
        AddCellDialogButton(gameGrid, innerGrid, i, j, buttonCallback);
    }

    private static void AddCellDialogButton(Grid gameGrid, Grid innerGrid, int i, int j, Action callback)
    {
        Button button = new()
        {
            Name = $"grid_cell_button_{i}_{j}",
            Opacity = 0,
            Background = Brushes.Transparent
        };
        button.Click += (sender, e) => { callback(); };
        innerGrid.Children.Add(button);
        Grid.SetRow(button, i);
        Grid.SetColumn(button, j);
        gameGrid.RegisterName(button.Name, button);
    }

    private static void AddGridValueUi(Grid gameGrid, int size, int i, int j)
    {
        double cellSize = Math.Min(gameGrid.Width, gameGrid.Height) / size;
        Label label = new()
        {
            Name = $"grid_cell_value_box_{i}_{j}",
            Content = "",
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
        gameGrid.Children.Add(label);
        Grid.SetRow(label, i + 1);
        Grid.SetColumn(label, j + 1);
        gameGrid.RegisterName(label.Name, label);
    }

    private Grid AddValidNumbersUi(Grid gameGrid, int size, Grid innerGrid, int i, int j)
    {
        int subGridSize = (int)Math.Ceiling(Math.Sqrt(puzzleSize));
        Grid subGrid = new()
        {
            Name = $"grid_cellset_box{i}_{j}",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(5)
        };

        for (int k = 0; k < subGridSize; k++)
        {
            subGrid.RowDefinitions.Add(new RowDefinition());
            subGrid.ColumnDefinitions.Add(new ColumnDefinition());
        }

        int number = 1;
        for (int k = 0; k < subGridSize; k++)
        {
            for (int l = 0; l < subGridSize && number <= size; l++)
            {
                TextBox box = new()
                {
                    Focusable = false,
                    BorderBrush = Brushes.Transparent,
                    Text = number.ToString(),
                    FontSize = 13,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Margin = new Thickness(-1),
                    Foreground = Brushes.Black,
                    Opacity = 1
                };
                subGrid.Children.Add(box);
                Grid.SetRow(box, k);
                Grid.SetColumn(box, l);
                number++;
            }
        }

        innerGrid.Children.Add(subGrid);
        Grid.SetRow(subGrid, i);
        Grid.SetColumn(subGrid, j);
        gameGrid.RegisterName(subGrid.Name, subGrid);
        return subGrid;
    }

    private void AddPuzzleConstraintsUi(Grid gameGrid)
    {
        for (int j = 0; j < puzzleSize; j++)
        {
            string position = $"_top_{j}";
            AddConstraintUiElements(gameGrid, 0, j + 1, position);
        }

        for (int j = 0; j < puzzleSize; j++)
        {
            string position = $"_bottom_{j}";
            AddConstraintUiElements(gameGrid, puzzleSize + 1, j + 1, position);
        }

        for (int i = 0; i < puzzleSize; i++)
        {
            string position = $"_left_{i}";
            AddConstraintUiElements(gameGrid, i + 1, 0, position);
        }

        for (int i = 0; i < puzzleSize; i++)
        {
            string position = $"_right_{i}";
            AddConstraintUiElements(gameGrid, i + 1, puzzleSize + 1, position);
        }
    }

    private void AddConstraintUiElements(Grid gameGrid, int i, int j, string positionStr)
    {
        Action buttonCallback = constraintDialogCallbackFactory.CreateButtonCallback(positionStr, puzzleSize);
        Label label = new()
        {
            Name = $"constr_box{positionStr}",
            Content = "0",
            FontSize = 20,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(2)
        };

        Button button = new()
        {
            Name = $"constr_button{positionStr}",
            Opacity = 0,
            Background = Brushes.Transparent
        };
        button.Click += (sender, e) => { buttonCallback(); };

        gameGrid.Children.Add(label);
        Grid.SetRow(label, i);
        Grid.SetColumn(label, j);
        gameGrid.RegisterName(label.Name, label);

        gameGrid.Children.Add(button);
        Grid.SetRow(button, i);
        Grid.SetColumn(button, j);
        gameGrid.RegisterName(button.Name, button);
    }
}
