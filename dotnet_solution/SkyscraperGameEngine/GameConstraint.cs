using System.Collections.Immutable;

namespace SkyscraperGameEngine;

class GameConstraint(int Id, byte Value, ImmutableArray<(int, int)> Positions)
{
    public int Id { get; } = Id;
    public byte Value { get; } = Value;
    public ImmutableArray<(int, int)> Positions { get; } = Positions;
}
