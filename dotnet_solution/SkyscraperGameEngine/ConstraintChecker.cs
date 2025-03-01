namespace SkyscraperGameEngine;

class ConstraintChecker
{
    public bool IsConstraintSatisfiable(int constraintValue,
                                               byte maxValue,
                                               IEnumerable<(byte, byte)> gridValueBounds)
    {
        ConstraintBounds bounds = new(maxValue);

        byte index = 0;
        foreach ((byte lb, byte ub) in gridValueBounds)
        {
            index++;
            if (!bounds.TryUpdateBounds(index, lb, ub))
                break;
            if (bounds.CheckViolation(constraintValue))
                return false;
        }
        return true;
    }

    private struct ConstraintBounds(int maximumValue)
    {
        public readonly int maximumValue = maximumValue;
        public int ConstraintValueLb { get; set; } = 1;
        public readonly int ConstraintValueUb => ConstraintValueUbPartial - CurrentMaxLb + maximumValue;
        public int ConstraintValueUbPartial { get; set; } = 0;
        public byte CurrentMaxLb { get; set; } = 0;
        public byte CurrentMaxUb { get; set; } = 0;

        public readonly bool CheckViolation(int constraintValue)
        {
            return (ConstraintValueLb > constraintValue || ConstraintValueUb < constraintValue);
        }

        public bool TryUpdateBounds(byte index, byte lb, byte ub)
        {
            if (CurrentMaxLb == maximumValue)
                return false;
            if (ub > CurrentMaxLb)
            {
                ConstraintValueUbPartial++;
                if (lb > CurrentMaxUb)
                {
                    CurrentMaxLb = lb;
                    CurrentMaxUb = ub;
                    if (CurrentMaxUb < maximumValue)
                        ConstraintValueLb++;
                }
                else
                {
                    CurrentMaxLb = Math.Max(CurrentMaxLb, lb);
                    CurrentMaxUb = Math.Max(CurrentMaxUb, ub);
                }
            }
            CurrentMaxLb = Math.Max(CurrentMaxLb, index);
            return true;
        }
    }
}
