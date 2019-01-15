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

        public void SaveBoardToDatabase(GameBoard board)
        {
            repo.SaveBoardToDatabase(board.Data, CurrentGameId, board.Generation);
        }

        public void SaveGameToDatabase(string name, GameBoard board)
        {
            repo.SaveGameToDatabase(name, CurrentGameId, board.Width, board.Height, board.Generation);
        }

        public int GetNextGameId()
        {
            return repo.GetGameIdFromDb() + 1;
        }

        public List<GameBoard> GetSavedGameFromDatabase(GameEntity board)
        {
            List<GameBoard> gameBoardList = new List<GameBoard>();

            GameEntity saveGameData = repo.GetSavedGameDataFromName(board.Name);
            List<BoardEntity> gbd = repo.GetGameBoardDataFromSaveGame(saveGameData);

            for (int i = 0; i < gbd.Count; i++)
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

        public async Task<List<GameEntity>> GetAllSavesFromDb()
        {
            return await repo.GetAllSavesAsync();
        }

        public void DeleteSaveGame(GameEntity game)
        {
            repo.DeleteSaveGame(game);
        }
    }
}
