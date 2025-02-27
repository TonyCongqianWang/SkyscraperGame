namespace SkyscraperGameEngine;

public class GameState(GameConstraints gameConstraints, Stack<GameNodes> gridStates, GameStatistics gameStatistics)
{
    public int GameSize => GameNodes.Peek().Size;
    public GameConstraints GameConstraints { get; } = gameConstraints;
    public Stack<GameNodes> GameNodes { get; } = gridStates;
    public GameStatistics GameStatistics { get; } = gameStatistics;
}
