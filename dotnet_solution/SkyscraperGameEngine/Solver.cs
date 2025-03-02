namespace SkyscraperGameEngine;

class Solver
{
    readonly Random backupRng = new();
    GameEngine gameEngine;

    public Solver()
    {
        byte[,] empty = new byte[0, 0];
        GameConstraints constraints = GameConstraintsFactory.CreateEmptyConstraints(empty);
        var rootNode = GameNode.CreateRootNode(constraints.GridConstraintMap, empty);
        GameState gameState = new(constraints, rootNode);
        gameEngine = new(gameState, new("solver"));
    }

    public void SolvePuzzle(GameState state, Random? rng)
    {
        gameEngine.GameState = state;
        rng ??= backupRng;

        while (MakeStep(gameEngine, rng))
        {
        }
    }

    private bool MakeStep(GameEngine gameEngine, Random rng)
    {
        GameNode curNode = gameEngine.GameState.GameNodes.Peek();
        int size = curNode.Size;
        var cellPositions = from i in Enumerable.Range(0, size)
                            from j in Enumerable.Range(0, size)
                            select (i, j);
        if (curNode.IsSolved)
            return false;
        if (curNode.IsInfeasible)
        {
            if (!gameEngine.TryUndoLast())
                return false;
            return true;
        }
        var (y, x) = cellPositions.Where(((int y, int x) p) => curNode.GridValues[p.y, p.x] == 0)
                    .MinBy(((int y, int x) p) => curNode.GridValidValues[p.y, p.x].Count);
        byte[] nextVal = rng.GetItems([.. curNode.GridValidValues[y, x]], 1);
        gameEngine.TryInsertValue((y, x), nextVal[0]);
        gameEngine.TryCheckAllConstraints();
        return true;
    }
}
