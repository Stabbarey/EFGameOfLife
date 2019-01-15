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
        private IEnumerable<GameEntity> _savedGames { get; set; }

        private List<GameBoard> loadedGameBoards = null;

        private DispatcherTimer _timer = new DispatcherTimer();
        private Service service = new Service();

        private bool recording = false;
        private bool loadedGame = false;
        private int currentFrame = 0;

        public MainWindow()
        {
            InitializeComponent();
            GenerateNewWorld();

            FetchSavedGamesAsync();

            SetSpeed(500);
            _timer.Tick += TimerTick;
        }

        public async void FetchSavedGamesAsync()
        {
            _savedGames = await service.GetAllSavesFromDb();
            ListBoxSavedGames.ItemsSource = _savedGames;
        }

        private void GenerateNewWorld()
        {
            GameRecord.IsEnabled = true;
            ListBoxSavedGames.SelectedIndex = -1;
            loadedGame = false;
            recording = false;

            int.TryParse(WorldWidth.Text, out int width);
            int.TryParse(WorldHeight.Text, out int height);
            bool infinite = (bool)WorldInfinite.IsChecked;

            GridControl1.GenerateNewWorld(width, height, infinite);

            UpdatePlayButton();
        }

        private async void GenerateGeneration()
        {
            var world = GridControl1.boardGrid.GenerateNextGeneration();

            if (recording)
                await service.SaveBoardToDatabaseAsync(world);

            GridControl1.LoadWorld(world);
        }

        public void UpdatePlayButton()
        {
            string state = _timer.IsEnabled ? "Pause" : "Play";
            GamePlay.Content = recording  || ListBoxSavedGames.SelectedIndex > 0 ? state + " recording" : state + " game";

        }

        private void PlayRecording()
        {
            if (loadedGameBoards.Count > 0)
            {
                GridControl1.LoadWorld(loadedGameBoards[currentFrame]);

                currentFrame++;

                if (currentFrame >= loadedGameBoards.Count)
                {
                    MessageBox.Show("Recording done...");
                    loadedGameBoards.Clear();
                    currentFrame = 0;
                    GenerateNewWorld();
                    Stop();
                }
            }
        }

        #region Timer related methods


        public void Play()
        {
            _timer.Start();
            UpdatePlayButton();
        }

        public void Stop()
        {
            _timer.Stop();
            UpdatePlayButton();
        }

        public void SetSpeed(double speed)
        {
            GameSpeed.Value = speed;
            _timer.Interval = TimeSpan.FromMilliseconds(speed);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (loadedGame)
            {
                PlayRecording();
            }
            else
            {
                GenerateGeneration();
            }
        }

        #endregion

        #region Events

        private void GameNew_Click(object sender, RoutedEventArgs e)
        {
            GenerateNewWorld();
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
            if (recording)
            {
                FetchSavedGamesAsync();
            }
        }

        private async void GameRecord_Click(object sender, RoutedEventArgs e)
        {

            if (recording)
            {
                recording = false;
                FetchSavedGamesAsync();
                textBox_saveDataName.Text = "";
            }
            else
            {
                if (textBox_saveDataName.Text != "")
                {
                    await service.SaveGameToDatabaseAsync(textBox_saveDataName.Text, GridControl1.boardGrid);
                    await service.SaveBoardToDatabaseAsync(GridControl1.boardGrid);
                    recording = true;
                    
                } else
                {
                    MessageBox.Show("Choose a name for your recording");
                    textBox_saveDataName.Focus();
                }
            }
            
            GameRecord.Content = recording ? "Stop recording" : "Record new game";
            GameRecord.Background = recording ? Brushes.Red : Brushes.LightGray;
            UpdatePlayButton();
        }

        private async void ListBoxSavedGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (ListBoxSavedGames.SelectedIndex != -1)
            {
                Stop();

                loadedGameBoards = await service.GetSavedGameFromDatabaseAsync((GameEntity) ListBoxSavedGames.SelectedItem);
                loadedGame = true;

                GameRecord.IsEnabled = false;

                if (loadedGameBoards.Count > 0)
                    GridControl1.LoadWorld(loadedGameBoards[0]);
            }

            UpdatePlayButton();
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

        private async void Button_RemoveGame_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxSavedGames.SelectedIndex != -1)
            {
                ListBox listbox = ListBoxSavedGames;
                await service.DeleteSaveGameAsync((GameEntity) listbox.SelectedItem);

                FetchSavedGamesAsync();
            }
        }

        #endregion
    }
}