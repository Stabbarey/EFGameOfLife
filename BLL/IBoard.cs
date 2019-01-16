namespace BLL
{
    public interface IBoard<T>
    {
        int Width { get;  }
        int Height { get;  }
        bool Infinite { get; }
        int Generation { get; }
        int Alive { get; }
        T Data { get; set; }

        int GetIndex(int x, int y);
        void GetCoords(int n, out int x, out int y);
        int GetCell(int x, int y);
        void ClearCells();
        void SetCell(int x, int y, bool value);
        int GetNeighbours(int x, int y);
    }
}
