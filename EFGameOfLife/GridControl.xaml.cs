using BLL;
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

namespace EFGameOfLife
{
    /// <summary>
    /// Interaction logic for Grid.xaml
    /// </summary>
    public partial class GridControl : UserControl
    {
        public event EventHandler OnRenderChanged;

        public Brush CellAlive = Brushes.Red;
        public Brush CellDead = Brushes.LightSkyBlue;
        public Brush CellVacuum = Brushes.LightBlue;

        private Point? _dragStart = null;
        private bool _dragState = true;
        private GameBoard _dragBoard;

        public GameBoard boardGrid { get; protected set; }

        public double Border = 0;
        private int SmallestSize;

        public int Alive => boardGrid.Alive;
        public int Changes => boardGrid.Changes;
        public int Generation => boardGrid.Generation;

        private int throttleIndex;

        public GridControl()
        {
            InitializeComponent();
        }

        // TODO: Flytta ut
        public void GenerateNewWorld(int width, int height, bool infinite = false)
        {
            try
            {
                boardGrid = new GameBoard()
                {
                    Width = width,
                    Height = height,
                    Infinite = infinite
                };

                if (boardGrid.Width <= 0 && boardGrid.Height <= 0)
                {
                    throw new Exception("Could not parse correct dimensions of grid");
                }

                boardGrid.ClearCells();
                UpdateGrid();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void LoadWorld(GameBoard newBoard)
        {
            if (boardGrid.Width != newBoard.Width || boardGrid.Height != newBoard.Height)
            {
                GenerateNewWorld(newBoard.Width, newBoard.Height);
            }

            UpdateGridChanges(newBoard);
            boardGrid = newBoard;
        }

        private void UpdateGrid()
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

            OnRenderChanged?.Invoke(this, new EventArgs());
        }

        private void UpdateGridChanges(GameBoard newBoard)
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
            boardGrid = newBoard;

            OnRenderChanged?.Invoke(this, new EventArgs());

            //Console.WriteLine($"{newBoard.Alive} cells are alive at generation {newBoard.Generation}!");
            //Console.WriteLine($"UpdateGridChanges preformed {updates} instead of " + boardGrid.Height * boardGrid.Width);
            //Stats.DataContext = new { Alive = newBoard.Alive, Generation = newBoard.Generation, Updates = updates };
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
            //Stop();
            //Dispatcher.BeginInvoke(DispatcherPriority.Normal, Stop(), sender);
            var element = (UIElement)sender;
            Point point = e.GetPosition(element);

            var x = (int)Math.Floor(point.X / SmallestSize);
            var y = (int)Math.Floor(point.Y / SmallestSize);

            _dragState = !(boardGrid.GetCell(x, y) == 1 ? true : false);
            _dragStart = point;
            //_dragBoard = new GameBoard(boardGrid);
            //_dragBoard.ClearCells();
            element.CaptureMouse();
        }

        private void WorldGridCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement)sender;
            _dragStart = null;

            element.ReleaseMouseCapture();
            UpdateGrid();
            //UpdateGridChanges(_dragBoard);
        }

        private void WorldGridCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragStart != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var element = (UIElement)sender;
                var point = e.GetPosition(WorldGridCanvas);

                var x = (int)Math.Floor(point.X / SmallestSize);
                var y = (int)Math.Floor(point.Y / SmallestSize);

                var tempIndex = (y * boardGrid.Width) + x;

                if (throttleIndex == tempIndex)
                    return;

                // Prevent index from going outside range
                if ((x >= 0 && x < boardGrid.Width) && (y >= 0 && y < boardGrid.Height))
                {
                    //UpdateGrid();
                    boardGrid.SetCell(x, y, _dragState);
                    throttleIndex = tempIndex;
                    //UpdateGrid();
                    //_dragBoard.SetCell(x, y, _dragState);
                    //UpdateGridChanges(_dragBoard);
                    //Console.WriteLine(x + " " + y);
                }
            }
        }
    }
}
