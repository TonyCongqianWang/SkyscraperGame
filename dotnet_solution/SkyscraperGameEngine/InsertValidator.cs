namespace SkyscraperGameEngine;

class InsertValidator
{
    public bool ValidateInsert(GameState model, (int, int) position, byte value)
    {
        (int x, int y) = position;
        return model.GridValidValues[x, y].Contains(value);
    }
}
