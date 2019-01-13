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
        public double Border = 0.1;

        private int SmallestSize;
        private GameBoard boardGrid;

        private Point? _dragStart = null;
        private bool _dragState = true;

        private IEnumerable<GameBoard> _savedGames { get; set; }
        private DispatcherTimer _timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            GenerateNewWorld();

            //_savedGames.Add(new GameBoardViewModel { Name = "Mittgame", Width = 100, Height = 200 });
            //_savedGames.Add(new GameBoardViewModel { Name = "2v", Width = 200, Height = 50 });
            ListBoxSavedGames.ItemsSource = _savedGames;
            SetSpeed(1000);
            _timer.Tick += TimerTick;

        }

        public void GenerateNewWorld()
        {
            if (Width <= 0 && Height <= 0)
            {
                new Exception("Could not parse correct dimensions of grid");
                return;
            }
            try
            {
                boardGrid = new GameBoard()
                {
                    Width = int.Parse(WorldWidth.Text),
                    Height = int.Parse(WorldHeight.Text)
                };

                boardGrid.ClearCells();

                UpdateGrid();
            } catch
            {

            }
        }

        public void LoadWorld(GameBoard board)
        {
            boardGrid = board;

            UpdateGrid();
        }

        // TODO: Impove framerate
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

            _dragState = !(boardGrid.GetCell(x, y) == 1 ? true : false);
            _dragStart = point;
            element.CaptureMouse();
        }

        private void WorldGridCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement)sender;
            _dragStart = null;

            element.ReleaseMouseCapture();

            UpdateGrid();
        }

        private void WorldGridCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragStart != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var element = (UIElement)sender;
                var point = e.GetPosition(WorldGridCanvas);

                var x = (int)Math.Floor(point.X / SmallestSize);
                var y = (int)Math.Floor(point.Y / SmallestSize);

                // Prevent index from going outside range
                if ((x >= 0 && x < boardGrid.Width) && (y >= 0 && y < boardGrid.Height))
                {
                    boardGrid.SetCell(x, y, _dragState);
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

            //boardGrid.SaveToDb("Name haina", 10, boardGrid.Generation);

            LoadWorld(world);
        }


        public void Play()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public void SetSpeed(double speed)
        {
            GameSpeed.Value = speed;
            _timer.Interval = TimeSpan.FromMilliseconds(speed);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            GenerateGeneration();
        }
        
        private void GamePlay_Click(object sender, RoutedEventArgs e)
        {
            if (_timer.IsEnabled == false)
            {
                Play();

            }
            else
            {
                Stop();
            }
        }

        private void GameRecord_Click(object sender, RoutedEventArgs e)
        {
            //boardGrid.GetGridFromDb();

            boardGrid.SaveGameToDatabase("Haina", 10, boardGrid.Width, boardGrid.Height, boardGrid.Generation);
            GenerateNewWorld();
        }

        private void GameLoad_Click(object sender, RoutedEventArgs e)
        {

            List<GameBoard> bg = boardGrid.GetSavedGameFromDatabase(10);
            _savedGames = bg;
            LoadWorld(bg[3]);

            //GenerateNewWorld();
            
        }

        private void ListBoxSavedGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Stop();
            ListBox listbox = (ListBox) sender;
            Console.WriteLine(((GameBoard)listbox.SelectedItem).Name);
        }

        private void GameSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetSpeed(GameSpeed.Value);
        }
    }
}
