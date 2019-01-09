using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            DAL.DatabaseRepository dr = new DAL.DatabaseRepository();


            //bool[,] bData = Grid;

            //bool[] baData = new bool[bData.Length];

            ////byte[] gridByteData = ConvertBoolArrayToByteArray(baData);

            //for (int i = 0; i < baData.Length; i++)
            //{
            //    Convert.ToByte(baData[i]);
            //}

            //Buffer.BlockCopy(bData, 0, baData, 0, bData.Length);

            //dr.SaveBoardToDatabase(gridByteData);

           
        }


        //private static bool[] ConvertByteToBoolArray(byte b)
        //{
        //    // prepare the return result
        //    bool[] result = new bool[8];

        //    // check each bit in the byte. if 1 set to true, if 0 set to false
        //    for (int i = 0; i < 8; i++)
        //        result[i] = (b & (1 << i)) == 0 ? false : true;

        //    // reverse the array
        //    Array.Reverse(result);

        //    return result;
        //}

    }
}
