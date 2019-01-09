using System;
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

        public void SaveBoardToDatabase(byte grid)
        {

            //MemoryStream ms = new MemoryStream();

           // BinaryFormatter bf = new BinaryFormatter();

            //bf.Serialize(ms, grid);

            //ms.Position = 0;

            //byte[] serializedData = new byte[ms.Length];

            //ms.Read(serializedData, 0, (int)ms.Length);

            //ms.Close();

            using(var db = new BoardDataContext()) {

                BoardGrid bg = new BoardGrid();
                bg.Grid = grid;

                //BoardGrid gridToSave = boardGrid;
                //gridToSave.Grid = serializedData;

                Console.WriteLine("Db save called");

                db.BoardGrid.Add(bg);

                db.SaveChanges();
            }
        }

    }
}
