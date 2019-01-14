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
        private int CurrentGameId { get; set; }

        private IEnumerable<GameEntity> _savedGames { get; set; }

        private List<GameBoard> loadedGameBoards = null;

        private DispatcherTimer _timer = new DispatcherTimer();
        private Service service = new Service();

        public MainWindow()
        {
            InitializeComponent();
            GenerateNewWorld();

            //var c_savedGames = await service.GetAllSavesFromDb();

            fetchNewData();
            CurrentGameId = service.GetNextGameId();


            SetSpeed(500);
            _timer.Tick += TimerTick;
        }

        public async void fetchNewData()
        {
            _savedGames = await service.GetAllSavesFromDb();

            ListBoxSavedGames.ItemsSource = _savedGames;

        }

        private void GameNew_Click(object sender, RoutedEventArgs e)
        {
            GenerateNewWorld();
        }

        private void GenerateNewWorld()
        {
            int.TryParse(WorldWidth.Text, out int width);
            int.TryParse(WorldHeight.Text, out int height);
            bool infinite = (bool)WorldInfinite.IsChecked;

            GridControl1.GenerateNewWorld(width, height, infinite);
        }

        private void GenerateGeneration()
        {

            var world = GridControl1.boardGrid.GenerateNextGeneration();

            service.SaveBoardToDatabase(CurrentGameId, world.Generation, world.Data);

            GridControl1.LoadWorld(world);
        }

        /*
         * Timer related methods
         */

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
            service.SaveGameToDatabase("Game_" + CurrentGameId, CurrentGameId, GridControl1.boardGrid.Width, GridControl1.boardGrid.Height, GridControl1.boardGrid.Generation);
            GenerateNewWorld();
        }

        private void ListBoxSavedGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Stop();
            ListBox listbox = (ListBox) sender;

            loadedGameBoards = service.GetSavedGameFromDatabase(((GameEntity)listbox.SelectedItem).Name);

            GridControl1.LoadWorld(loadedGameBoards[0]);
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
                GridControl1.LoadWorld(loadedGameBoards[currentFrame]);

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