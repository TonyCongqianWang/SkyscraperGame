using System.Collections.Immutable;

namespace SkyscraperGameEngine;

class GameConstraint(int Id, byte Value, ImmutableArray<(int, int)> Positions)
{
    public int Id { get; } = Id;
    public byte Value { get; } = Value;
    public ImmutableArray<(int, int)> Positions { get; } = Positions;

    public bool IsViolated(GameNode node)
    {
        return ConstraintChecking.IsConstraintViolated(Value, node.GetGridValues(Positions));
    }

    public bool IsSatisfiable(GameNode node)
    {
        return ConstraintChecking.IsConstraintSatisfiable(Value, (byte)(Positions.Length + 1), node.GetGridValueBounds(Positions));
    }
}
