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

        public void SaveBoardToDatabase(StringBuilder sb, int gameId, int generation)
        {

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

        public GameBoardData[] GetGameBoardDataFromSaveGameID(int gameId)
        {

            List<GameBoardData> gbdList = new List<GameBoardData>();

            using (var db = new BoardDataContext())
            {
                var gbd = db.BoardGrid;

                //Get all generations from an ID in the database, store in List<GameBoardData> perhaps

                foreach (var item in gbd.Where(x => x.GameId == gameId))
                {
                    gbdList.Add(item);
                }


                return gbdList.ToArray();
            }
        }

        public SaveGameData GetSavedGameDataFromId(int gameId)
        {

            using (var db = new BoardDataContext())
            {
                var sgd = db.SavedGames.Where(x => x.BoardGridGameID == gameId).FirstOrDefault();

                return sgd;
            }
            
        }
   
    }
}
