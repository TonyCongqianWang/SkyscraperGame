using System.Collections.Immutable;

namespace SkyscraperGameEngine;

using GridContraintMap = ImmutableDictionary<(int, int), ImmutableArray<GameConstraint>>;

class GameNode
{
    public int Size { get; }
    public int NumCells { get; }
    public bool IsSolved { get; private set; } = false;
    public bool IsInfeasible { get; private set; }
    public int NumInserted { get; private set; }
    public (int, int) LastInsertPosition { get; private set; } = (-1, -1);
    public byte[,] GridValues { get; private set; }
    public HashSet<byte>[,] GridInvalidValues { get; }
    public HashSet<GameConstraint> NeedsCheckConstraints { get; }
    readonly GridContraintMap gridContraintMap;
    readonly byte[,] invalidPositionsForValuePerCol;
    readonly byte[,] invalidPositionsForValuePerRow;

    private GameNode(int size,
                     bool isInfeasible,
                     int numInserted,
                     byte[,] gridValues,
                     HashSet<byte>[,] gridInvalidValues,
                     HashSet<GameConstraint> needsCheckConstraints,
                     GridContraintMap gridContraintMap,
                     byte[,] invalidPositionsForValuePerCol,
                     byte[,] invalidPositionsForValuePerRow)
    {
        Size = size;
        IsInfeasible = isInfeasible;
        NumInserted = numInserted;
        NumCells = size * size;
        GridValues = gridValues;
        GridInvalidValues = gridInvalidValues;
        NeedsCheckConstraints = needsCheckConstraints;
        this.gridContraintMap = gridContraintMap;
        this.invalidPositionsForValuePerCol = invalidPositionsForValuePerCol;
        this.invalidPositionsForValuePerRow = invalidPositionsForValuePerRow;
    }

    static public GameNode CreateRootNode(GridContraintMap gridContraintMap, byte[,] initialGrid)
    {
        int size = initialGrid.GetLength(0);
        byte[,] emptyGrid = new byte[size, size];
        HashSet<byte>[,] invalidValues = new HashSet<byte>[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                invalidValues[i, j] = [];
            }
        }
        GameNode rootNode = new(size,
                                false,
                                0,
                                emptyGrid,
                                invalidValues,
                                [],
                                gridContraintMap,
                                new byte[size, size],
                                new byte[size, size]);
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (initialGrid[i, j] != 0)
                    rootNode.InsertValue((i, j), initialGrid[i, j]);
            }
        }
        rootNode.LastInsertPosition = (-1, -1);
        rootNode.NeedsCheckConstraints.Clear();
        rootNode.UpdateSolveStatus();
        return rootNode;
    }

    public GameNode? TryCreateChild((int, int) position, byte value)
    {
        if (IsSolved || IsInfeasible)
            return null;
        if (!CanInsertValue(position, value))
            return null;
        GameNode child = Clone();
        child.InsertValue(position, value);
        return child;
    }

    public void CheckConstraint(GameConstraint constraint, ConstraintChecker checker)
    {
        if (checker.IsConstraintSatisfiable(constraint.Value, (byte)Size, getGridValueBounds()))
        {
            NeedsCheckConstraints.Remove(constraint);
            UpdateSolveStatus();
        }
        else
        {
            IsInfeasible = true;
        }
        IEnumerable<(byte, byte)> getGridValueBounds()
        {
            foreach (var (i, j) in constraint.Positions)
            {
                byte value = GridValues[i, j];
                if (value > 0)
                    yield return (value, value);
                else
                {
                    byte lb = 1;
                    while (lb < Size && GridInvalidValues[i, j].Contains(lb))
                    {
                        lb++;
                    }
                    byte ub = (byte)Size;
                    while (ub > 1 && GridInvalidValues[i, j].Contains(lb))
                    {
                        ub--;
                    }
                    yield return (lb, ub);
                }
            }
        }
    }

    private bool CanInsertValue((int, int) position, byte value)
    {
        if (value < 1 || value > Size)
            return false;
        (int x, int y) = position;
        if (x >= Size || y >= Size || x < 0 || y < 0)
            return false;
        return !GridInvalidValues[x, y].Contains(value);
    }

    private void InsertValue((int, int) position, byte value)
    {
        (int x, int y) = position;
        NumInserted++;
        LastInsertPosition = position;
        GridValues[x, y] = value;
        for (byte val = 1; val <= Size; val++)
            GridInvalidValues[x, y].Add(val);
        for (int row = 0; row < Size; row++)
            AddInvalidValue(row, y, value);
        for (int col = 0; col < Size; col++)
            AddInvalidValue(x, col, value);
        MarkConstraintNeedCheck(position);
    }

    internal void MarkConstraintNeedCheck((int, int) position)
    {
        foreach (var constr in gridContraintMap[position])
        {
            NeedsCheckConstraints.Add(constr);
        }
    }

    private void UpdateSolveStatus()
    {
        IsSolved = NumInserted == NumCells && NeedsCheckConstraints.Count == 0;
    }

    private GameNode Clone()
    {
        byte[,] newGridValues = new byte[Size, Size];
        Array.Copy(GridValues, newGridValues, NumCells);
        byte[,] newColValidCounter = new byte[Size, Size];
        Array.Copy(invalidPositionsForValuePerCol, newColValidCounter, NumCells);
        byte[,] newRowValidCounter = new byte[Size, Size];
        Array.Copy(invalidPositionsForValuePerRow, newRowValidCounter, NumCells);
        HashSet<byte>[,] gridValidValues = new HashSet<byte>[Size, Size];
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                gridValidValues[i, j] = [.. GridInvalidValues[i, j]];
            }
        }
        HashSet<GameConstraint> needsCheckConstrCpy = [.. NeedsCheckConstraints];
        return new GameNode(Size,
                            IsInfeasible,
                            NumInserted,
                            newGridValues,
                            gridValidValues,
                            needsCheckConstrCpy,
                            gridContraintMap,
                            newColValidCounter,
                            newRowValidCounter);
    }

    internal void AddInvalidValue(int i, int j, byte value)
    {
        if(GridInvalidValues[i, j].Add(value))
        {
            invalidPositionsForValuePerCol[j, value - 1]++;
            invalidPositionsForValuePerRow[i, value - 1]++;
            if (GridInvalidValues[i, j].Count == Size ||
                invalidPositionsForValuePerCol[j, value - 1] == Size ||
                invalidPositionsForValuePerRow[j, value - 1] == Size)
                IsInfeasible = true;
        }
    }
}
