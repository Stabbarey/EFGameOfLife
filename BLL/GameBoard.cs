using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public int Generation { get; private set; }

        public bool[,] Grid;

        public int GetCell(int x, int y)
        {
            if ((x >= 0 && x < Width) && (y >= 0 && y < Height))
            {
                return Grid[x, y] == true ? 1 : 0;
            }
            return 0;
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

            var board = new GameBoard();

            board.Name = Name;
            board.Width = Width;
            board.Height = Height;
            board.Generation = Generation++;
            board.Grid = new bool[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var current = GetNeighbours(x, y);
                    //Console.WriteLine(x + " " + y + ": " + current);

                    switch (current)
                    {
                        case 2:
                            board.Grid[x, y] = GetCell(x, y) == 1 ? true : false;
                        break;
                        case 3:
                            board.Grid[x, y] = true;
                        break;
                    }
                }
            }
            return board;
        }

        public void SaveToDb()
        {
            Console.WriteLine("Save to db called from GameBoard.cs");

            DatabaseRepository dr = new DatabaseRepository();


            bool[,] bData = Grid;

            bool[] baData = new bool[bData.Length];

            baData[2] = true;

            //BitArray bits = new BitArray(baData);
            //byte[] Bytes = new byte[1];
            //bits.CopyTo(Bytes, 0);

            //Console.WriteLine("GridByteData " + gridByteData);

            //dr.SaveBoardToDatabase(gridByteData);

            //for (int i = 0; i < baData.Length; i++)
            //{
            //    Convert.ToByte(baData[i]);
            //}

            //Buffer.BlockCopy(bData, 0, baData, 0, bData.Length);

            BoardGrid bg = new BoardGrid
            {
                Id = 1,
                Grid = 0x00005,
            };

            try
            {
                dr.SaveBoardToDatabase(bg);
            }
            catch (Exception e)
            {
                throw e;
            }


        }



    }
}
