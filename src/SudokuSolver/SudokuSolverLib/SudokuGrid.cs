// Copyright (c) Alex Ghiondea. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SudokuSolverLib
{
    public class SudokuGrid
    {
        private SudokuNode[,] nodes;
        private bool hasSolution;
        private readonly int boxWidth;
        private readonly int boxHeight;
        private readonly ulong winMask;

        public int BoxWidth { get { return boxWidth; } }
        public int BoxHeight { get { return boxHeight; } }
        public bool HasSolution { get { return hasSolution; } }

        #region Factory methods and ctors
        public static SudokuGrid FromPuzzle(string puzzle, int boxWidth, int boxHeight)
        {
            if (string.IsNullOrEmpty(puzzle))
                throw new ArgumentException("Puzzle cannot be empty", "puzzle");

            if (boxWidth <= 0 || boxHeight <= 0)
                throw new ArgumentException("Cannot have a negative or zero puzzle size");

            if (boxWidth > 16 || boxHeight > 16)
                throw new ArgumentException("Cannot have a puzzle box size larger than 16x16");

            return new SudokuGrid(puzzle, boxWidth, boxHeight);
        }

        public static SudokuGrid CreatePuzzle(int boxWidth, int boxHeight, int hintsCount)
        {
            // Create a new grid by solving an empty grid using a randomized order of picking values
            SudokuGrid grid = new SudokuGrid(boxWidth, boxHeight);
            Random r = new Random((int)DateTime.Now.Ticks);
            grid.SolveGrid(ArrayHelpers.CreateArrayOfRandomValues(r, boxWidth * boxHeight));

            // Create a new grid by taking hintsCount hints from the solved grid.
            int[] hintsLocation = ArrayHelpers.CreateArrayOfRandomValues(r, boxHeight * boxWidth * boxHeight * boxWidth);

            SudokuGrid resultGrid = new SudokuGrid(boxWidth, boxHeight);

            var x = grid.GetNodes().ToArray();
            for (int i = 0; i < hintsCount; i++)
            {
                resultGrid.SetValue(x[hintsLocation[i]].Line, x[hintsLocation[i]].Column, x[hintsLocation[i]].Value);
            }

            return resultGrid;
        }

        private SudokuGrid(int boxWidth, int boxHeight)
        {
            this.boxHeight = boxHeight;
            this.boxWidth = boxWidth;
            winMask = ulong.MaxValue << (boxWidth * boxHeight);

            // Create the nodes
            nodes = new SudokuNode[boxHeight * boxWidth, boxHeight * boxWidth];
            for (int i = 0; i < boxHeight * boxWidth; i++)
            {
                for (int j = 0; j < boxHeight * boxWidth; j++)
                {
                    nodes[i, j] = new SudokuNode(i, j, boxHeight * boxWidth);
                }
            }

            ComputeNeighbours(boxWidth, boxHeight);
        }

        private SudokuGrid(string puzzle, int width, int height)
            : this(width, height)
        {
            CreateGridNodesFromPuzzle(puzzle, width, height);
        }
        #endregion 

        public void SetValue(int line, int column, int value)
        {
            nodes[line, column].Node.Value = value;
            nodes[line, column].Node.PartOfPuzzle = true;
            nodes[line, column].HasValue = true;
            nodes[line, column].RemoveValueFromNeighbours(value);
        }

        public IEnumerable<SudokuPuzzleNode> GetNodes()
        {
            foreach (var item in nodes)
            {
                yield return item.Node;
            }
        }

        public bool SolveGrid()
        {
            return SolveGrid(ArrayHelpers.CreateOrderedArray(boxHeight * boxWidth));
        }

        private bool SolveGrid(int[] orderedValues)
        {
            return SolveGridInternal(CreateHeapForPuzzle(), orderedValues);
        }

        private bool SolveGridInternal(Heap<SudokuNode> stillToFix, int[] orderedValues)
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

            // depending if we are creating a puzzle or solving one
            node.HasValue = true;
            for (int i = 0; i < boxHeight * boxWidth; i++)
            {
                if (IsSet(values, orderedValues[i]))
                {
                    continue;
                }

                if (TrySetValue(node, orderedValues[i] + 1, stillToFix, orderedValues))
                    return true;
            }

            node.HasValue = false;

            stillToFix.Insert(node);

            return false;
        }

        private bool TrySetValue(SudokuNode node, int value, Heap<SudokuNode> stillToFix, int[] orderedValues)
        {
            node.Node.Value = value;

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

            var solved = SolveGridInternal(stillToFix, orderedValues);

            if (solved)
                return true;

            //recover the previous state
            foreach (var nn in changed)
            {
                nn.AddPossibleValue(value);
            }

            if (changed.Count > 0)
            {
                stillToFix.Resort();
            }

            return false;
        }

        private void CreateGridNodesFromPuzzle(string puzzle, int width, int height)
        {
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
                            {
                                break;
                            }
                        }
                        if (textCol == lineText.Length)
                        {
                            break;
                        }

                        //we got to either a digit or a .
                        char c = lineText[textCol];
                        int value = -1;

                        if (char.IsDigit(c))
                        {
                            value = c - '0';
                        }
                        else if (char.IsLetter(c) && c >= 'A' && c <= 'Z')
                        {
                            value = c - 'A' + 10;
                        }
                        else if (c != '.')
                        {
                            throw new FormatException(string.Format("Unexpected character '{0}' while parsing puzzle (line {1} col {2})", c, line + 1, textCol + 1));
                        }

                        if (column >= boxHeight * boxWidth)
                        {
                            throw new ArgumentException("There is a mismatch between the size of the grid and the nodes identified in the puzzle");
                        }

                        nodes[line, column].SetValue(value);

                        column++;
                        textCol++;
                    }

                    // Making sure we have all the nodes we need
                    if (column != width * height)
                        throw new ArgumentException("There is a mismatch between the size of the grid and the nodes identified in the puzzle");

                    line++;
                }
            }

            // we need to make sure we have enough elements.
            if (line != width * height)
                throw new ArgumentException("There is a mismatch between the size of the grid and the nodes identified in the puzzle");
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
                            n.Add(nodes[i, col]);

                        if (i != col)
                            n.Add(nodes[line, i]);
                    }

                    // add the box
                    for (int boxLine = (line / height) * height; boxLine < ((line / height) + 1) * height; boxLine++)
                    {
                        for (int boxCol = (col / width) * width; boxCol < ((col / width) + 1) * width; boxCol++)
                        {
                            if (boxLine != line && boxCol != col)
                                n.Add(nodes[boxLine, boxCol]);
                        }
                    }

                    nodes[line, col].SetNeighbours(n);

                    if (nodes[line, col].HasValue)
                    {
                        nodes[line, col].RemoveValueFromNeighbours(nodes[line, col].Node.Value);
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
                    if (nodes[line, i].Node.Value < 0)
                        return false;
                    rezLine &= (ulong)~(1 << (nodes[line, i].Node.Value - 1));

                    // we haven't finished
                    if (nodes[i, line].Node.Value < 0)
                        return false;
                    rezCol &= (ulong)~(1 << (nodes[i, line].Node.Value - 1));
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
                            if (nodes[boxLine, boxCol].Node.Value < 0)
                                return false;
                            rezBox &= (ulong)~(1 << (nodes[boxLine, boxCol].Node.Value - 1));
                        }
                    }

                    if (rezBox != winMask)
                        return false;
                }
            }
            hasSolution = true;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSet(ulong possibleValues, int index)
        {
            return (possibleValues & (ulong)(1 << index)) != (ulong)(1 << index);
        }

        private Heap<SudokuNode> CreateHeapForPuzzle()
        {
            Heap<SudokuNode> mh = new MinSudokuHeap(boxHeight * boxWidth * boxHeight * boxWidth);

            foreach (var node in nodes)
            {
                if (!node.HasValue)
                {
                    mh.Insert(node);
                }
            }

            return mh;
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < boxHeight * boxWidth; i++)
            {
                for (int j = 0; j < boxHeight * boxWidth; j++)
                {
                    sb.Append(nodes[i, j].HasValue ? nodes[i, j].Node.ValueToChar() : '.');
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
