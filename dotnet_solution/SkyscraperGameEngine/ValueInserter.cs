
namespace SkyscraperGameEngine;

class ValueInserter
{
    internal void InsertValue(GameNodes state, (int, int) position, byte value)
    {
        (int x, int y) = position;
        state.CurrentDepth++;
        state.NumInserts++;
        state.LastInsertPosition = position;
        for (int row = 0; row < state.Size; row++)
            state.GridInvalidValues[row, y].Add(value);
        for (int col = 0; col < state.Size; col++)
            state.GridInvalidValues[x, col].Add(value);
        state.GridValues[x, y] = value;
        for (value = 1; value <= state.Size; value++)
            state.GridInvalidValues[x, y].Add(value);
    }

    internal void UpdateSolveStatus(GameNodes gameState)
    {
        if (gameState.NumInserts == gameState.NumCells)
            gameState.IsSolved = true;
    }
}
