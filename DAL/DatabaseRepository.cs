using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DatabaseRepository
    {
        private bool _isConnected = true;

        public DatabaseRepository()
        {
            using (var db = new BoardDataContext())
            {
                _isConnected = db.Database.Connection.State == System.Data.ConnectionState.Closed;
            }
        }

        public void SaveBoardToDatabase(StringBuilder sb, int gameId, int generation)
        {
            if (!_isConnected)
                return;

            var gridString = sb.ToString();

            using (var db = new BoardDataContext())
            {

                GameBoardData bg = new GameBoardData
                {
                    GameId = gameId,
                    Generation = generation,
                    Grid = gridString
                };

                db.BoardGrid.Add(bg);

                db.SaveChanges();
            }
        }

        public void SaveGameToDatabase(string name, int gameId, int width, int height, int generations)
        {
            if (!_isConnected)
                return;

            using (var db = new BoardDataContext())
            {
                var savedGames = db.SavedGames;

                SaveGameData sg = new SaveGameData
                {
                    Name = name,
                    Width = width,
                    Height = height,
                    BoardGridGameID = gameId,
                    Generations = generations
                };

                db.SavedGames.Add(sg);

                db.SaveChanges();
            }
        }

        public List<GameBoardData> GetGameBoardDataFromSaveGame(SaveGameData saveData)
        {
            List<GameBoardData> gbdList = new List<GameBoardData>();

            if (!_isConnected)
                return gbdList;


            using (var db = new BoardDataContext())
            {
                var gbd = db.BoardGrid;

                foreach (var item in gbd.Where(x => x.GameId == saveData.BoardGridGameID))
                {
                    gbdList.Add(item);
                }


                return gbdList;
            }
        }

        public SaveGameData GetSavedGameDataFromName(string saveName)
        {

            using (var db = new BoardDataContext())
            {
                var sgd = db.SavedGames.Where(x => x.Name == saveName).FirstOrDefault();

                return sgd;
            }
            
        }

        public List<SaveGameData> GetAllSaves()
        {

            List<SaveGameData> gameBoards = new List<SaveGameData>();
            if (_isConnected)
            {
                using (var db = new BoardDataContext())
                {
                    gameBoards = db.SavedGames.ToList();
                }
            }
           
            return gameBoards;
        }

        public int GetGameIdFromDb()
        {
            if (_isConnected)
            {
                using (var db = new BoardDataContext())
                {
                    var sgd = db.BoardGrid.OrderByDescending(x => x.GameId).FirstOrDefault().GameId;

                    return sgd;
                }
            }
            return -1;
        }

        public void DeleteSaveGame(string gameName)
        {
            if (!_isConnected)
                return;

            using (var db = new BoardDataContext())
            {
                var sgd = db.SavedGames.Where(x => x.Name == gameName).FirstOrDefault();

                db.SavedGames.Remove(sgd);

                db.SaveChanges();
            }
        }
   
    }
}
