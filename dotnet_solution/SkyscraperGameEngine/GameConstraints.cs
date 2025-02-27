using System.Collections.Immutable;

namespace SkyscraperGameEngine;

public class GameConstraints(ImmutableArray<GameConstraint> constraints, ImmutableArray<GameConstraint>[,] constraintsPerGridCell)
{
    public ImmutableArray<GameConstraint> Constraints { get; } = constraints;
    public ImmutableArray<GameConstraint>[,] ContraintsPerGridCell { get; } = constraintsPerGridCell;
}
