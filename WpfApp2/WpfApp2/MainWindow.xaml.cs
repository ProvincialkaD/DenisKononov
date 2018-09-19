using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp2
{
    public partial class MainWindow : Window
    {
        BitmapImage image;
        Image img;
        List<Rectangle> initialUnallocatedParts = new List<Rectangle>();
        List<Rectangle> allocatedParts = new List<Rectangle>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void RandomizeTiles()
        {
            Random rand = new Random();
            int allocated = 0;
            while (allocated != 8)
            {
                int index = 0;
                if (initialUnallocatedParts.Count > 1)
                {
                    index = (int)(rand.NextDouble() * initialUnallocatedParts.Count);
                }
                allocatedParts.Add(initialUnallocatedParts[index]);
                initialUnallocatedParts.RemoveAt(index);
                allocated++;
            }
        }

        private void CreatePuzzleForImage()
        {
            initialUnallocatedParts.Clear();
            allocatedParts.Clear();

            CreateImagePart(0, 0, 0.33333, 0.33333);
            CreateImagePart(0.33333, 0, 0.33333, 0.33333);
            CreateImagePart(0.66666, 0, 0.33333, 0.33333);
            CreateImagePart(0, 0.33333, 0.33333, 0.33333);
            CreateImagePart(0.33333, 0.33333, 0.33333, 0.33333);
            CreateImagePart(0.66666, 0.33333, 0.33333, 0.33333);
            CreateImagePart(0, 0.66666, 0.33333, 0.33333);
            CreateImagePart(0.33333, 0.66666, 0.33333, 0.33333);
            RandomizeTiles();
            CreateBlankRect();

            int index = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    allocatedParts[index].SetValue(Grid.RowProperty, i);
                    allocatedParts[index].SetValue(Grid.ColumnProperty, j);
                    gridMain.Children.Add(allocatedParts[index]);
                    index++;
                }
            }
        }

        private void CreateBlankRect()
        {
            Rectangle rectPart = new Rectangle();
            rectPart.Fill = new SolidColorBrush(Colors.White);
            rectPart.Margin = new Thickness(0);
            rectPart.HorizontalAlignment = HorizontalAlignment.Stretch;
            rectPart.VerticalAlignment = VerticalAlignment.Stretch;
            allocatedParts.Add(rectPart);
        }

       
        private void CreateImagePart(double x, double y, double width, double height)
        {
            ImageBrush ib = new ImageBrush();
            ib.Stretch = Stretch.UniformToFill;
            ib.ImageSource = image;
            ib.Viewport = new Rect(0, 0, 1.0, 1.0);
            ib.Viewbox = new Rect(x, y, width, height);
            ib.ViewboxUnits = BrushMappingMode.RelativeToBoundingBox;
            ib.TileMode = TileMode.None;

            Rectangle rectPart = new Rectangle();
            rectPart.Fill = ib;
            rectPart.Margin = new Thickness(0);
            rectPart.HorizontalAlignment = HorizontalAlignment.Stretch;
            rectPart.VerticalAlignment = VerticalAlignment.Stretch;
            rectPart.MouseDown += new MouseButtonEventHandler(rectPart_MouseDown);
            initialUnallocatedParts.Add(rectPart);
        }

        private void rectPart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rectCurrent = sender as Rectangle;
            Rectangle rectBlank = allocatedParts[allocatedParts.Count - 1];

            int currentTileRow = (int)rectCurrent.GetValue(Grid.RowProperty);
            int currentTileCol = (int)rectCurrent.GetValue(Grid.ColumnProperty);
            int currentBlankRow = (int)rectBlank.GetValue(Grid.RowProperty);
            int currentBlankCol = (int)rectBlank.GetValue(Grid.ColumnProperty);

            List<PossiblePositions> posibilities = new List<PossiblePositions>();
            posibilities.Add(new PossiblePositions
            { Row = currentBlankRow - 1, Col = currentBlankCol });
            posibilities.Add(new PossiblePositions
            { Row = currentBlankRow + 1, Col = currentBlankCol });
            posibilities.Add(new PossiblePositions
            { Row = currentBlankRow, Col = currentBlankCol - 1 });
            posibilities.Add(new PossiblePositions
            { Row = currentBlankRow, Col = currentBlankCol + 1 });

            bool validMove = false;
            foreach (PossiblePositions position in posibilities)
                if (currentTileRow == position.Row && currentTileCol == position.Col)
                    validMove = true;

            if (validMove)
            {
                rectCurrent.SetValue(Grid.RowProperty, currentBlankRow);
                rectCurrent.SetValue(Grid.ColumnProperty, currentBlankCol);

                rectBlank.SetValue(Grid.RowProperty, currentTileRow);
                rectBlank.SetValue(Grid.ColumnProperty, currentTileCol);
            }
            else
                return;
        }

        private void btnPickImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "Тип фото(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG" + "|All Files (*.*)|*.*";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    image = new BitmapImage(new Uri(ofd.FileName, UriKind.RelativeOrAbsolute));
                    img = new Image { Source = image };
                    CreatePuzzleForImage();
                }
                catch
                {
                    MessageBox.Show("Не удалось загрузить фото " + ofd.FileName);
                }
            }
        }
    }

    struct PossiblePositions
    {
        public int Row { get; set; }
        public int Col { get; set; }
    }
}
