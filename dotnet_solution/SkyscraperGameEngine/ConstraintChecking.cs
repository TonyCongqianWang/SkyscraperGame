namespace SkyscraperGameEngine;

class ConstraintChecking
{
    public static bool IsConstraintSatisfiable(int constraintValue,
                                               byte maxValue,
                                               IEnumerable<(byte, byte)> gridValueBounds)
    {
        return IsConstraintSatisfiableWithBounds(constraintValue, maxValue, gridValueBounds);
    }

    public static bool IsConstraintViolated(int constraintValue, IEnumerable<byte> gridValues)
    {
        int actualValue = CalculateConstraintValue(gridValues);
        return gridValues.All(v => v != 0) && actualValue != constraintValue;
    }

    public static int CalculateConstraintValue(IEnumerable<byte> gridValues)
    {
        int cValue = 0;
        int currentMax = 0;
        foreach (byte value in gridValues)
        {
            if (value > currentMax)
            {
                currentMax = value;
                cValue++;
            }
        }
        return cValue;
    }

    public static (int cValLb, int cValUb) CalculateConstraintBounds(
        IEnumerable<(byte, byte)> gridValueBounds,
        byte maxValue)
    {
        ConstraintBounds bounds = new(maxValue);

        int cValLb = 1;
        int cValUb = maxValue;
        byte index = 0;
        foreach ((byte lb, byte ub) in gridValueBounds)
        {
            index++;
            bounds.UpdateBounds(index, lb, ub);
            cValLb = Math.Max(cValLb, bounds.ConstraintValueLb);
            cValUb = Math.Min(cValUb, bounds.ConstraintValueUb);
        }
        return (cValLb, cValUb);
    }

    private static bool IsConstraintSatisfiableWithBounds(
        int constraintValue,
        byte maxValue,
        IEnumerable<(byte, byte)> gridValueBounds)
    {
        ConstraintBounds bounds = new(maxValue);

        if (constraintValue == 0)
            return true;
        byte index = 0;
        foreach ((byte lb, byte ub) in gridValueBounds)
        {
            index++;
            bounds.UpdateBounds(index, lb, ub);
            if (bounds.CheckViolation(constraintValue))
                return false;
            if (bounds.ReachedMaxValue())
                break;
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

        public void UpdateBounds(byte index, byte lb, byte ub)
        {
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
        }

        public readonly bool ReachedMaxValue()
        {
            return CurrentMaxLb == maximumValue;
        }
    }
}
