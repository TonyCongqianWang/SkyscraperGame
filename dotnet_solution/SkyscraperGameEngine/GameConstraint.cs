using System.Collections.Immutable;

namespace SkyscraperGameEngine;

public class GameConstraint(byte Value, ImmutableArray<(int, int)> Positions)
{
    public byte Value { get; } = Value;
    public ImmutableArray<(int, int)> Positions { get; } = Positions;
}
