
namespace SkyscraperGameEngine;

class ValueInserter
{
    internal void InsertValue(GameState state, (int, int) position, byte value)
    {
        (int x, int y) = position;
        state.CurrentDepth++;
        state.NumInserts++;
        for (int row = 0; row < state.Size; row++)
            state.GridValidValues[row, y].Remove(value);
        for (int col = 0; col < state.Size; col++)
            state.GridValidValues[x, col].Remove(value);
        state.GridValidValues[x, y].Clear();
        state.GridValues[x, y] = value;
    }

    internal void UpdateSolveStatus(GameState gameState)
    {
        if (gameState.NumInserts == gameState.Size * gameState.Size)
            gameState.IsSolved = true;
    }
}
