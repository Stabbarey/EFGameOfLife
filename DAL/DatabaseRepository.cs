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

        public GameBoardData[] GetGridDataFromSavedGame(int gameId)
        {

            using (var db = new BoardDataContext())
            {

                var gridData = db.BoardGrid.Where(x => x.GameId == gameId).ToArray();


                return gridData;
            }    
        }

        public SaveGameData GetSavedGameFromId(int savedGameId)
        {

            using (var db = new BoardDataContext())
            {

                var boardGridData = db.BoardGrid;
                var savedGameData = db.SavedGames;

                //var savedGameQuery = from s in savedGameData
                //                     join b in boardGridData on s.Id equals b.GameId
                //                     where s.Id == savedGameId
                //                     select s.Id;

                // Console.WriteLine(savedGameQuery);

                return savedGameData.Where(x => x.Id == savedGameId) as SaveGameData;
            }
        }
   
    }
}
