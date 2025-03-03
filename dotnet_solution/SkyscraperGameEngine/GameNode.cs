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
    public HashSet<byte>[,] GridValidValues { get; }
    public HashSet<GameConstraint> NeedsCheckConstraints { get; }
    readonly HashSet<int>[,] validPositionsForValuePerCol;
    readonly HashSet<int>[,] validPositionsForValuePerRow;
    readonly GridContraintMap gridContraintMap;

    private GameNode(int size,
                     bool isInfeasible,
                     int numInserted,
                     byte[,] gridValues,
                     HashSet<byte>[,] gridValidValues,
                     HashSet<GameConstraint> needsCheckConstraints,
                     GridContraintMap gridContraintMap,
                     HashSet<int>[,] validPositionsForValuePerCol,
                     HashSet<int>[,] validPositionsForValuePerRow)
    {
        Size = size;
        IsInfeasible = isInfeasible;
        NumInserted = numInserted;
        NumCells = size * size;
        GridValues = gridValues;
        GridValidValues = gridValidValues;
        NeedsCheckConstraints = needsCheckConstraints;
        this.gridContraintMap = gridContraintMap;
        this.validPositionsForValuePerCol = validPositionsForValuePerCol;
        this.validPositionsForValuePerRow = validPositionsForValuePerRow;
    }

    static public GameNode CreateRootNode(GridContraintMap gridContraintMap, byte[,] initialGrid)
    {
        int size = initialGrid.GetLength(0);
        byte[,] emptyGrid = new byte[size, size];
        HashSet<byte>[,] validValues = new HashSet<byte>[size, size];
        HashSet<int>[,] validPosCol = new HashSet<int>[size, size];
        HashSet<int>[,] validPosRow = new HashSet<int>[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                validValues[i, j] = [.. Enumerable.Range(0, size).Select(n => (byte)(n + 1))];
                validPosCol[i, j] = [.. Enumerable.Range(0, size)];
                validPosRow[i, j] = [.. Enumerable.Range(0, size)];
            }
        }


        GameNode rootNode = new(size,
                                size == 0,
                                0,
                                emptyGrid,
                                validValues,
                                [],
                                gridContraintMap,
                                validPosCol,
                                validPosRow);
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (initialGrid[i, j] != 0)
                    rootNode.InsertValue((i, j), initialGrid[i, j]);
            }
        }
        rootNode.LastInsertPosition = (-1, -1);
        foreach (var constr in rootNode.NeedsCheckConstraints)
        {
            if (constr.IsViolated(rootNode))
                rootNode.IsInfeasible = true;
        }
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

    public void UpdateConstraintStatus(GameConstraint constraint, bool isSatisfiable)
    {
        if (isSatisfiable)
        {
            NeedsCheckConstraints.Remove(constraint);
            UpdateSolveStatus();
        }
        else
        {
            IsInfeasible = true;
        }

    }

    public IEnumerable<(byte, byte)> GetGridValueBounds(IEnumerable<(int, int)> positions)
    {
        foreach (var (i, j) in positions)
        {
            byte value = GridValues[i, j];
            if (value > 0)
                yield return (value, value);
            else
            {
                byte lb = GridValidValues[i, j].Min();
                byte ub = GridValidValues[i, j].Max();
                yield return (lb, ub);
            }
        }
    }

    public IEnumerable<byte> GetGridValues(IEnumerable<(int, int)> positions)
    {
        foreach (var (i, j) in positions)
        {
            yield return GridValues[i, j];
        }
    }

    private bool CanInsertValue((int, int) position, byte value)
    {
        if (value < 1 || value > Size)
            return false;
        (int x, int y) = position;
        if (x >= Size || y >= Size || x < 0 || y < 0)
            return false;
        return GridValidValues[x, y].Contains(value);
    }

    private void InsertValue((int, int) position, byte value)
    {
        (int y, int x) = position;
        NumInserted++;
        LastInsertPosition = position;
        GridValues[y, x] = value;
        for (int row_offset = 1; row_offset < Size; row_offset++)
            AddInvalidValue((y + row_offset) % Size, x, value);
        for (int col_offset = 1; col_offset < Size; col_offset++)
            AddInvalidValue(y, (x + col_offset) % Size, value);
        for (int val_offset = 0; val_offset < Size - 1; val_offset++)
            AddInvalidValue(y, x, (byte)((value + val_offset) % Size + 1));
        GridValidValues[y, x].Remove(value);
        validPositionsForValuePerRow[y, value - 1].Remove(x);
        validPositionsForValuePerCol[x, value - 1].Remove(y);
        MarkConstraintNeedCheck(position);
        UpdateSolveStatus();
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
        HashSet<byte>[,] gridValidValues = new HashSet<byte>[Size, Size];
        HashSet<int>[,] validPosCol = new HashSet<int>[Size, Size];
        HashSet<int>[,] validPosRow = new HashSet<int>[Size, Size];
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                gridValidValues[i, j] = [.. GridValidValues[i, j]];
                validPosCol[i, j] = [.. validPositionsForValuePerCol[i, j]];
                validPosRow[i, j] = [.. validPositionsForValuePerRow[i, j]];
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
                            validPosCol,
                            validPosRow);
    }

    internal void AddInvalidValue(int i, int j, byte value)
    {
        if (GridValidValues[i, j].Remove(value))
        {
            validPositionsForValuePerCol[j, value - 1].Remove(i);
            validPositionsForValuePerRow[i, value - 1].Remove(j);
            if (GridValidValues[i, j].Count == 0 ||
                validPositionsForValuePerCol[j, value - 1].Count == 0 ||
                validPositionsForValuePerRow[i, value - 1].Count == 0)
                IsInfeasible = true;
        }
    }
}
