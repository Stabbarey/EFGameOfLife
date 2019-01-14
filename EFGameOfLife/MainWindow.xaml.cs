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
using DAL;

namespace EFGameOfLife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public double Border = 0;
        public Brush CellAlive = Brushes.Red;
        public Brush CellDead = Brushes.LightBlue;
        public Brush CellVacuum = Brushes.SkyBlue;

        private int CurrentGameId { get; set; }

        private int SmallestSize;
        private GameBoard boardGrid;

        private Point? _dragStart = null;
        private bool _dragState = true;

        private IEnumerable<SaveGameData> _savedGames { get; set; }

        private List<GameBoard> loadedGameBoards = null;

        private DispatcherTimer _timer = new DispatcherTimer();
        private Service service = new Service();

        public MainWindow()
        {
            InitializeComponent();
            GenerateNewWorld();

            _savedGames = service.GetAllSavesFromDb();

            CurrentGameId = service.GetNextGameId();

            ListBoxSavedGames.ItemsSource = _savedGames;

            SetSpeed(1000);

            _timer.Tick += TimerTick;
        }

        public void GenerateNewWorld()
        {
            try
            {
                boardGrid = new GameBoard()
                {
                    Width = int.Parse(WorldWidth.Text),
                    Height = int.Parse(WorldHeight.Text),
                    Infinite = (bool) WorldInfinite.IsChecked
                };

                if (boardGrid.Width <= 0 && boardGrid.Height <= 0)
                {
                    throw new Exception("Could not parse correct dimensions of grid");
                }

                Stats.DataContext = new { Alive = 0, Generation = 0, Updates = 0 };

                boardGrid.ClearCells();

                UpdateGrid();
            } catch (Exception e)
            {
                throw e;
            }
        }

        public void LoadWorld(GameBoard newBoard)
        {
            if (boardGrid.Width != newBoard.Width || boardGrid.Height != newBoard.Height)
            {
                WorldWidth.Text = newBoard.Width.ToString();
                WorldHeight.Text = newBoard.Height.ToString();

                GenerateNewWorld();
            }
            UpdateGridChanges(newBoard);
            boardGrid = newBoard;
        }

        public void UpdateGrid()
        {
            SmallestSize = WorldGridCanvas.Width <= WorldGridCanvas.Height ? (int)WorldGridCanvas.Width / boardGrid.Width : (int)WorldGridCanvas.Height / boardGrid.Height;
            //SmallestSize = (int)WorldGridCanvas.Height / boardGrid.Height;

            WorldGridCanvas.Children.Clear();

            for (int x = 0; x < boardGrid.Width; x++)
            {
                for (int y = 0; y < boardGrid.Height; y++)
                {
                    var color = boardGrid.GetCell(x, y) == 1 ? CellAlive : CellVacuum;
                    RenderCell(x, y, color);
                }
            }
        }

        public void UpdateGridChanges(GameBoard newBoard)
        {
            // If the length differs, abort
            if (boardGrid.Data.Length != newBoard.Data.Length)
                return;

            // If the data is the same there then there is no need to check for changes
            if (boardGrid.Data == newBoard.Data)
                return;

            int updates = 0;

            for (int i = 0; i < boardGrid.Data.Length; i++)
            {
                if (boardGrid.Data[i] != newBoard.Data[i])
                {
                    boardGrid.GetCoords(i, out int x, out int y);

                    var color = newBoard.GetCell(x, y) == 1 ? CellAlive : CellDead;

                    //WorldGridCanvas.Children.RemoveAt();
                    RenderCell(x, y, color);
                    //Console.WriteLine($"Update frame {updates}: {x} {y}");
                    updates++;
                }
            }
            //Console.WriteLine($"{newBoard.Alive} cells are alive at generation {newBoard.Generation}!");
            //Console.WriteLine($"UpdateGridChanges preformed {updates} instead of " + boardGrid.Height * boardGrid.Width);
            Stats.DataContext = new { Alive = newBoard.Alive, Generation = newBoard.Generation, Updates = updates };
        }

        private void RenderCell(int x, int y, Brush color = null)
        {
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
                    //UpdateGrid();
                    boardGrid.SetCell(x, y, _dragState);
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

            service.SaveBoardToDatabase(CurrentGameId, 8989, world.Data);

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
            service.SaveGameToDatabase("Game_" + CurrentGameId, CurrentGameId, boardGrid.Width, boardGrid.Height, boardGrid.Generation);
            GenerateNewWorld();
        }

        private void GameLoad_Click(object sender, RoutedEventArgs e)
        {
            

            loadedGameBoards = service.GetSavedGameFromDatabase("Game_2");

            LoadWorld(loadedGameBoards[0]);

            MessageBox.Show(loadedGameBoards[0].Name + " loaded...");

        }

        private void ListBoxSavedGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Stop();
            ListBox listbox = (ListBox) sender;

            loadedGameBoards = service.GetSavedGameFromDatabase(((SaveGameData)listbox.SelectedItem).Name);

            LoadWorld(loadedGameBoards[0]);

            //Console.WriteLine(((GameBoard)listbox.SelectedItem).Name);
        }

        private void GameSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetSpeed(GameSpeed.Value);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Stop();
            _timer.Tick -= TimerTick;
        }

        int currentFrame = 0;
        private void Button_playRecord_Click_1(object sender, RoutedEventArgs e)
        {
            if (loadedGameBoards != null)
            {
                LoadWorld(loadedGameBoards[currentFrame]);

                currentFrame++;

                if (currentFrame > loadedGameBoards.Count - 1)
                {
                    MessageBox.Show("Recording done...");
                    loadedGameBoards = null;
                    currentFrame = 0;
                    return;
                }
            }
            else
            {
                MessageBox.Show("No savegame selected...");
            }
        }

        private void Button_RemoveGame_Click(object sender, RoutedEventArgs e)
        {

            if (ListBoxSavedGames.SelectedIndex != -1)
            {
                //ListBox listbox = ListBoxSavedGames;

                //boardGrid.RemoveGameFromDatabase(((SaveGameData)listbox.SelectedItem).Name);

                //ListBoxSavedGames.ItemsSource = _savedGames;
            }

        }
    }
}


