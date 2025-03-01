namespace SkyscraperGameEngine;

public class GameInterface
{
    private readonly GameEngine engine;

    private readonly InstanceGenerator instanceGenerator;

    public GameInterface()
    {
        instanceGenerator = new();
        engine = new(instanceGenerator.GenerateNewGame(new()));

        StartNewGame(new InstanceGenerationOptions());
    }

    public void StartNewGame(InstanceGenerationOptions options)
    {
        engine.GameState = instanceGenerator.GenerateNewGame(options);
    }

    public GameStateViewModel GetState()
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
