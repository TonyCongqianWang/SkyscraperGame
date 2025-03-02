namespace SkyscraperGameEngine;

class GameOptions
{
    public bool AutoCheckConstraintsAfterInsert { get; set; } = false;
    public bool AutoUndoWhenInfeasible { get; set; } = false;
    public bool AutoInsertUnambiguous { get; set; } = false;

    public GameOptions(string? preset = null)
    {
        if (preset == "solver")
        {
            AutoInsertUnambiguous = true;
        }
        if (preset == "all_automations")
        {
            AutoInsertUnambiguous = true;
            AutoUndoWhenInfeasible = true;
            AutoInsertUnambiguous = true;
        }
    }
}
