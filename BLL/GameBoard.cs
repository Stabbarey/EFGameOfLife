﻿using System;
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
        public bool Infinite = false;

        public int Generation { get; private set; }

        DatabaseRepository dr = new DatabaseRepository();

        // Modulus for negative numbers. eg. -1 % 30 should return 29.
        public int Mod(int input, int mod) => (input % mod + mod) % mod;

        public int GetCell(int x, int y)
        {
            if (Infinite == true)
            {
                x = Mod(x, Width);
                y = Mod(y, Height);
            }
            else if ((x >= 0 && x < Width) && (y >= 0 && y < Height))
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
                Generation = Generation+1,
                Infinite = Infinite
            };
            newBoard.ClearCells();

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var current = GetNeighbours(x, y);
                    //Console.WriteLine($"{x} {y} = {current}");

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

        public void SaveGameToDatabase(string name, int gameId, int width, int height, int generations)
        {
            dr.SaveGameToDatabase(name, gameId, width, height, generations);

            Console.WriteLine("Game saved yo!");
        }

        public List<GameBoard> GetSavedGameFromDatabase(int id)
        {

            List<GameBoard> gameBoardList = new List<GameBoard>();

            GameBoardData[] gbd = dr.GetGameBoardDataFromSaveGameID(id);

            SaveGameData saveGameData = dr.GetSavedGameDataFromId(id);

            for (int i = 0; i < gbd.Length; i++)
            {
                string gbData = gbd[i].Grid;
                StringBuilder sb = new StringBuilder(gbData);

                GameBoard gb = new GameBoard
                {
                    Width = saveGameData.Width,
                    Height = saveGameData.Height,
                    Name = saveGameData.Name,
                    Data = sb
                };

                gameBoardList.Add(gb);
            }

            return gameBoardList;
        }
    }
}
