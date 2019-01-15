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
        private bool _isConnected = false;

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

        public async Task<int> SaveBoardToDatabaseAsync(StringBuilder sb, int gameId, int generation)
        {
            if (!_isConnected)
                return 0;

            int changes = 0;
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

                changes = await db.SaveChangesAsync();
            }
            return changes;
        }

        public async Task<int> SaveGameToDatabaseAsync(GameEntity game)
        {
            if (!_isConnected)
                return 0;

            int changes = 0;

            using (var db = new BoardDataContext())
            {
                db.SavedGames.Add(game);
                changes = await db.SaveChangesAsync();
            }
            return changes;
        }

        //public List<BoardEntity> GetGameBoardDataFromSaveGame(GameEntity saveData)
        //{
        //    List<BoardEntity> gbdList = new List<BoardEntity>();

        //    if (!_isConnected)
        //        return gbdList;

        //    using (var db = new BoardDataContext())
        //    {
        //        var gbd = db.BoardGrid;

        //        foreach (var item in gbd.Where(x => x.GameId == saveData.BoardGridGameID))
        //        {
        //            gbdList.Add(item);
        //        }

        //        return gbdList;
        //    }
        //}

        public async Task<List<BoardEntity>> GetGameBoardDataFromSaveGameAsync(GameEntity saveData)
        {
            if (!_isConnected)
                return new List<BoardEntity>();

            using (var db = new BoardDataContext())
            {
                return await db.BoardGrid.Where(x => x.GameId == saveData.BoardGridGameID).ToListAsync();
            }
        }

        //public GameEntity GetSavedGameDataFromName(string saveName)
        //{
        //    using (var db = new BoardDataContext())
        //    {
        //        var sgd = db.SavedGames.Where(x => x.Name == saveName).FirstOrDefault();

        //        return sgd;
        //    }
        //}

        public async Task<GameEntity> GetSavedGameDataFromNameAsync(GameEntity game)
        {
            using (var db = new BoardDataContext())
            {
                return await db.SavedGames.Where(x => x.BoardGridGameID == game.BoardGridGameID).FirstOrDefaultAsync();
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

        //public List<GameEntity> GetAllSaves()
        //{
        //    List<GameEntity> gameBoards = new List<GameEntity>();
        //    if (_isConnected)
        //    {
        //        using (var db = new BoardDataContext())
        //        {
        //            gameBoards = db.SavedGames.ToList();
        //        }
        //    }
           
        //    return gameBoards;
        //}

        public int GetGameIdFromDb()
        {
            if (_isConnected)
            {
                using (var db = new BoardDataContext())
                {
                    var sgd = db.BoardGrid;

                    if (!sgd.Any())
                    {
                        return 1;
                    }
                    else
                    {
                        var dbId = sgd.OrderByDescending(x => x.GameId).FirstOrDefault().GameId;
                        return dbId;
                    }
                }
            }
            return -1;
        }

        //public void DeleteSaveGame(GameEntity game)
        //{
        //    if (!_isConnected)
        //        return;

        //    using (var db = new BoardDataContext())
        //    {
        //        var sgd = db.SavedGames.Where(x => x.Id == game.Id).FirstOrDefault();
        //        var boardsToDelete = db.BoardGrid.Where(x => x.GameId == sgd.BoardGridGameID);

        //        db.BoardGrid.RemoveRange(boardsToDelete);
        //        db.SavedGames.Remove(sgd);

        //        db.SaveChanges();
        //    }
        //}

        public async Task<int> DeleteSaveGameAsync(GameEntity game)
        {
            if (!_isConnected)
                return 0;

            int changes = 0;

            using (var db = new BoardDataContext())
            {
                var sgd = db.SavedGames.Where(x => x.Id == game.Id).FirstOrDefault();
                var boardsToDelete = db.BoardGrid.Where(x => x.GameId == sgd.BoardGridGameID);

                db.BoardGrid.RemoveRange(boardsToDelete);
                db.SavedGames.Remove(sgd);

                changes = await db.SaveChangesAsync();
            }

            return changes;
        }

    }
}
