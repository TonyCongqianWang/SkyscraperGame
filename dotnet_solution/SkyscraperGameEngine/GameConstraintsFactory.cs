using System.Collections.Immutable;

namespace SkyscraperGameEngine;

using GridContraintMap = ImmutableDictionary<(int, int), ImmutableArray<GameConstraint>>;

class GameConstraintsFactory
{
    private readonly ConstraintChecker checker = new();

    public GameConstraints CreateEmptyConstraints(byte[,] grid)
    {
        int size = grid.GetLength(0);
        var enumerator = Array.Empty<int>().Select(i => i).GetEnumerator();
        return CreateGameConstraints(grid, size, [], [], enumerator);
    }

    public GameConstraints CreateGameConstraints(InstanceGenerationOptions options, byte[,] grid, Random rng)
    {
        int size = options.Size;
        int numKeep = (int)(Math.Min(options.ConstraintFillRate, 1.0) * size * 4);
        numKeep = Math.Max(numKeep, 0);
        int[] keepIndeces = [.. Enumerable.Range(0, 4 * size)];
        int[] modifyIndeces = [];
        rng.Shuffle(keepIndeces);
        keepIndeces = [.. keepIndeces.Take(numKeep)];
        if (options.AllowInfeasible && numKeep > 0)
            modifyIndeces = rng.GetItems(keepIndeces, 1);
        IEnumerator<int> modifyValues = modifyIndeces.Select(_ => rng.NextDouble() < 0.33 ? 1 : -1).GetEnumerator();
        return CreateGameConstraints(grid, size, keepIndeces, modifyIndeces, modifyValues);
    }

    private GameConstraints CreateGameConstraints(
        byte[,] grid,
        int size,
        int[] keepIndeces,
        int[] modifyIndeces,
        IEnumerator<int> modifyValues)
    {
        GameConstraint[] constraints = new GameConstraint[size * 4];

        Dictionary<(int, int), List<GameConstraint>> gridContraintMap = [];
        foreach (int col in Enumerable.Range(0, size))
        {
            foreach (int row in Enumerable.Range(0, size))
            {
                gridContraintMap[(row, col)] = [];
            }
        }
        for (int constraintIdx = 0; constraintIdx < 4 * size; constraintIdx++)
        {
            if (!keepIndeces.Contains(constraintIdx))
            {
                constraints[constraintIdx] = new GameConstraint(constraintIdx, 0, []);
                continue;
            }
            ImmutableArray<(int, int)> gridPositions = GetGridPositions(size, constraintIdx);
            var gridValues = gridPositions.Select(((int y, int x) p) => grid[p.y, p.x]);
            int constraintValue = checker.CalculateConstraintValue(gridValues);
            if (modifyIndeces.Contains(constraintIdx))
            {
                modifyValues.MoveNext();
                constraintValue += modifyValues.Current;
                constraintValue = Math.Min(constraintValue, size);
                constraintValue = Math.Max(constraintValue, 1);
            }
            constraints[constraintIdx] = new GameConstraint(constraintIdx, (byte)constraintValue, gridPositions);
            foreach (var pos in gridPositions)
                gridContraintMap[pos].Add(constraints[constraintIdx]);

        }
        GridContraintMap.Builder mappingBuilder = ImmutableDictionary.CreateBuilder<(int, int), ImmutableArray<GameConstraint>>();
        foreach (var (key, value) in gridContraintMap)
            mappingBuilder.Add(key, [.. value]);
        return new([.. constraints], mappingBuilder.ToImmutableDictionary());
    }

    private static ImmutableArray<(int, int)> GetGridPositions(int size, int constraintIdx)
    {
        if (constraintIdx < 1 * size)
        {
            int col = constraintIdx % size;
            return [.. (from row in Enumerable.Range(0, size)
                                                            select (row, col))];
        }
        else if (constraintIdx < 2 * size)
        {
            int col = constraintIdx % size;
            return [.. (from row in Enumerable.Range(0, size).Reverse()
                                                            select (row, col))];
        }
        else if (constraintIdx < 3 * size)
        {
            int row = constraintIdx % size;
            return [.. (from col in Enumerable.Range(0, size)
                                                            select (row, col))];
        }
        else
        {
            int row = constraintIdx % size;
            return [.. (from col in Enumerable.Range(0, size).Reverse()
                                                            select (row, col))];
        }
    }
}
