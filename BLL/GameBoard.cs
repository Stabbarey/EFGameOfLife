using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace BLL
{
    public class GameBoard
    {

        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; set; }
        public StringBuilder Data { get; private set; }

        public int Generation { get; private set; }

        DatabaseRepository dr = new DatabaseRepository();

        public int GetCell(int x, int y)
        {
            if ((x >= 0 && x < Width) && (y >= 0 && y < Height))
            {
                return Data[(y * Width) + x] == '1' ? 1 : 0;
            }
            return 0;
        }

        public void ClearCells()
        {
            Data = new StringBuilder(Width * Height);
            Data.Insert(0, "0", Width * Height);
        }

        public void SetCell(int x, int y, bool value)
        {
            int position = (y * Width) + x;
            Data.Remove(position, 1);
            Data.Insert(position, value == true ? "1" : "0");
        }

        public int GetNeighbours(int x, int y)
        {
            return
                GetCell(x + 1, y + 1) + GetCell(x + 1, y + 0) + GetCell(x + 1, y - 1) +
                GetCell(x + 0, y + 1)                         + GetCell(x + 0, y - 1) +
                GetCell(x - 1, y + 1) + GetCell(x - 1, y + 0) + GetCell(x - 1, y - 1);
        }

        public GameBoard GenerateNextGeneration()
        {

            var newBoard = new GameBoard
            {
                Name = Name,
                Width = Width,
                Height = Height,
                Generation = Generation++
            };
            newBoard.ClearCells();

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var current = GetNeighbours(x, y);
                    //Console.WriteLine(x + " " + y + ": " + current);

                    switch (current)
                    {
                        case 2:
                            newBoard.SetCell(x, y, GetCell(x, y) == 1 ? true : false);
                        break;
                        case 3:
                            newBoard.SetCell(x, y, true);
                        break;
                    }
                }
            }
            return newBoard;
        }

        public void SaveToDb(string name, int gameId, int generation)
        {
            //Save the name to a listbox or something
            dr.SaveBoardToDatabase(Data, gameId, generation);
        }

        public GameBoard GetSavedGameFromDatabase(int id)
        {

            SaveGameData sgd = dr.GetSavedGameFromId(1);
            GameBoardData[] gridData = dr.GetGridDataFromSavedGame(1);


            string gbData = gridData[0].Grid;
            StringBuilder sb = new StringBuilder(gbData);

            GameBoard newBoard = new GameBoard
            {
                Width = (int)4,
                Height = (int)4,
                Data = sb
            };

            return newBoard;
        }

        public GameBoard GetBoardFromDatabase()
        {

            //GEt saved game data and get it's generations

            //string gbData = bg[0].Grid;

            //GameBoard newBoard = new GameBoard();

            ////StringBuilder sb = new StringBuilder(gbData);

            //newBoard.Width = 4;
            //newBoard.Height = 4;
            //newBoard.Data = sb;

            //return newBoard;

            return null;
        }
    }
}
