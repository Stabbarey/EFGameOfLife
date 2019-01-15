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
    /// Interaction logic for GridBitmapControl.xaml
    /// </summary>
    public partial class GridBitmapControl : UserControl
    {

        public GameBoard boardGrid;

        public GridBitmapControl()
        {
            InitializeComponent();
        }

        public void NewWorld(int width, int height, bool infinite = false)
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
                //UpdateGrid();
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
                NewWorld(newBoard.Width, newBoard.Height);
            }

            //UpdateGridChanges(newBoard);
            boardGrid = newBoard;
        }
    }
}
