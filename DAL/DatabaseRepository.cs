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

        public void SaveBoardToDatabase(StringBuilder sb)
        {

            var gridString = sb.ToString();

            //Console.WriteLine(myBA4[0]);
            using (var db = new BoardDataContext())
            {

                BoardGrid bg = new BoardGrid
                {
                    Grid = gridString
                };

                db.BoardGrid.Add(bg);

                db.SaveChanges();
            }

        }

        public BoardGrid GetGridDataFromDatabase()
        {

            using (var db = new BoardDataContext())
            {

                db.BoardGrid
                
            }
        }
    }
}
