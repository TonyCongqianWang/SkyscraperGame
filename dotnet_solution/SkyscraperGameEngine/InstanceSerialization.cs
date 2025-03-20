using System.Text;

namespace SkyscraperGameEngine;

static class InstanceSerialization
{
    public static string SerializePuzzle(this GameState state)
    {
        StringBuilder stringBuilder = new();
        stringBuilder.Append('\"');
        foreach (var constr in state.GameConstraints.Constraints)
        {
            stringBuilder.Append(constr.Value);
            stringBuilder.Append(' ');
        }
        stringBuilder[^1] = '\"';
        stringBuilder.Append(' ');
        stringBuilder.Append('\"');
        var node = state.GameNodes.Peek();
        foreach (int i in Enumerable.Range(0, node.Size))
        {
            foreach (int j in Enumerable.Range(0, node.Size))
            {
                stringBuilder.Append(node.GridValues[i, j]);
                stringBuilder.Append(' ');
            }
        }
        stringBuilder[^1] = '\"';
        return stringBuilder.ToString();
    }

    public static GameState? TryDeserializePuzzle(string puzzlestr)
    {
        string[] parts = puzzlestr.Split('\"', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 1 && parts.Length != 3)
            return null;
        string[] constraintStrValues = parts[0].Split(' ');
        if (constraintStrValues.Length % 4 != 0)
            return null;
        int puzzleSize = constraintStrValues.Length / 4;
        byte[,] initialGrid = new byte[puzzleSize, puzzleSize];
        if (parts.Length == 3)
        {
            string[] gridValues = parts[2].Split(" ");
            if (gridValues.Length != puzzleSize * puzzleSize)
                return null;
            for (int i = 0; i < gridValues.Length; i++)
            {
                if (!byte.TryParse(gridValues[i], out byte gridVal)
                    || gridVal > puzzleSize)
                    return null;
                initialGrid[i / puzzleSize, i % puzzleSize] = gridVal;
            }
        }
        int[] constraintValues = new int[4 * puzzleSize];
        for (int i = 0; i < constraintValues.Length; i++)
        {
            if (!int.TryParse(constraintStrValues[i], out int constrVal))
                return null;
            constraintValues[i] = constrVal;
        }
        GameConstraints constraints = GameConstraintsFactory.CreateGameConstraints(constraintValues);
        GameNode rootNode = GameNode.CreateRootNode(constraints.GridConstraintMap, initialGrid);
        return new(constraints, rootNode);
    }
}
