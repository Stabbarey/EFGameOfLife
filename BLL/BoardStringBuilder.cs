using System.Text;

namespace BLL
{
    public class BoardStringBuilder : IBoard<StringBuilder>
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public bool Infinite { get; private set; }
        public int Generation { get; private set; }
        public int Alive { get; private set; }

        public StringBuilder Data { get; set; }

        public BoardStringBuilder(int width, int height, bool infinite = false, StringBuilder data = null)
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

                return Data[(y * Width) + x] == '1' ? 1 : 0;
            }
            else if ((x >= 0 && x < Width) && (y >= 0 && y < Height))
            {
                return Data[(y * Width) + x] == '1' ? 1 : 0;
            }
            return 0;
        }

        public void ClearCells()
        {
            Data = new StringBuilder(Width * Height);
            Data.Insert(0, "0", Width * Height);
            Alive = 0;
        }

        public void SetCell(int x, int y, bool value)
        {
            int position = (y * Width) + x;
            Data.Remove(position, 1);
            Data.Insert(position, value == true ? "1" : "0");

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
