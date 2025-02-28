using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;

namespace SkyscraperGameGui
{
    public partial class CellDialog : Window
    {
        private readonly Action<int> digitCallback;

        public CellDialog(Grid cellGrid, Action<int> digitCallback)
        {
            InitializeComponent();
            CloneAndDisplayGrid(cellGrid);
            this.digitCallback = digitCallback;
        }

        private void CloneAndDisplayGrid(Grid cellGrid)
        {
            string gridXaml = XamlWriter.Save(cellGrid);
            StringReader stringReader = new(gridXaml);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            Grid clonedGrid = (Grid)XamlReader.Load(xmlReader);

            clonedGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            clonedGrid.VerticalAlignment = VerticalAlignment.Stretch;
            clonedGrid.Margin = new Thickness(0);

            OptionsGrid.Children.Clear();
            OptionsGrid.Children.Add(clonedGrid);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                int digit = e.Key - Key.D0;
                digitCallback(digit);
            }
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                int digit = e.Key - Key.NumPad0;
                digitCallback(digit);
            }
            Close();
        }
    }
}
