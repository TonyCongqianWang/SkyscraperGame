namespace SkyscraperGameEngine;

class GameStatistics
{
    public UInt128 NumInserts { get; private set; } = 0;
    public UInt128 NumUndos { get; private set; } = 0;
    public UInt128 NumChecks { get; private set; } = 0;

    public void IncrementInserts()
    {
        NumInserts++;
    }

    public void IncrementUndos()
    {
        NumUndos++;
    }

    public void IncrementChecks()
    {
        NumChecks++;
    }
}