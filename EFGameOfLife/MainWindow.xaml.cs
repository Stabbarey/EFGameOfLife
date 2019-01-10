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
using BLL;

namespace EFGameOfLife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public double Border = 0.2;

        private int SmallestSize;
        private GameBoard boardGrid;

        private Point? dragStart = null;
        private bool dragState = true;

        public MainWindow()
        {
            InitializeComponent();
            GenerateNewWorld();
        }

        public void GenerateNewWorld()
        {
            int.TryParse(WorldWidth.Text, out int width);
            int.TryParse(WorldHeight.Text, out int height);

            if (width > 0 && height > 0)
            {
                boardGrid = new GameBoard();
                boardGrid.Width = width;
                boardGrid.Height = height;

                boardGrid.ClearCells();

                UpdateGrid();
            }
            else
            {
                new Exception("Could not parse correct dimensions of grid");
            }
        }

        public void LoadWorld(GameBoard board)
        {
            boardGrid = board;

            UpdateGrid();
        }

        public void UpdateGrid()
        {
            SmallestSize = (int) WorldGridCanvas.Height / boardGrid.Height;
            WorldGridCanvas.Children.Clear();

            for (int x = 0; x < boardGrid.Width; x++)
            {
                for (int y = 0; y < boardGrid.Height; y++)
                {
                    var color = boardGrid.GetCell(x, y) == 1 ? Brushes.Red : Brushes.SkyBlue;
                    var rectangle = new Rectangle
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = Border,
                        Fill = color,
                        Height = SmallestSize,
                        Width = SmallestSize,
                    };
                    Canvas.SetTop(rectangle, SmallestSize * y);
                    Canvas.SetLeft(rectangle, SmallestSize * x);
                    WorldGridCanvas.Children.Add(rectangle);
                }
            }
        }

        private void WorldGridCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement) sender;
            Point point = e.GetPosition(element);

            var x = (int) Math.Floor(point.X / SmallestSize);
            var y = (int) Math.Floor(point.Y / SmallestSize);

            dragState = !(boardGrid.GetCell(x, y) == 1 ? true : false);
            dragStart = point;

            element.CaptureMouse();
        }

        private void WorldGridCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement) sender;
            dragStart = null;

            element.ReleaseMouseCapture();
            UpdateGrid();
        }

        private void WorldGridCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragStart != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var element = (UIElement) sender;
                var point = e.GetPosition(WorldGridCanvas);

                var x = (int) Math.Floor(point.X / SmallestSize);
                var y = (int) Math.Floor(point.Y / SmallestSize);

                // Prevent index from going outside range
                if ((x >= 0 && x < boardGrid.Width) && (y >= 0 && y < boardGrid.Height))
                {
                    boardGrid.SetCell(x, y, dragState);
                    //UpdateGrid();
                    //Console.WriteLine(x + " " + y);
                }
            }
        }

        private void GameRecord_Click(object sender, RoutedEventArgs e)
        {
            boardGrid.SaveToDb();

            var world = boardGrid.GenerateNextGeneration();
            LoadWorld(world);

            
        }

        private void GameNew_Click(object sender, RoutedEventArgs e)
        {
            GenerateNewWorld();
        }

        private void GetGridButton_Click(object sender, RoutedEventArgs e)
        {
            boardGrid.GetGridFromDb();
        }
    }
}
