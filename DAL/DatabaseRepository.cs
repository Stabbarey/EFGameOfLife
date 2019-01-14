using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
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

                BoardEntity bg = new BoardEntity
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

                GameEntity sg = new GameEntity
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

        public List<BoardEntity> GetGameBoardDataFromSaveGame(GameEntity saveData)
        {
            List<BoardEntity> gbdList = new List<BoardEntity>();

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

        public GameEntity GetSavedGameDataFromName(string saveName)
        {

            using (var db = new BoardDataContext())
            {
                var sgd = db.SavedGames.Where(x => x.Name == saveName).FirstOrDefault();

                return sgd;
            }
            
        }

        public async Task<List<GameEntity>> GetAllSavesAsync()
        {

            List<GameEntity> gameBoards = new List<GameEntity>();
            if (_isConnected)
            {
                using (var db = new BoardDataContext())
                {
                    gameBoards = await db.SavedGames.ToListAsync();
                }
            }

            return gameBoards;
        }

        public List<GameEntity> GetAllSaves()
        {

            List<GameEntity> gameBoards = new List<GameEntity>();
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
