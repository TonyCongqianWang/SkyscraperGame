﻿using System.Collections.Immutable;
using System.Data;

namespace SkyscraperGameEngine;

using GridContraintMap = ImmutableDictionary<(int, int), ImmutableArray<GameConstraint>>;

class InstanceGenerator
{
    private Random random = new();

    public GameState GenerateNewGame(InstanceGenerationOptions options)
    {
        if (options.RandomSeed >= 0)
            random = new(options.RandomSeed);
        byte[,] latinSquare = GenerateLatinSquare(options.Size);
        GameConstraints constraints = CreateConstraints(options, latinSquare);
        RemoveRandomValuesFromGrid(options, latinSquare);
        GameNode initialState = GameNode.CreateRootNode(constraints.GridConstraintMap, latinSquare);
        Stack<GameNode> gridStates = new();
        gridStates.Push(initialState);
        return new(constraints, gridStates, new());
    }

    private void RemoveRandomValuesFromGrid(InstanceGenerationOptions options, byte[,] latinSquare)
    {
        int size = latinSquare.GetLength(0);
        int numKeep = (int)(size * size * Math.Min(options.GridFillRate, 1.0));
        HashSet<(int, int)> allPositions = [.. (from i in Enumerable.Range(0, size)
                                    from j in Enumerable.Range(0, size)
                                    select (i, j))];

        if (options.AllowInfeasible)
        {
            int[] removeColumn = [.. Enumerable.Range(0, size)];
            random.Shuffle(removeColumn);
            foreach (int row in Enumerable.Range(0, size))
            {
                latinSquare[row, removeColumn[row]] = 0;
                allPositions.Remove((row, removeColumn[row]));
            }
            numKeep += size;
            if (numKeep > size * size)
                return;
        }

        (int, int)[] removePositions = [.. allPositions];
        random.Shuffle(removePositions);
        foreach ((int i, int j) in removePositions.Take(size * size - numKeep))
        {
            latinSquare[i, j] = 0;
        }
    }

    private byte[,] GenerateLatinSquare(int size)
    {
        int[] permutation = [.. Enumerable.Range(0, size)];
        random.Shuffle(permutation);
        byte[,] latinSquare = new byte[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                int col = (j + permutation[i]) % size;
                latinSquare[i, col] = (byte)(j + 1);
            }
        }
        random.Shuffle(permutation);
        for (int i = 0; i < size; i++)
        {
            int swapIdx = permutation[i];
            for (int j = 0; j < size; j++)
            {
                (latinSquare[j, swapIdx], latinSquare[j, i]) = (latinSquare[j, i], latinSquare[j, swapIdx]);
            }
        }
        random.Shuffle(permutation);
        for (int j = 0; j < size; j++)
        {
            int swapIdx = permutation[j];
            for (int i = 0; i < size; i++)
            {
                (latinSquare[swapIdx, i], latinSquare[j, i]) = (latinSquare[j, i], latinSquare[swapIdx, i]);
            }
        }
        return latinSquare;
    }

    private GameConstraints CreateConstraints(InstanceGenerationOptions options, byte[,] latinSquare)
    {
        int size = options.Size;
        int numConstraints = (int)(Math.Min(options.ConstraintFillRate, 1.0) * size * 4);
        numConstraints = Math.Max(numConstraints, 0);
        int[] keepIndeces = [.. Enumerable.Range(0, 4 * size)];
        int[] modifyIndeces = [];
        random.Shuffle(keepIndeces);
        keepIndeces = [.. keepIndeces.Take(numConstraints)];
        if (options.AllowInfeasible && numConstraints > 0)
            modifyIndeces = random.GetItems(keepIndeces, 1);

        GameConstraint[] constraints = new GameConstraint[size * 4];

        Dictionary<(int, int), List<GameConstraint>> gridContraintMap = [];
        foreach (int col in Enumerable.Range(0, size))
        {
            foreach (int row in Enumerable.Range(0, size))
            {
                gridContraintMap[(row, col)] = [];
            }
        }
        for (int constraintIdx = 0; constraintIdx < 4 * size; constraintIdx++)
        {
            if (!keepIndeces.Contains(constraintIdx))
            {
                constraints[constraintIdx] = new GameConstraint(constraintIdx, 0, []);
                continue;
            }
            ImmutableArray<(int, int)> gridPositions;
            if (constraintIdx < 1 * size)
            {
                int col = constraintIdx % size;
                gridPositions = [.. (from row in Enumerable.Range(0, size)
                                                            select (row, col))];
            }
            else if (constraintIdx < 2 * size)
            {
                int col = constraintIdx % size;
                gridPositions = [.. (from row in Enumerable.Range(0, size).Reverse()
                                                            select (row, col))];
            }
            else if (constraintIdx < 3 * size)
            {
                int row = constraintIdx % size;
                gridPositions = [.. (from col in Enumerable.Range(0, size)
                                                            select (row, col))];
            }
            else
            {
                int row = constraintIdx % size;
                gridPositions = [.. (from col in Enumerable.Range(0, size).Reverse()
                                                            select (row, col))];
            }
            int constraintValue = CalculateConstraintValue(latinSquare, gridPositions);
            if (modifyIndeces.Contains(constraintIdx))
            {
                constraintValue += random.NextDouble() < 0.33 ? 1 : -1;
                constraintValue = Math.Min(constraintValue, size);
                constraintValue = Math.Max(constraintValue, 1);
            }
            constraints[constraintIdx] = new GameConstraint(constraintIdx, (byte)constraintValue, gridPositions);
            foreach (var pos in gridPositions)
                gridContraintMap[pos].Add(constraints[constraintIdx]);

        }
        GridContraintMap.Builder mappingBuilder = ImmutableDictionary.CreateBuilder<(int, int), ImmutableArray<GameConstraint>>();
        foreach (var (key, value) in gridContraintMap)
            mappingBuilder.Add(key, [.. value]);
        return new([.. constraints], mappingBuilder.ToImmutableDictionary());
    }

    private int CalculateConstraintValue(byte[,] latinSquare, ImmutableArray<(int, int)> gridPositions)
    {
        int value = 0;
        int currentMax = 0;
        foreach ((int i, int j) in gridPositions)
        {
            if (latinSquare[i, j] > currentMax)
            {
                currentMax = latinSquare[i, j];
                value++;
            }
        }
        return value;
    }
}
