using System.Collections;

namespace BLL
{
    public class BoardBitArray : IBoard<BitArray>
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public bool Infinite { get; private set; }
        public int Generation { get; private set; }
        public int Alive { get; private set; }

        public BitArray Data { get; set; }
        
        public BoardBitArray(int width, int height, bool infinite = false, BitArray data = null)
        {
            Width = width;
            Height = height;
            Infinite = infinite;
            if (data == null)
                ClearCells();
            else
                Data = data;
        }

        // Modulus for negative numbers. eg. -1 % 30 should return 29.
        public int Mod(int input, int mod) => (input % mod + mod) % mod;

        public int GetIndex(int x, int y)
        {
            return (y * Width) + x;
        }

        public void GetCoords(int n, out int x, out int y)
        {
            x = n % Width;
            y = (n - x) / Width;
        }

        public int GetCell(int x, int y)
        {
            if (Infinite == true)
            {
                x = Mod(x, Width);
                y = Mod(y, Height);

                return Data[(y * Width) + x] == true ? 1 : 0;
            }
            else if ((x >= 0 && x < Width) && (y >= 0 && y < Height))
            {
                return Data[(y * Width) + x] == true ? 1 : 0;
            }
            return 0;
        }

        public void ClearCells()
        {
            Data.SetAll(false);
            Alive = 0;
        }

        public void SetCell(int x, int y, bool value)
        {
            int position = (y * Width) + x;
            
            Data[position] = value;

            if (value == true)
                Alive++;
        }

        public int GetNeighbours(int x, int y)
        {
            return
                GetCell(x + 1, y + 1) + GetCell(x + 1, y + 0) + GetCell(x + 1, y - 1) +
                GetCell(x + 0, y + 1) + GetCell(x + 0, y - 1) +
                GetCell(x - 1, y + 1) + GetCell(x - 1, y + 0) + GetCell(x - 1, y - 1);
        }
    }
}
