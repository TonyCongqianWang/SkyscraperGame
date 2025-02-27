namespace SkyscraperGameEngine;

public class GameNodes(int size,
                 bool isSolved,
                 bool isInfeasible,
                 int numInserted,
                 byte[,] gridValues,
                 HashSet<byte>[,] gridValidValues)
{
    public int Size { get; } = size;
    public bool IsSolved { get; set; } = isSolved;
    public bool IsInfeasible { get; set; } = isInfeasible;
    public int NumInserted { get; set; } = numInserted;
    public int NumCells { get; } = size * size;
    public (int, int) LastInsertPosition { get; set; } = (-1, -1);
    public byte[,] GridValues { get; set; } = gridValues;
    public HashSet<byte>[,] GridInvalidValues { get; set; } = gridValidValues;

    internal GameNodes Clone()
    {
        byte[,] newGridValues = new byte[Size, Size];
        Array.Copy(GridValues, newGridValues, GridValues.Length);
        HashSet<byte>[,] gridValidValues = new HashSet<byte>[Size, Size];
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                gridValidValues[i, j] = [.. GridInvalidValues[i, j]];
            }
        }
        return new GameNodes(Size, IsSolved, IsInfeasible, NumInserted, newGridValues, gridValidValues);
    }
}
