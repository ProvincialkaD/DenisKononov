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
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Data
        BitmapImage image;
        Image img;
        List<Rectangle> initialUnallocatedParts = new List<Rectangle>();
        List<Rectangle> allocatedParts = new List<Rectangle>();
        #endregion

        #region Ctor
        /// <summary>
        /// Creates a new Window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Randomizes the tiles such that they are not
        /// show in the order they were created
        /// </summary>
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

        /// <summary>
        /// Creates all the puzzles squares, which will either
        /// be a portion of the original image or will be the
        /// single blank puzzle square
        /// </summary>
        private void CreatePuzzleForImage()
        {
            initialUnallocatedParts.Clear();
            allocatedParts.Clear();

            //row0
            CreateImagePart(0, 0, 0.33333, 0.33333);
            CreateImagePart(0.33333, 0, 0.33333, 0.33333);
            CreateImagePart(0.66666, 0, 0.33333, 0.33333);
            //row1
            CreateImagePart(0, 0.33333, 0.33333, 0.33333);
            CreateImagePart(0.33333, 0.33333, 0.33333, 0.33333);
            CreateImagePart(0.66666, 0.33333, 0.33333, 0.33333);
            //row2
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

        /// <summary>
        /// Creates the single blank Rectangle for the puzzle
        /// </summary>
        private void CreateBlankRect()
        {
            Rectangle rectPart = new Rectangle();
            rectPart.Fill = new SolidColorBrush(Colors.White);
            rectPart.Margin = new Thickness(0);
            rectPart.HorizontalAlignment = HorizontalAlignment.Stretch;
            rectPart.VerticalAlignment = VerticalAlignment.Stretch;
            allocatedParts.Add(rectPart);
        }

        /// <summary>
        /// Creates a ImageBrush using x/y/width/height params
        /// to create a Viewbox to only show a portion of original
        /// image. This ImageBrush is then used to fill a Rectangle
        /// which is added to the internal list of unallocated
        /// Rectangles
        /// </summary>
        /// <param name="x">x position in the original image</param>
        /// <param name="y">y position in the original image</param>
        /// <param name="width">the width to use</param>
        /// <param name="height">the hieght to use</param>
        private void CreateImagePart(double x, double y, double width, double height)
        {
            ImageBrush ib = new ImageBrush();
            ib.Stretch = Stretch.UniformToFill;
            ib.ImageSource = image;
            ib.Viewport = new Rect(0, 0, 1.0, 1.0);
            //grab image portion
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

        /// <summary>
        /// Swaps the blank puzzle square with the square clicked if its a
        /// valid move
        /// </summary>
        private void rectPart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //get the source Rectangle, and the blank Rectangle
            //NOTE : Blank Rectangle never moves, its always the last Rectangle
            //in the allocatedParts List, but it gets re-allocated to 
            //different Gri Row/Column
            Rectangle rectCurrent = sender as Rectangle;
            Rectangle rectBlank = allocatedParts[allocatedParts.Count - 1];

            //get current grid row/col for clicked Rectangle and Blank one
            int currentTileRow = (int)rectCurrent.GetValue(Grid.RowProperty);
            int currentTileCol = (int)rectCurrent.GetValue(Grid.ColumnProperty);
            int currentBlankRow = (int)rectBlank.GetValue(Grid.RowProperty);
            int currentBlankCol = (int)rectBlank.GetValue(Grid.ColumnProperty);

            //create possible valid move positions
            List<PossiblePositions> posibilities = new List<PossiblePositions>();
            posibilities.Add(new PossiblePositions
            { Row = currentBlankRow - 1, Col = currentBlankCol });
            posibilities.Add(new PossiblePositions
            { Row = currentBlankRow + 1, Col = currentBlankCol });
            posibilities.Add(new PossiblePositions
            { Row = currentBlankRow, Col = currentBlankCol - 1 });
            posibilities.Add(new PossiblePositions
            { Row = currentBlankRow, Col = currentBlankCol + 1 });

            //check for valid move
            bool validMove = false;
            foreach (PossiblePositions position in posibilities)
                if (currentTileRow == position.Row && currentTileCol == position.Col)
                    validMove = true;

            //only allow valid move
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

        /// <summary>
        /// Allows user to pick a new image to create a puzzle for
        /// </summary>
        private void btnPickImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG" +
                        "|All Files (*.*)|*.*";
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
                    MessageBox.Show("Couldnt load the image file " + ofd.FileName);
                }
            }
        }
        #endregion
    }

    #region PossiblePositions STRUCT
    /// <summary>
    /// Simply struct to store Row/Column data
    /// </summary>
    struct PossiblePositions
    {
        public int Row { get; set; }
        public int Col { get; set; }
    }
    #endregion
}
