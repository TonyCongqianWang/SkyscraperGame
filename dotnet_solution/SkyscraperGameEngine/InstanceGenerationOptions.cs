namespace SkyscraperGameEngine;

public class InstanceGenerationOptions
{
    public int RandomSeed { get; set; } = -1;
    public int Size { get; set; } = 9;
    public double GridFillRate { get; set; } = 0.33;
    public double ConstraintFillRate { get; set; } = 0.9;
    public bool AllowInfeasible { get; set; } = false;
}
