using System.Drawing;

namespace SkyscraperGameEngine;

class ConstraintChecker
{
    public bool IsConstraintSatisfiable(int constraintValue, byte maxValue, IEnumerable<(byte, byte)> gridValueBounds)
    {
        int valueLb = 1;
        int valueLhsUb = 0;
        byte currentMaxLb = 0;
        byte currentMaxUb = 0;

        byte index = 0;
        foreach ((byte lb, byte ub) in gridValueBounds)
        {
            if (ub > currentMaxLb)
            {
                valueLhsUb++;
                if (lb > currentMaxUb)
                {
                    currentMaxLb = lb;
                    currentMaxUb = ub;
                    if (currentMaxUb < maxValue)
                        valueLb++;
                }
                else
                {
                    currentMaxLb = Math.Max(currentMaxLb, lb);
                    currentMaxUb = Math.Max(currentMaxUb, ub);
                }
            }
            index++;
            currentMaxLb = Math.Max(currentMaxLb, index);
            if (valueLb > constraintValue || valueLhsUb + maxValue - currentMaxLb < constraintValue)
                return false;
            if (currentMaxLb == maxValue)
                break;
        }
        return true;
    }
}
