using System.Diagnostics.CodeAnalysis;

namespace SkyscraperGameEngine;


public record class GameObservation
{
    public string StringRepresentation { get; }

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
    public bool[] TopValueNeedsCheckArray { get; set; }
    public bool[] BottomValueNeedsCheckArray { get; set; }
    public bool[] LeftValueNeedsCheckArray { get; set; }
    public bool[] RightValueNeedsCheckArray { get; set; }
    public byte[,] GridValues { get; set; }
    public bool[,,] ValidInsertionsArray { get; set; }

    internal GameObservation(int size)
    {
        if (size < 4 || size > 9)
            throw new ArgumentException("Size must be between 4 and 9");
        StringRepresentation = "";
        Size = size;
        TopValues = new byte[size];
        BottomValues = new byte[size];
        LeftValues = new byte[size];
        RightValues = new byte[size];

        TopValueNeedsCheckArray = new bool[size];
        BottomValueNeedsCheckArray = new bool[size];
        LeftValueNeedsCheckArray = new bool[size];
        RightValueNeedsCheckArray = new bool[size];

        GridValues = new byte[size, size];
        ValidInsertionsArray = new bool[size, size, size];
    }

    internal GameObservation(GameState gameState)
    {
        StringRepresentation = gameState.SerializePuzzle();
        GameNode currendNode = gameState.GameNodes.Peek();
        NumInserts = gameState.GameStatistics.NumInserts;
        NumChecks = gameState.GameStatistics.NumChecks;
        NumUnsets = gameState.GameStatistics.NumUndos;

        Size = gameState.GameSize;
        CurrentDepth = gameState.GameNodes.Count - 1;
        IsSolved = currendNode.IsSolved;
        IsInfeasible = currendNode.IsInfeasible;
        LastSetIndex = currendNode.LastInsertPosition;
        FillNeedCheckArrays(currendNode);
        FillConstraintValues(gameState);
        GridValues = currendNode.GridValues;
        FillValidInsertionsArray(currendNode);
    }

    [MemberNotNull(nameof(ValidInsertionsArray))]
    private void FillValidInsertionsArray(GameNode currendNode)
    {
        ValidInsertionsArray = new bool[Size, Size, Size];
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                for (int val = 1; val <= Size; val++)
                {
                    ValidInsertionsArray[i, j, val - 1] = currendNode.GridValidValues[i, j].Contains((byte)val);
                }
            }
        }
    }

    [MemberNotNull(nameof(TopValueNeedsCheckArray), nameof(BottomValueNeedsCheckArray), nameof(LeftValueNeedsCheckArray), nameof(RightValueNeedsCheckArray))]
    private void FillNeedCheckArrays(GameNode currendNode)
    {
        TopValueNeedsCheckArray = new bool[Size];
        BottomValueNeedsCheckArray = new bool[Size];
        LeftValueNeedsCheckArray = new bool[Size];
        RightValueNeedsCheckArray = new bool[Size];
        foreach (GameConstraint constraint in currendNode.NeedsCheckConstraints)
        {
            int constrIdx = constraint.Id;
            if (constrIdx < Size)
                TopValueNeedsCheckArray[constrIdx] = true;
            else if (constrIdx < 2 * Size)
                BottomValueNeedsCheckArray[constrIdx % Size] = true;
            else if (constrIdx < 3 * Size)
                LeftValueNeedsCheckArray[constrIdx % Size] = true;
            else
                RightValueNeedsCheckArray[constrIdx % Size] = true;
        }
    }

    [MemberNotNull(nameof(TopValues), nameof(BottomValues), nameof(LeftValues), nameof(RightValues))]
    private void FillConstraintValues(GameState gameState)
    {
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
    }
}
