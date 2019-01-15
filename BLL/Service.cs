using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class Service
    {
        private DatabaseRepository repo = new DatabaseRepository();
        private int CurrentGameId { get; set; }

        public Service()
        {
            repo = new DatabaseRepository();
            CurrentGameId = GetNextGameId();
        }

        public async Task<int> SaveBoardToDatabaseAsync(GameBoard board)
        {
            return await repo.SaveBoardToDatabaseAsync(board.Data, CurrentGameId, board.Generation);
        }

        public async Task<int> SaveGameToDatabaseAsync(string name, GameBoard board)
        {
            var game = new GameEntity
            {
                BoardGridGameID = CurrentGameId,
                Width = board.Width,
                Height = board.Height,
                Generations = board.Generation,
                Name = name
            };

            return await repo.SaveGameToDatabaseAsync(game);
        }

        public int GetNextGameId()
        {
            return repo.GetGameIdFromDb() + 1;
        }

        //TODO: Async
        //public List<GameBoard> GetSavedGameFromDatabase(GameEntity board)
        //{
        //    List<GameBoard> gameBoardList = new List<GameBoard>();

        //    GameEntity saveGameData = repo.GetSavedGameDataFromName(board.Name);
        //    List<BoardEntity> gbd = repo.GetGameBoardDataFromSaveGame(saveGameData);

        //    for (int i = 0; i < gbd.Count; i++)
        //    {
        //        string gbData = gbd[i].Grid;
        //        StringBuilder sb = new StringBuilder(gbData);

        //        GameBoard gb = new GameBoard
        //        {
        //            Width = saveGameData.Width,
        //            Height = saveGameData.Height,
        //            Name = saveGameData.Name,
        //            Data = sb
        //        };

        //        gameBoardList.Add(gb);
        //    }

        //    return gameBoardList;
        //}

        public async Task<List<GameBoard>> GetSavedGameFromDatabaseAsync(GameEntity gameboard)
        {
            List<GameBoard> gameBoardList = new List<GameBoard>();

            GameEntity game = await repo.GetSavedGameDataFromNameAsync(gameboard);
            List<BoardEntity> boards = await repo.GetGameBoardDataFromSaveGameAsync(game);

            foreach (BoardEntity board in boards)
            {
                StringBuilder sb = new StringBuilder(board.Grid);

                GameBoard gb = new GameBoard
                {
                    Width = game.Width,
                    Height = game.Height,
                    Name = game.Name,
                    Data = sb
                };

                gameBoardList.Add(gb);
            }

            return gameBoardList;
        }


        public async Task<List<GameEntity>> GetAllSavesFromDb()
        {
            return await repo.GetAllSavesAsync();
        }

        public async Task<int> DeleteSaveGameAsync(GameEntity game)
        {
            return await repo.DeleteSaveGameAsync(game);
        }
    }
}
