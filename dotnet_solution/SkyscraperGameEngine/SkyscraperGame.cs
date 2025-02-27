namespace SkyscraperGameEngine;

class SkyscraperGame
{
    private readonly Stack<GameState> gameStates;
    private readonly InstanceGenerator instanceGenerator;
    private readonly ConstraintChecker constraintChecker;
    private readonly InsertValidator insertValidator;
    private readonly ValueInserter valueInserter;

    public SkyscraperGame()
    {
        gameStates = new();
        instanceGenerator = new();
        constraintChecker = new();
        insertValidator = new();
        valueInserter = new();

        StartNewGame(new GameOptions());
    }

    public void StartNewGame(GameOptions options)
    {
        gameStates.Clear();
        gameStates.Push(instanceGenerator.GenerateNewGame(options));
    }

    public GameState GetCurrentState()
    {
        return gameStates.Peek();
    }

    public bool TryUndoLast()
    {
        if (gameStates.Count == 1)
            return false;
        _ = gameStates.Pop();
        return true;
    }

    public bool TryInsertValue((int, int) position, byte value)
    {
        GameState currentState = gameStates.Peek();
        if (!insertValidator.ValidateInsert(currentState, position, value))
            return false;
        GameState nextState = currentState.Clone();
        valueInserter.InsertValue(nextState, position, value);
        gameStates.Push(nextState);
        return true;
    }
    public void CheckConstraint(int constraintIndex)
    {
        GameState currentState = gameStates.Peek();
        constraintChecker.CheckConstraint(currentState, constraintIndex);
    }
}