//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
//using System.Windows.Threading;
//using BLL;

//namespace EFGameOfLife
//{
//    /// <summary>
//    /// Interaction logic for MainWindow.xaml
//    /// </summary>
//    public partial class MainWindow : Window
//    {
//        public double Border = 0.1;

//        private int SmallestSize;
//        private GameBoard boardGrid;

//        private Point? _dragStart = null;
//        private bool _dragState = true;

//        private int CurrentGameId { get; set; }

//        private List<string> _savedGames { get; set; } = new List<string>();
//        private DispatcherTimer _timer = new DispatcherTimer();

//        private List<GameBoard> loadedGameBoards = null;

//        public MainWindow()
//        {
//            InitializeComponent();
//            GenerateNewWorld();

//            CurrentGameId = boardGrid.GetNextGameId();

//            _savedGames.AddRange(boardGrid.GetAllSavedBoardNames());

//            ListBoxSavedGames.ItemsSource = _savedGames;

//            SetSpeed(1000);
//            _timer.Tick += TimerTick;
//        }

//        public void GenerateNewWorld()
//        {
//            if (Width <= 0 && Height <= 0)
//            {
//                new Exception("Could not parse correct dimensions of grid");
//                return;
//            }
//            try
//            {
//                boardGrid = new GameBoard
//                {
//                    Width = int.Parse(WorldWidth.Text),
//                    Height = int.Parse(WorldHeight.Text)
//                };

//                boardGrid.ClearCells();

//                UpdateGrid();
//            }
//            catch
//            {

//            }
//        }

//        public void LoadWorld(GameBoard board)
//        {
//            boardGrid = board;

//            UpdateGrid();
//        }

//        // TODO: Impove framerate
//        public void UpdateGrid()
//        {
//            SmallestSize = (int)WorldGridCanvas.Height / boardGrid.Height;
//            WorldGridCanvas.Children.Clear();

//            for (int x = 0; x < boardGrid.Width; x++)
//            {
//                for (int y = 0; y < boardGrid.Height; y++)
//                {
//                    var color = boardGrid.GetCell(x, y) == 1 ? Brushes.Red : Brushes.SkyBlue;
//                    var rectangle = new Rectangle
//                    {
//                        Stroke = Brushes.Black,
//                        StrokeThickness = Border,
//                        Fill = color,
//                        Height = SmallestSize,
//                        Width = SmallestSize,
//                    };
//                    Canvas.SetTop(rectangle, SmallestSize * y);
//                    Canvas.SetLeft(rectangle, SmallestSize * x);
//                    WorldGridCanvas.Children.Add(rectangle);
//                }
//            }
//        }

