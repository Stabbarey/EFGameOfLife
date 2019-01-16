using System;
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
        private int current;

        public int GameId { get; set; }
        public string Name { get; set; }
        public bool isRecorded { get; set; } = false;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public bool Infinite { get; private set; }

        public List<BoardStringBuilder> Boards { get; set; }

        public BoardStringBuilder PreviousBoard => (current == 0) ? null : Boards[(current - 1)];
        public BoardStringBuilder CurrentBoard => Boards[current];

        public int Alive => CurrentBoard.Alive;

        public GameBoard(int width, int height, bool infinite = false, StringBuilder data = null)
        {
            Width = width;
            Height = height;
            Infinite = infinite;

            Boards = new List<BoardStringBuilder>();
            Boards.Add(new BoardStringBuilder(width, height, false, data));
            current = 0;
        }

        public GameBoard(int id, string name, int width, int height, bool infinite = false, StringBuilder data = null)
            : this(width, height, infinite, data)
        {
            GameId = id;
            Name = name;
        }

        public void Next()
        {
            if (current++ >= Boards.Count)
            {
                current = Boards.Count - 1;
            }
            //current = current + 1 % Boards.Count;
        }

        public void GoToGeneration(int generation)
        {

            current = generation;
        }

        public void GenerateNextGeneration()
        {
            Console.WriteLine("A: " + CurrentBoard.Data.ToString());
            var newBoard = new BoardStringBuilder(Width, Height, Infinite);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int neigbours = CurrentBoard.GetNeighbours(x, y);
                    //Console.WriteLine($"{x} {y} = {current}");

                    switch (neigbours)
                    {
                        case 2:
                            newBoard.SetCell(x, y, CurrentBoard.GetCell(x, y) == 1 ? true : false);
                        break;
                        case 3:
                            newBoard.SetCell(x, y, true);
                        break;
                    }
                }
            }

            Boards.Add(newBoard);
            Next();
            Console.WriteLine("B: " + CurrentBoard.Data.ToString());
        }
    }
}
