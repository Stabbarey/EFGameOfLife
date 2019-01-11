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
using System.Windows.Threading;
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


        private List<GameBoard> savedGames { get; set; } = new List<GameBoard>();
        DispatcherTimer timer = new DispatcherTimer();

        private bool recording = false;

        public MainWindow()
        {
            InitializeComponent();
            GenerateNewWorld();

            savedGames.Add(new GameBoard { Name = "Mittgame", Width = 100, Height = 200 });
            savedGames.Add(new GameBoard { Name = "2v", Width = 200, Height = 50 });
            ListBoxSavedGames.ItemsSource = savedGames;
            timer.Tick += timer_Tick;

        }

        public void GenerateNewWorld()
        {
            int.TryParse(WorldWidth.Text, out int width);
            int.TryParse(WorldHeight.Text, out int height);

            if (width > 0 && height > 0)
            {
                boardGrid = new GameBoard
                {
                    Width = width,
                    Height = height
                };

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
            SmallestSize = (int)WorldGridCanvas.Height / boardGrid.Height;
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
            Stop();
            //Dispatcher.BeginInvoke(DispatcherPriority.Normal, Stop(), sender);
            var element = (UIElement)sender;
            Point point = e.GetPosition(element);

            var x = (int)Math.Floor(point.X / SmallestSize);
            var y = (int)Math.Floor(point.Y / SmallestSize);

            dragState = !(boardGrid.GetCell(x, y) == 1 ? true : false);
            dragStart = point;
            element.CaptureMouse();
        }

        private void WorldGridCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement)sender;
            dragStart = null;

            element.ReleaseMouseCapture();

            UpdateGrid();
        }

        private void WorldGridCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragStart != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var element = (UIElement)sender;
                var point = e.GetPosition(WorldGridCanvas);

                var x = (int)Math.Floor(point.X / SmallestSize);
                var y = (int)Math.Floor(point.Y / SmallestSize);

                // Prevent index from going outside range
                if ((x >= 0 && x < boardGrid.Width) && (y >= 0 && y < boardGrid.Height))
                {
                    boardGrid.SetCell(x, y, dragState);
                    //UpdateGrid();
                    //Console.WriteLine(x + " " + y);
                }
            }
        }


        private void GameNew_Click(object sender, RoutedEventArgs e)
        {
            GenerateNewWorld();
        }

        private void GenerateGeneration()
        {

            var world = boardGrid.GenerateNextGeneration();

            boardGrid.SaveToDb("Name haina", 10, boardGrid.Generation);

            LoadWorld(world);

        private void GameLoad_Click(object sender, RoutedEventArgs e)
        {

            GameBoard[] bg = boardGrid.GetSavedGameFromDatabase(10);

            LoadWorld(bg[3]);

            //GenerateNewWorld();
            Stop();
        }

        public void Play()
        {

            timer.Interval = TimeSpan.FromMilliseconds(1000);

            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            GenerateGeneration();
        }
        
        private void GamePlay_Click_1(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled == false)
            {
                Play();

            }
            else
            {
                Stop();
            }
        }

        private void Button_RecordGame_Click(object sender, RoutedEventArgs e)
        {
            //boardGrid.GetGridFromDb();
        }

        private void ListBoxSavedGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listbox = (ListBox) sender;
            Console.WriteLine(((GameBoard)listbox.SelectedItem).Name);
        }

        private void GameSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            boardGrid.SaveGameToDatabase("Haina", 10, boardGrid.Width, boardGrid.Height, boardGrid.Generation);

            GenerateNewWorld();
        }
    }
}