//        private void WorldGridCanvas_MouseDown(object sender, MouseButtonEventArgs e)
//        {
//            Stop();
//            //Dispatcher.BeginInvoke(DispatcherPriority.Normal, Stop(), sender);
//            var element = (UIElement)sender;
//            Point point = e.GetPosition(element);

//            var x = (int)Math.Floor(point.X / SmallestSize);
//            var y = (int)Math.Floor(point.Y / SmallestSize);

//            _dragState = !(boardGrid.GetCell(x, y) == 1 ? true : false);
//            _dragStart = point;
//            element.CaptureMouse();
//        }

//        private void WorldGridCanvas_MouseUp(object sender, MouseButtonEventArgs e)
//        {
//            var element = (UIElement)sender;
//            _dragStart = null;

//            element.ReleaseMouseCapture();

//            UpdateGrid();
//        }

//        private void WorldGridCanvas_MouseMove(object sender, MouseEventArgs e)
//        {
//            if (_dragStart != null && e.LeftButton == MouseButtonState.Pressed)
//            {
//                var element = (UIElement)sender;
//                var point = e.GetPosition(WorldGridCanvas);

//                var x = (int)Math.Floor(point.X / SmallestSize);
//                var y = (int)Math.Floor(point.Y / SmallestSize);

//                // Prevent index from going outside range
//                if ((x >= 0 && x < boardGrid.Width) && (y >= 0 && y < boardGrid.Height))
//                {
//                    boardGrid.SetCell(x, y, _dragState);
//                    //UpdateGrid();
//                    //Console.WriteLine(x + " " + y);
//                }
//            }
//        }


//        private void GameNew_Click(object sender, RoutedEventArgs e)
//        {
//            GenerateNewWorld();
//        }

//        private void GenerateGeneration()
//        {

//            var world = boardGrid.GenerateNextGeneration();

//            boardGrid.SaveBoardToDatabase(CurrentGameId);

//            LoadWorld(world);
//        }


//        public void Play()
//        {
//            _timer.Start();
//        }

//        public void Stop()
//        {
//            _timer.Stop();
//        }

//        public void SetSpeed(double speed)
//        {
//            GameSpeed.Value = speed;
//            _timer.Interval = TimeSpan.FromMilliseconds(speed);
//        }

//        private void TimerTick(object sender, EventArgs e)
//        {
//            GenerateGeneration();
//        }

//        private void GamePlay_Click(object sender, RoutedEventArgs e)
//        {
//            if (_timer.IsEnabled == false)
//            {
//                Play();

//            }
//            else
//            {
//                Stop();
//            }
//        }

//        private void GameRecord_Click(object sender, RoutedEventArgs e)
//        {
//            //boardGrid.GetGridFromDb();

//            boardGrid.SaveGameToDatabase("Game_" + CurrentGameId, CurrentGameId, boardGrid.Width, boardGrid.Height, boardGrid.Generation);
//            GenerateNewWorld();
//        }

//        private void GameLoad_Click(object sender, RoutedEventArgs e)
//        {
//            loadedGameBoards = boardGrid.GetSavedGameFromDatabase("Game_2");

//            LoadWorld(loadedGameBoards[0]);


//            MessageBox.Show(loadedGameBoards[0].Name + " loaded...");
//        }

//        int currentFrame = 0;
//        private void Button_playRecord_Click(object sender, RoutedEventArgs e)
//        {
//            if (loadedGameBoards != null)
//            {
//                LoadWorld(loadedGameBoards[currentFrame]);

//                currentFrame++;

//                if (currentFrame > loadedGameBoards.Count - 1)
//                {
//                    MessageBox.Show("Recording done...");
//                    loadedGameBoards = null;
//                    return;

//                }
//            }
//            else
//            {
//                MessageBox.Show("Can't play blablabla...");
//            }


//        }

//        private void GameSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
//        {
//            SetSpeed(GameSpeed.Value);
//        }


//        private void ListBoxSavedGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            Stop();
//            //ListBox listbox = (ListBox) sender;
//            //Console.WriteLine(((GameBoard)listbox.SelectedItem).Name);

//            try
//            {
//                //byt detta mot vad som står i textfältet på denna lsitbox
//                // boardGrid.GetSavedGameFromDatabase("Game_1");
//            }
//            catch
//            {
//                MessageBox.Show("Couldn't get any save game with dat name");
//            }
//        }
//    }
//}
