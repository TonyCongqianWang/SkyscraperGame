using System.Diagnostics.CodeAnalysis;

namespace SkyscraperGameEngine;

public class GameEngine
{
    private GameState gameState;

    private readonly InstanceGenerator instanceGenerator;
    private readonly ConstraintChecker constraintChecker;

    public GameEngine()
    {
        instanceGenerator = new();
        constraintChecker = new();

        StartNewGame(new InstanceGenerationOptions());
    }

    [MemberNotNull(nameof(gameState))]
    public void StartNewGame(InstanceGenerationOptions options)
    {
        gameState = instanceGenerator.GenerateNewGame(options);
    }

    public GameStateViewModel GetState()
    {
        return new(gameState);
    }

    public bool TryUndoLast()
    {
        GameNode currentNode = gameState.GameNodes.Peek();
        if ( currentNode.IsSolved)
            return false;
        if (gameState.GameNodes.Count == 1)
            return false;
        gameState.GameStatistics.IncrementUndos();
        _ = gameState.GameNodes.Pop();
        if (currentNode.IsInfeasible)
        {
            GameNode parentNode = gameState.GameNodes.Peek();
            (int i, int j) = currentNode.LastInsertPosition;
            parentNode.AddInvalidValue(i, j, currentNode.GridValues[i, j]);
        }
        return true;
    }

    public bool TryInsertValue((int, int) position, byte value)
    {
        GameNode currentNode = gameState.GameNodes.Peek();
        if (currentNode.IsSolved)
            return false;
        GameNode? nextNode = currentNode.TryCreateChild(position, value);
        if (nextNode == null)
            return false;
        gameState.GameStatistics.IncrementInserts();
        gameState.GameNodes.Push(nextNode);
        return true;
    }
    public bool TryCheckConstraint(int constraintIndex)
    {
        GameNode currentNode = gameState.GameNodes.Peek();
        if (currentNode.IsSolved)
            return false;
        if (currentNode.IsInfeasible)
            return false;
        if (constraintIndex < 0 || constraintIndex >= gameState.GameConstraints.Constraints.Length)
            return false;
        gameState.GameStatistics.IncrementChecks();
        currentNode.CheckConstraint(
            gameState.GameConstraints.Constraints[constraintIndex], constraintChecker);
        return true;
    }
}
