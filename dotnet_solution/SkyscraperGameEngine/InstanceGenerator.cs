namespace SkyscraperGameEngine;

class InstanceGenerator
{
    private readonly LatinSquareGenerator lsGen = new();
    private readonly GameConstraintsFactory constraintsFactory = new();

    private Random backupRng;
    private Random activeRng;

    public InstanceGenerator()
    {
        backupRng = new();
        activeRng = backupRng;
    }

    public GameState GenerateNewGame(InstanceGenerationOptions options)
    {
        if (options.RandomSeed >= 0)
            activeRng = new(options.RandomSeed);
        else
            activeRng = backupRng;
        byte[,] latinSquare = lsGen.GenerateLatinSquare(options.Size, activeRng);
        GameConstraints constraints = constraintsFactory.CreateGameConstraints(options, latinSquare, activeRng);
        RemoveRandomValuesFromGrid(options, latinSquare);
        GameNode roodNode = GameNode.CreateRootNode(constraints.GridConstraintMap, latinSquare);
        return new(constraints, roodNode);
    }

    public void SetRandomSeed(int seed)
    {
        backupRng = new(seed);
        activeRng = backupRng;
    }

    private void RemoveRandomValuesFromGrid(InstanceGenerationOptions options, byte[,] latinSquare)
    {
        int size = latinSquare.GetLength(0);
        int numKeep = (int)(size * size * Math.Min(options.GridFillRate, 1.0));
        HashSet<(int, int)> allPositions = [.. (from i in Enumerable.Range(0, size)
                                    from j in Enumerable.Range(0, size)
                                    select (i, j))];

        if (options.AllowInfeasible)
        {
            RemoveOneFromEachColumnAndRow(latinSquare, size, allPositions);
            numKeep += size;
            if (numKeep > size * size)
                return;
        }

        (int, int)[] removePositions = [.. allPositions];
        activeRng.Shuffle(removePositions);
        foreach ((int i, int j) in removePositions.Take(size * size - numKeep))
        {
            latinSquare[i, j] = 0;
        }

        void RemoveOneFromEachColumnAndRow(byte[,] latinSquare, int size, HashSet<(int, int)> allPositions)
        {
            int[] removeColumn = [.. Enumerable.Range(0, size)];
            activeRng.Shuffle(removeColumn);
            foreach (int row in Enumerable.Range(0, size))
            {
                latinSquare[row, removeColumn[row]] = 0;
                allPositions.Remove((row, removeColumn[row]));
            }
        }
    }
}
