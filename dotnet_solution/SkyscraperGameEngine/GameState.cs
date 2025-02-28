﻿namespace SkyscraperGameEngine;


class GameState(GameConstraints gameConstraints, Stack<GameNode> gridStates, GameStatistics gameStatistics)
{
    public int GameSize => GameNodes.Peek().Size;
    public GameConstraints GameConstraints { get; } = gameConstraints;
    public Stack<GameNode> GameNodes { get; } = gridStates;
    public GameStatistics GameStatistics { get; } = gameStatistics;
}
