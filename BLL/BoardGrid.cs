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

    }
}
