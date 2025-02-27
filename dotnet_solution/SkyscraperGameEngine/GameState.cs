
namespace SkyscraperGameEngine;

public class GameState(int size,
                 bool isSolved,
                 bool isInfeasible,
                 int currentDepth,
                 int numInserts,
                 byte[,] gridValues,
                 HashSet<byte>[,] gridValidValues)
{
    public int Size { get; set; } = size;
    public bool IsSolved { get; set; } = isSolved;
    public bool IsInfeasible { get; set; } = isInfeasible;
    public int CurrentDepth { get; set; } = currentDepth;
    public int NumInserts { get; set; } = numInserts;
    public byte[,] GridValues { get; set; } = gridValues;
    public HashSet<byte>[,] GridValidValues { get; set; } = gridValidValues;

    internal GameState Clone()
    {
        byte[,] newGridValues = new byte[Size, Size];
        Array.Copy(GridValues, newGridValues, GridValues.Length);
        HashSet<byte>[,] gridValidValues = new HashSet<byte>[Size, Size];
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                gridValidValues[i, j] = [.. GridValidValues[i, j]];
            }
        }
        return new GameState(Size, IsSolved, IsInfeasible, CurrentDepth, NumInserts, newGridValues, gridValidValues);
    }
}
