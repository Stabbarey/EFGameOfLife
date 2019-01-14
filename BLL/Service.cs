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
        DatabaseRepository repo = new DatabaseRepository();

        public Service()
        {
            repo = new DatabaseRepository();
        }

        public void SaveBoardToDatabase(int gameId, int generation, StringBuilder data)
        {
            repo.SaveBoardToDatabase(data, gameId, generation);
        }

        public void SaveGameToDatabase(string name, int gameId, int width, int height, int generations)
        {
            repo.SaveGameToDatabase(name, gameId, width, height, generations);
        }

        public int GetNextGameId()
        {
            return repo.GetGameIdFromDb() + 1;
        }

        public List<GameBoard> GetSavedGameFromDatabase(string saveName)
        {

            List<GameBoard> gameBoardList = new List<GameBoard>();

            GameEntity saveGameData = repo.GetSavedGameDataFromName(saveName);
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

        public List<GameEntity> GetAllSavesFromDb()
        {

            List<GameEntity> saveGames = repo.GetAllSaves();

            return saveGames;
        }

        public void DeleteSaveGame(string name)
        {
            repo.DeleteSaveGame(name);
        }
    }
}
