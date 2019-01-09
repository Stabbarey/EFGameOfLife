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

        public int GetNeighbours(int x, int y)
        {

            
            bool n1 = Grid[x + 1, y + 1];
            bool n2 = Grid[x + 1, y - 1];
            bool n3 = Grid[x - 1, y + 1];
            bool n4 = Grid[x - 1, y - 1];
            

            int int1 = Convert.ToInt32(n1);
            int int2 = Convert.ToInt32(n2);
            int int3 = Convert.ToInt32(n3);
            int int4 = Convert.ToInt32(n4);

            
            return int1 + int2 + int3 + int4;

        }

        public void GenerateNextGeneration()
        {

            Generation++;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {

                    var current = Grid[x, y];
                    Console.WriteLine(x + " " + y + ": " + current);
                }
            }

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
