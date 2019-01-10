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

        DAL.DatabaseRepository dr = new DAL.DatabaseRepository();

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

            var newBoard = new GameBoard();


            newBoard.Name = Name;
            newBoard.Width = Width;
            newBoard.Height = Height;
            newBoard.Generation = Generation++;
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

        public void SaveToDb()
        {
            Console.WriteLine("Save to db called from GameBoard.cs");

            

            //bool[] baData = new bool[bData.Length];

            //baData[2] = true;

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

            //dr.SaveBoardToDatabase(gridByteData);

            dr.SaveBoardToDatabase(Data);
        }

        public GameBoard GetBoardFromDatabase()
        {

            GameBoard newBoard = new GameBoard();

            StringBuilder sb = new StringBuilder(dr.GetGridDataFromDatabase());

            newBoard.Data = sb;

            newBoard.Width = 4;
            newBoard.Height = 4;

            return newBoard;
        }
    }
}
