using System.Diagnostics.CodeAnalysis;

namespace SkyscraperGameEngine;

public class GameEngine
{
    private GameState gameState;

    private readonly InstanceGenerator instanceGenerator;
    private readonly ConstraintChecker constraintChecker;
    private readonly InsertValidator insertValidator;
    private readonly ValueInserter valueInserter;

    public GameEngine()
    {
        instanceGenerator = new();
        constraintChecker = new();
        insertValidator = new();
        valueInserter = new();

        StartNewGame(new InstanceGenerationOptions());
    }

    [MemberNotNull(nameof(gameState))]
    public void StartNewGame(InstanceGenerationOptions options)
    {
        gameState = instanceGenerator.GenerateNewGame(options);
    }

    public GameState GetState()
    {
        return gameState;
    }

    public bool TryUndoLast()
    {
        gameState.GameStatistics.IncrementUndos();
        if (gameState.GameNodes.Count == 1)
            return false;
        _ = gameState.GameNodes.Pop();
        return true;
    }

    public bool TryInsertValue((int, int) position, byte value)
    {
        gameState.GameStatistics.IncrementInserts();
        GameNodes currentNode = gameState.GameNodes.Peek();
        if (!insertValidator.ValidateInsert(currentNode, position, value))
            return false;
        GameNodes nextNode = currentNode.Clone();
        valueInserter.InsertValue(nextNode, position, value);
        gameState.GameNodes.Push(nextNode);
        return true;
    }
    public void CheckConstraint(int constraintIndex)
    {
        gameState.GameStatistics.IncrementChecks();
        GameNodes currentState = gameState.GameNodes.Peek();
        constraintChecker.CheckConstraint(currentState, constraintIndex);
    }
}
