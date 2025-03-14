namespace SkyscraperGameEngine;

public class GameInterface
{
    private readonly GameEngine engine;

    private readonly InstanceGenerator instanceGenerator;

    public GameInterface()
    {
        instanceGenerator = new();
        engine = new(instanceGenerator.GenerateNewGame(new()));
    }

    public string StartNewGame(InstanceGenerationOptions options)
    {
        engine.GameState = instanceGenerator.GenerateNewGame(options);
        return InstanceSerialization.SerializePuzzle(engine.GameState);
    }

    public string? StartNewGame(string puzzleString)
    {
        GameState? gameState = InstanceSerialization.TryDeserializePuzzle(puzzleString);
        if (gameState == null)
            return null;
        engine.GameState = gameState;
        return InstanceSerialization.SerializePuzzle(engine.GameState);
    }

    public string GenerateNewGame(InstanceGenerationOptions options)
    {
        GameState state = instanceGenerator.GenerateNewGame(options);
        return InstanceSerialization.SerializePuzzle(state);
    }

    public GameObservation GetState()
    {
        return new(engine.GameState);
    }

    public bool TryUndoLast()
    {
        return engine.TryUndoLast();
    }

    public bool TryInsertValue((int, int) position, byte value)
    {
        return engine.TryInsertValue(position, value);
    }
    public bool TryCheckConstraint(int constraintIndex)
    {
        return engine.TryCheckConstraint(constraintIndex);
    }
}
