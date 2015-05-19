// Copyright (c) Alex Ghiondea. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SudokuSolverLib
{
    public class SudokuGrid
    {
        private SudokuNode[,] Nodes;
        //private List<SudokuNode> Nodes;
        private bool hasSolution;
        private int boxWidth;
        private int boxHeight;
        private ulong winMask;

        public int BoxWidth { get { return boxWidth; } }
        public int BoxHeight { get { return boxHeight; } }

        public static SudokuGrid CreateGrid(string puzzle, int boxWidth, int boxHeight)
        {
            if (string.IsNullOrEmpty(puzzle))
                throw new ArgumentException("Puzzle cannot be empty", "puzzle");

            if (boxWidth <= 0 || boxHeight <= 0)
                throw new ArgumentException("Cannot have a negative or zero puzzle size");

            if (boxWidth > 16 || boxHeight > 16)
                throw new ArgumentException("Cannot have a puzzle box size larger than 16x16");

            return new SudokuGrid(puzzle, boxWidth, boxHeight);
        }

        private class MinSudokuHeap : Heap<SudokuNode>
        {
            public MinSudokuHeap(int storageSize)
                : base(storageSize)
            {

            }
            protected override bool Sorter(SudokuNode left, SudokuNode right)
            {
                return left.PossibleValuesCount < right.PossibleValuesCount;
            }
        }

        public bool SolveGrid()
        {
            Heap<SudokuNode> mh = new MinSudokuHeap(boxHeight * boxWidth * boxHeight * boxWidth);

            foreach (var node in Nodes)
            {
                if (!node.HasValue)
                {
                    mh.Insert(node);
                }
            }

            return solveGrid(mh);
        }

        public IEnumerable<SudokuPuzzleNode> GetSolution()
        {
            if (!hasSolution)
            {
                yield return null;
            }

            foreach (var item in Nodes)
            {
                yield return item.Node;
            }
        }

        private SudokuGrid(string puzzle, int width, int height)
        {
            this.boxHeight = height;
            this.boxWidth = width;
            this.winMask = ulong.MaxValue << (width * height);

            ParsePazzleToNodes(puzzle, width, height);
        }

        private void ParsePazzleToNodes(string puzzle, int width, int height)
        {
            CreateGridNodes(puzzle, width, height);

            ComputeNeighbours(width, height);
        }

        private void CreateGridNodes(string puzzle, int width, int height)
        {
            Nodes = new SudokuNode[height * width, height * width];

            int line = 0;

            // we do to upper to support HEX digits
            using (StringReader sr = new StringReader(puzzle.ToUpper()))
            {
                string lineText;
                while ((lineText = sr.ReadLine()) != null)
                {
                    int column = 0;

                    int textCol = 0;

                    while (textCol < lineText.Length)
                    {
                        while (char.IsWhiteSpace(lineText[textCol]))
                        {
                            textCol++;
                            if (textCol == lineText.Length)
                                break;
                        }
                        if (textCol == lineText.Length)
                            break;

                        //we got to either a digit or a .
                        char c = lineText[textCol];

                        SudokuNode node = CreateNode(line, column, c, width * height);

                        if (node == null)
                        {
                            throw new FormatException(string.Format("Unexpected character '{0}' while parsing puzzle (line {1} col {2})", c, line + 1, column + 1));
                        }

                        Nodes[line, column] = node;

                        column++;
                        textCol++;
                    }

                    // Making sure we have all the nodes we need
                    if (column != width * width)
                        throw new ArgumentException("There is a mismatch between the size of the grid and the nodes identified in the puzzle");

                    line++;
                }
            }

            // we need to make sure we have enough elements.
            if (line != height * height)
                throw new ArgumentException("There is a mismatch between the size of the grid and the nodes identified in the puzzle");
        }

        private static SudokuNode CreateNode(int line, int column, char c, int maxNodeValues)
        {
            if (char.IsDigit(c))
            {
                return new SudokuNode(line, column, maxNodeValues, (int)c - '0');
            }
            else if (char.IsLetter(c) && c >= 'A' && c <= 'Z')
            {
                return new SudokuNode(line, column, maxNodeValues, (int)c - 'A' + 10);
            }
            else if (c == '.')
            {
                return new SudokuNode(line, column, maxNodeValues);
            }

            return null;
        }

        private void ComputeNeighbours(int width, int height)
        {
            for (int line = 0; line < width * height; line++)
            {
                for (int col = 0; col < height * width; col++)
                {
                    List<SudokuNode> n = new List<SudokuNode>();
                    // add the line
                    // add the column
                    for (int i = 0; i < width * height; i++)
                    {
                        if (i != line)
                            n.Add(Nodes[i, col]);

                        if (i != col)
                            n.Add(Nodes[line, i]);
                    }

                    // add the box
                    for (int boxLine = (line / height) * height; boxLine < ((line / height) + 1) * height; boxLine++)
                    {
                        for (int boxCol = (col / width) * width; boxCol < ((col / width) + 1) * width; boxCol++)
                        {
                            if (boxLine != line && boxCol != col)
                                n.Add(Nodes[boxLine, boxCol]);
                        }
                    }

                    Nodes[line, col].SetNeighbours(n);

                    if (Nodes[line, col].HasValue)
                    {
                        Nodes[line, col].RemoveValueFromNeighbours(Nodes[line, col].Node.Value);
                    }
                }
            }
        }

        private bool ValidateSolution()
        {
            for (int i = 0; i < boxWidth * boxHeight; i++)
            {
                ulong rezLine = ulong.MaxValue;
                ulong rezCol = ulong.MaxValue;

                for (int line = 0; line < boxHeight * BoxWidth; line++)
                {
                    // we haven't finished
                    if (Nodes[line, i].Node.Value < 0)
                        return false;
                    rezLine &= (ulong)~(1 << (Nodes[line, i].Node.Value - 1));

                    // we haven't finished
                    if (Nodes[i, line].Node.Value < 0)
                        return false;
                    rezCol &= (ulong)~(1 << (Nodes[i, line].Node.Value - 1));
                }

                if (rezLine != winMask || rezCol != winMask)
                    return false;
            }

            // validate the boxes
            for (int line = 0; line < boxWidth; line++)
            {
                for (int col = 0; col < boxHeight; col++)
                {
                    ulong rezBox = ulong.MaxValue;
                    for (int boxLine = line * boxHeight; boxLine < (line + 1) * boxHeight; boxLine++)
                    {
                        for (int boxCol = col * boxWidth; boxCol < (col + 1) * boxWidth; boxCol++)
                        {
                            // we haven't finished
                            if (Nodes[boxLine, boxCol].Node.Value < 0)
                                return false;
                            rezBox &= (ulong)~(1 << (Nodes[boxLine, boxCol].Node.Value - 1));
                        }
                    }

                    if (rezBox != winMask)
                        return false;
                }
            }
            hasSolution = true;
            return true;
        }

        private bool IsSet(ulong possibleValues, int index)
        {
            return (possibleValues & (ulong)(1 << index)) != (ulong)(1 << index);
        }

        private bool solveGrid(Heap<SudokuNode> stillToFix)
        {
            // if we have a column with no potential values... that is bad :)
            if (stillToFix.IsEmpty)
            {
                //are we done?
                return ValidateSolution();
            }
            if (stillToFix.PeakAtRoot().PossibleValuesCount == 0)
            {
                return false;
            }

            var node = stillToFix.GetRoot();
            ulong values = node.PossibleValues;

            for (int i = 0; i < boxHeight * boxWidth; i++)
            {
                if (IsSet(values, i))
                {
                    continue;
                }

                int value = i + 1;
                node.Node.Value = value;
                node.HasValue = true;

                var changed = new List<SudokuNode>();
                foreach (var nn in node.neighbours)
                {
                    if (nn.HasValue == false)
                    {
                        if (nn.RemovePossibleValue(value))
                        {
                            // if we removed a value, we need to rebalance the heap
                            changed.Add(nn);
                        }
                    }
                }

                // we need to re-sort the heap after we made the changes.
                if (changed.Count > 0)
                {
                    stillToFix.Resort();
                }

                var solved = solveGrid(stillToFix);

                if (solved)
                    return true;

                foreach (var nn in changed)
                {
                    nn.AddPossibleValue(value);
                }

                if (changed.Count > 0)
                {
                    stillToFix.Resort();
                }

                node.HasValue = false;
            }

            stillToFix.Insert(node);

            return false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < boxHeight * boxWidth; i++)
            {
                for (int j = 0; j < boxHeight * boxWidth; j++)
                {
                    sb.Append(Nodes[i, j].HasValue ? Nodes[i, j].Node.ValueToChar() : '.');
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
