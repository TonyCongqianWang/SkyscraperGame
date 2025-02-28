using System.Collections.Immutable;

namespace SkyscraperGameEngine;

using GridContraintMap = ImmutableDictionary<(int, int), ImmutableArray<GameConstraint>>;

class GameConstraints(ImmutableArray<GameConstraint> constraints, GridContraintMap gridConstraintMap)
{
    public ImmutableArray<GameConstraint> Constraints { get; } = constraints;
    public GridContraintMap GridConstraintMap { get; } = gridConstraintMap;
}
