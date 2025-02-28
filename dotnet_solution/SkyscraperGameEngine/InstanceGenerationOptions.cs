namespace SkyscraperGameEngine;

public class InstanceGenerationOptions
{
    private int _size = 9;

    public int RandomSeed { get; set; } = -1;
    public int Size
    {
        get { return _size; }
        set { _size = Math.Max(Math.Min(value, 9), 4); }
    }
    public double GridFillRate { get; set; } = 0.33;
    public double ConstraintFillRate { get; set; } = 0.9;
    public bool AllowInfeasible { get; set; } = false;
}
