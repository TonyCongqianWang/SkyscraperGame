namespace SkyscraperGameEngine;

class InsertValidator
{
    public bool ValidateInsert(GameNodes model, (int, int) position, byte value)
    {
        if (value < 1 || value > model.Size)
            return false;
        (int x, int y) = position;
        return !model.GridInvalidValues[x, y].Contains(value);
    }
}
