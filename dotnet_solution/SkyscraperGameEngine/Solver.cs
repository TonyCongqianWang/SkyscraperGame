namespace SkyscraperGameEngine;

class Solver
{
    GameEngine? gameEngine;

    public void SolvePuzzle(GameState state, Random rng)
    {
        if (gameEngine == null)
            gameEngine = new(state);
        else
            gameEngine.GameState = state;
        int size = state.GameSize;
        (int, int)[] cellPositions = [.. (from i in Enumerable.Range(0, size)
                                    from j in Enumerable.Range(0, size)
                                    select (i, j))];
        while (true)
        {
            GameNode curNode = gameEngine.GameState.GameNodes.Peek();
            if (curNode.IsSolved)
                break;
            if (curNode.IsInfeasible)
            {
                gameEngine.TryUndoLast();
                continue;
            }
            (int, int) bestCell = (-1, -1);
            foreach ((int y, int x) in cellPositions)
            {
                
            }
        }
    }
}
