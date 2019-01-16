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
        private DatabaseRepository repo;
        private int CurrentGameId { get; set; }

        public Service()
        {
            repo = new DatabaseRepository();
            CurrentGameId = GetNextGameId();
        }

        public async Task<int> SaveBoardToDatabaseAsync(BoardStringBuilder board)
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
                Generations = board.CurrentBoard.Generation,
                Name = name,
                Infinite = board.Infinite
            };

            return await repo.SaveGameToDatabaseAsync(game);
        }

        // TODO: Remove
        public int GetNextGameId()
        {
            return repo.GetGameIdFromDb() + 1;
        }

        public async Task<GameBoard> GetSavedGameFromDatabaseAsync(GameEntity gameboard)
        {
            //List<GameBoard> gameBoardList = new List<GameBoard>();
            GameEntity game = await repo.GetSavedGameDataFromNameAsync(gameboard);

            var newGameBoard = new GameBoard(game.BoardGridGameID, game.Name, game.Width, game.Height, game.Infinite);
            List<BoardEntity> boards = await repo.GetGameBoardDataFromSaveGameAsync(game);

            foreach (BoardEntity board in boards)
            {
                StringBuilder sb = new StringBuilder(board.Grid);

                //GameBoard gb = new GameBoard(game.BoardGridGameID, game.Name, game.Width, game.Height, game.Infinite, sb)
                //{
                //    isRecorded = true
                //};

                var newBoard = new BoardStringBuilder(game.Width, game.Height, game.Infinite, sb);

                newGameBoard.Boards.Add(newBoard);
            }

            return newGameBoard;
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
