namespace SkyscraperGameEngine;

class GameEngine(GameState initialState)
{
    public GameState GameState { get; set; } = initialState;

    private readonly ConstraintChecker constraintChecker = new();

    public bool TryUndoLast()
    {
        GameNode currentNode = GameState.GameNodes.Peek();
        if (currentNode.IsSolved)
            return false;
        if (GameState.GameNodes.Count == 1)
            return false;
        GameState.GameStatistics.IncrementUndos();
        _ = GameState.GameNodes.Pop();
        if (currentNode.IsInfeasible)
        {
            GameNode parentNode = GameState.GameNodes.Peek();
            (int i, int j) = currentNode.LastInsertPosition;
            parentNode.AddInvalidValue(i, j, currentNode.GridValues[i, j]);
        }
        return true;
    }

    public bool TryInsertValue((int, int) position, byte value)
    {
        GameNode currentNode = GameState.GameNodes.Peek();
        if (currentNode.IsSolved)
            return false;
        GameNode? nextNode = currentNode.TryCreateChild(position, value);
        if (nextNode == null)
            return false;
        GameState.GameStatistics.IncrementInserts();
        GameState.GameNodes.Push(nextNode);
        return true;
    }
    public bool TryCheckConstraint(int constraintIndex)
    {
        GameNode currentNode = GameState.GameNodes.Peek();
        if (currentNode.IsSolved)
            return false;
        if (currentNode.IsInfeasible)
            return false;
        if (constraintIndex < 0 || constraintIndex >= GameState.GameConstraints.Constraints.Length)
            return false;
        GameState.GameStatistics.IncrementChecks();
        var constraint = GameState.GameConstraints.Constraints[constraintIndex];

        currentNode.UpdateConstraintStatus(
            constraint,
            constraintChecker.IsConstraintSatisfiable(
                constraint.Value,
                (byte)GameState.GameSize,
                currentNode.GetGridValueBounds(constraint.Positions)));
        return true;
    }
}
