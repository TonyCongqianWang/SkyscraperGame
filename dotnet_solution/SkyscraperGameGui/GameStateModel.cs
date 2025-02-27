using SkyscraperGameEngine;

namespace SkyscraperGameGui;

record class GameStateModel
{
    public int Size { get; }
    public int CurrentDepth { get; set; } = 0;
    public bool IsSolved { get; set; } = false;
    public bool IsInfeasible { get; set; } = false;
    public UInt128 NumInserts { get; set; } = 0;
    public UInt128 NumChecks { get; set; } = 0;
    public UInt128 NumUnsets { get; set; } = 0;
    public (int, int) LastSetIndex { get; set; } = (-1, -1);
    public byte[] TopValues { get; set; }
    public byte[] BottomValues { get; set; }
    public byte[] LeftValues { get; set; }
    public byte[] RightValues { get; set; }
    public bool[] TopValuesCheckStatus { get; set; }
    public bool[] BottomValuesCheckStatus { get; set; }
    public bool[] LeftValuesCheckStatus { get; set; }
    public bool[] RightValuesCheckStatus { get; set; }
    public byte[,] GridValues { get; set; }
    public bool[,,] GridValueValidities { get; set; }

    public GameStateModel(int size)
    {
        if (size < 4 || size > 9)
            throw new ArgumentException("Size must be between 4 and 9");
        Size = size;
        TopValues = new byte[size];
        BottomValues = new byte[size];
        LeftValues = new byte[size];
        RightValues = new byte[size];

        TopValuesCheckStatus = new bool[size];
        BottomValuesCheckStatus = new bool[size];
        LeftValuesCheckStatus = new bool[size];
        RightValuesCheckStatus = new bool[size];

        GridValues = new byte[size, size];
        GridValueValidities = new bool[size, size, size];
    }

    public GameStateModel (GameState gameState)
    {
        GameNodes currendNode = gameState.GameNodes.Peek();
        NumInserts = gameState.GameStatistics.NumInserts;
        NumChecks = gameState.GameStatistics.NumChecks;
        NumUnsets = gameState.GameStatistics.NumUndos;

        Size = currendNode.Size;
        CurrentDepth = currendNode.CurrentDepth;
        IsSolved = currendNode.IsSolved;
        IsInfeasible = currendNode.IsInfeasible;
        LastSetIndex = currendNode.LastInsertPosition;

        TopValuesCheckStatus = new bool[Size];
        BottomValuesCheckStatus = new bool[Size];
        LeftValuesCheckStatus = new bool[Size];
        RightValuesCheckStatus = new bool[Size];

        TopValues = new byte[Size];
        BottomValues = new byte[Size];
        LeftValues = new byte[Size];
        RightValues = new byte[Size];
        int constrIdx = 0;
        for (; constrIdx < 1 * Size; constrIdx++)
        {
            TopValues[constrIdx % Size] = gameState.GameConstraints.Constraints[constrIdx].Value;
        }
        for (; constrIdx < 2 * Size; constrIdx++)
        {
            BottomValues[constrIdx % Size] = gameState.GameConstraints.Constraints[constrIdx].Value;
        }
        for (; constrIdx < 3 * Size; constrIdx++)
        {
            LeftValues[constrIdx % Size] = gameState.GameConstraints.Constraints[constrIdx].Value;
        }
        for (; constrIdx < 4 * Size; constrIdx++)
        {
            RightValues[constrIdx % Size] = gameState.GameConstraints.Constraints[constrIdx].Value;
        }
        GridValues = currendNode.GridValues;
        GridValueValidities = new bool[Size, Size, Size];
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                for (int val = 1; val <= Size; val++)
                {
                    GridValueValidities[i, j, val - 1] = !currendNode.GridInvalidValues[i, j].Contains((byte)val);
                }
            }
        }
    }
}
