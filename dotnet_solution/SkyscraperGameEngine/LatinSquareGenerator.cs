namespace SkyscraperGameEngine;

class LatinSquareGenerator
{
    private readonly GameConstraintsFactory constraintsFactory = new();

    public byte[,] GenerateLatinSquare(int size, Random rng)
    {
        int[] permutation = [.. Enumerable.Range(0, size)];
        rng.Shuffle(permutation);
        byte[,] latinSquare = GetInitialSquare(size, permutation);
        rng.Shuffle(permutation);
        ReorderColumns(size, permutation, latinSquare);
        rng.Shuffle(permutation);
        ReorderRows(size, permutation, latinSquare);
        rng.Shuffle(permutation);
        ReorderSymbols(size, permutation, latinSquare);
        return latinSquare;
    }

    private static void ReorderColumns(int size, int[] permutation, byte[,] latinSquare)
    {
        for (int i = 0; i < size; i++)
        {
            int swapIdx = permutation[i];
            for (int j = 0; j < size; j++)
            {
                (latinSquare[j, swapIdx], latinSquare[j, i]) = (latinSquare[j, i], latinSquare[j, swapIdx]);
            }
        }
    }

    private static void ReorderRows(int size, int[] permutation, byte[,] latinSquare)
    {
        for (int j = 0; j < size; j++)
        {
            int swapIdx = permutation[j];
            for (int i = 0; i < size; i++)
            {
                (latinSquare[swapIdx, i], latinSquare[j, i]) = (latinSquare[j, i], latinSquare[swapIdx, i]);
            }
        }
    }

    private static void ReorderSymbols(int size, int[] permutation, byte[,] latinSquare)
    {
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                byte oldval = latinSquare[j, i];
                latinSquare[j, i] = (byte)(permutation[oldval - 1] + 1);
            }
        }
    }

    private byte[,] GetInitialSquare(int size, int[] permutation)
    {
        byte[,] latinSquare = new byte[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                int col = (j + permutation[i]) % size;
                latinSquare[i, col] = (byte)(j + 1);
            }
        }
        GameConstraints constraints = constraintsFactory.CreateEmptyConstraints(latinSquare);
        var rootNode = GameNode.CreateRootNode(constraints.GridConstraintMap, latinSquare);
        GameState gameState = new(constraints, rootNode);
        

        return latinSquare;
    }
}
