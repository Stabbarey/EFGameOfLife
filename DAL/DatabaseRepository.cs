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

        public void SaveBoardToDatabase(bool[,] grid)
        {

            //bool[,] gridBool = grid;
            //bool[] gridBoolArray = new bool[gridBool.Length];

            //Buffer.BlockCopy(gridBool, 0, gridBoolArray, 0, gridBool.Length);

            //List<bool> gridBoolList = new List<bool>();

            //gridBoolList.AddRange(gridBoolArray);

            //var binFormatter = new BinaryFormatter();
            //var mStream = new MemoryStream();
            //binFormatter.Serialize(mStream, gridBoolList);

            //var gridToByteArray = mStream.ToArray();

            //bool[,] gridBool = grid;

            //string hej = "";

            //for (int x = 0; x < gridBool.GetLength(0); x++)
            //{
            //    for (int y = 0; y < gridBool.GetLength(1); y++)
            //    {
            //        hej = gridBool[x, y].ToString();
            //    }
            //}

            //Console.WriteLine(myBA4[0]);
            //using (var db = new BoardDataContext())
            //{

            //    BoardGrid bg = new BoardGrid
            //    {
            //        Grid = hej
            //    };

            //    db.BoardGrid.Add(bg);

            //    db.SaveChanges();
            //}

        }

        //byte[] PackBoolsInByteArray(bool[] bools)
        //{
        //    int len = bools.Length;
        //    int bytes = len >> 3;
        //    if ((len & 0x07) != 0) ++bytes;
        //    byte[] arr2 = new byte[bytes];
        //    for (int i = 0; i < bools.Length; i++)
        //    {
        //        if (bools[i])
        //            arr2[i >> 3] |= (byte)(1 << (i & 0x07));
        //    }

        //    return arr2;
        //}

        public bool GetGridFromDb()
        {

            return false;

            //using (var db = new BoardDataContext())
            //{

            //    var grids = db.BoardGrid;

            //    foreach (var item in grids)
            //    {
            //        if (item.Id == 1006)
            //        {
            //            item.Grid = bytes;
            //        }
            //    }
            //}

            //bool result = false;

            //foreach (byte byteValue in bytes)
            //{
            //    result = Convert.ToBoolean(byteValue);
            //    Console.WriteLine("{0,-5}  -->  {1}", byteValue, result);


            //}
            //return result;
        }

    }
}
