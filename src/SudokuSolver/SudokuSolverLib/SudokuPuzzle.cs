// Copyright (c) Alex Ghiondea. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SudokuSolverLib.Helpers;
using SudokuSolverLib.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SudokuSolverLib
{
    public class SudokuPuzzle
    {
        private SudokuInternalNode[,] nodes;
        private bool hasSolution;
        private readonly int boxWidth;
        private readonly int boxHeight;
        private readonly int possibleNodeValueCount;
        private readonly ulong winMask;

        public int BoxWidth { get { return boxWidth; } }
        public int BoxHeight { get { return boxHeight; } }
        public bool HasSolution { get { return hasSolution; } }

        #region Factory methods and ctors
        public static SudokuPuzzle FromString(string puzzle, int boxWidth, int boxHeight)
        {
            if (string.IsNullOrEmpty(puzzle))
                throw new ArgumentException("Puzzle cannot be empty", "puzzle");

            if (boxWidth <= 0 || boxHeight <= 0)
                throw new ArgumentException("Cannot have a negative or zero puzzle size");

            if (boxWidth > 5 || boxHeight > 5)
                throw new ArgumentException("Cannot have a puzzle box size larger than 16x16");

            return new SudokuPuzzle(puzzle, boxWidth, boxHeight);
        }

        public static SudokuPuzzle Create(int boxWidth, int boxHeight, int hintsCount)
        {
            if (boxWidth <= 0 || boxHeight <= 0)
                throw new ArgumentException("Invalid box size");

            if (boxWidth > 5 || boxHeight > 5)
                throw new ArgumentException("Cannot have a puzzle box size larger than 16x16");

            if (hintsCount < 0 || hintsCount > (boxHeight * boxHeight * boxWidth * boxWidth))
                throw new ArgumentException("Invalid hints count");

            Random randomizer = new Random((int)DateTime.Now.Ticks);
            ValuesProvider randomizedValuesProvider = new ValuesProvider(boxWidth * boxHeight, randomizer);

            // Create a new grid by solving an empty grid using a randomized order of picking values
            SudokuPuzzle grid = new SudokuPuzzle(boxWidth, boxHeight);
            grid.SolveGrid(randomizedValuesProvider);

            // Create a new grid by taking hintsCount hints from the solved grid.
            int[] hintsLocation = ArrayHelpers.RandomizedRange(boxHeight * boxWidth * boxHeight * boxWidth, randomizer);

            SudokuPuzzle resultGrid = new SudokuPuzzle(boxWidth, boxHeight);

            var x = grid.GetNodes().ToArray();
            for (int i = 0; i < hintsCount; i++)
            {
                resultGrid.SetValue(x[hintsLocation[i]].Line, x[hintsLocation[i]].Column, x[hintsLocation[i]].Value, true);
            }

            return resultGrid;
        }

        private SudokuPuzzle(int boxWidth, int boxHeight)
        {
            this.boxHeight = boxHeight;
            this.boxWidth = boxWidth;
            possibleNodeValueCount = boxHeight * boxWidth;
            winMask = ulong.MaxValue << possibleNodeValueCount;

            // Create the nodes
            nodes = new SudokuInternalNode[possibleNodeValueCount, possibleNodeValueCount];
            for (int i = 0; i < possibleNodeValueCount; i++)
            {
                for (int j = 0; j < possibleNodeValueCount; j++)
                {
                    nodes[i, j] = new SudokuInternalNode(i, j, possibleNodeValueCount);
                }
            }
        }

        private SudokuPuzzle(string puzzle, int width, int height)
            : this(width, height)
        {
            CreateGridNodesFromPuzzle(puzzle, width, height);

            // Update the neighbours based on the information in the puzzle.
            for (int line = 0; line < width * height; line++)
            {
                for (int col = 0; col < height * width; col++)
                {
                    if (nodes[line, col].Value > SudokuInternalNode.NO_VALUE)
                    {
                        RemoveValueFromNeighbours(line, col, nodes[line, col].Value);
                    }
                }
            }
        }
        #endregion 

        private void SetValue(int line, int column, int value, bool partOfPuzzle)
        {
            SudokuInternalNode node = nodes[line, column];
            nodes[line, column].Value = value;
            nodes[line, column].PartOfPuzzle = partOfPuzzle;
            RemoveValueFromNeighbours(line, column, value);
        }

        public IEnumerable<SudokuNode> GetNodes()
        {
            foreach (var item in nodes)
            {
                yield return new SudokuNode(item.Line, item.Column, item.Value, item.PartOfPuzzle);
            }
        }

        #region Solve the puzzle
        public bool SolveGrid()
        {
            return SolveGrid(new ValuesProvider(possibleNodeValueCount, null));
        }

        /// <summary>
        /// This is an internal helper to create the minheap and use the values
        /// </summary>
        /// <param name="orderedValues"></param>
        /// <returns></returns>
        private bool SolveGrid(ValuesProvider values)
        {
            return SolveGridInternal(MinHeapFromGrid(), values);
        }

        /// <summary>
        /// Drives the back-tracking algorithm
        /// </summary>
        private bool SolveGridInternal(Heap<SudokuInternalNode> stillToFix, ValuesProvider valuesProvider)
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

            int[] orderedValues = valuesProvider.GetValues();
            // depending if we are creating a puzzle or solving one
            for (int i = 0; i < possibleNodeValueCount; i++)
            {
                if (IsSet(values, orderedValues[i]))
                {
                    continue;
                }

                if (TrySetValue(node, orderedValues[i] + 1, stillToFix, valuesProvider))
                    return true;
            }

            node.Value = SudokuInternalNode.NO_VALUE;

            stillToFix.Insert(node);

            return false;
        }

        /// <summary>
        /// Tries to set a value for a node. 
        /// </summary>
        /// <returns>True if the value is in the right location and the puzzle is solved</returns>
        private bool TrySetValue(SudokuInternalNode node, int value, Heap<SudokuInternalNode> stillToFix, ValuesProvider valuesProvider)
        {
            node.Value = value;

            var updatedNodes = RemoveValueFromNeighbours(node.Line, node.Column, node.Value); // new List<SudokuNode>();

            // we need to re-sort the heap after we made the changes.
            if (updatedNodes.Count > 0)
            {
                stillToFix.Resort();
            }

            var solved = SolveGridInternal(stillToFix, valuesProvider);

            if (solved)
                return true;

            //recover the previous state
            foreach (var nn in updatedNodes)
            {
                nn.AddPossibleValue(value);
            }

            if (updatedNodes.Count > 0)
            {
                stillToFix.Resort();
            }

            return false;
        }

        private List<SudokuInternalNode> RemoveValueFromNeighbours(int line, int col, int value)
        {
            List<SudokuInternalNode> updatedNodes = new List<SudokuInternalNode>();

            SudokuInternalNode node;
            // Check the line and column
            for (int i = 0; i < possibleNodeValueCount; i++)
            {
                if (i != line)
                {
                    node = nodes[i, col];
                    if (node.Value == SudokuInternalNode.NO_VALUE && node.RemovePossibleValue(value))
                    {
                        updatedNodes.Add(node);
                    }
                }

                if (i != col)
                {
                    node = nodes[line, i];
                    if (node.Value == SudokuInternalNode.NO_VALUE && node.RemovePossibleValue(value))
                    {
                        updatedNodes.Add(node);
                    }
                }
            }

            // check the box
            for (int boxLine = (line / boxHeight) * boxHeight; boxLine < ((line / boxHeight) + 1) * boxHeight; boxLine++)
            {
                for (int boxCol = (col / boxWidth) * boxWidth; boxCol < ((col / boxWidth) + 1) * boxWidth; boxCol++)
                {
                    if (boxLine != line && boxCol != col)
                    {
                        node = nodes[boxLine, boxCol];
                        if (node.Value == SudokuInternalNode.NO_VALUE && node.RemovePossibleValue(value))
                        {
                            updatedNodes.Add(node);
                        }
                    }
                }
            }
            return updatedNodes;
        }


        /// <summary>
        /// Validate that the solution is correct.
        /// </summary>
        /// <returns></returns>
        internal bool ValidateSolution()
        {
            for (int i = 0; i < possibleNodeValueCount; i++)
            {
                ulong rezLine = ulong.MaxValue;
                ulong rezCol = ulong.MaxValue;

                for (int line = 0; line < possibleNodeValueCount; line++)
                {
                    // we haven't finished
                    if (nodes[line, i].Value < 0)
                        return false;
                    rezLine &= (ulong)~(1 << (nodes[line, i].Value - 1));

                    // we haven't finished
                    if (nodes[i, line].Value < 0)
                        return false;
                    rezCol &= (ulong)~(1 << (nodes[i, line].Value - 1));
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
                            if (nodes[boxLine, boxCol].Value < 0)
                                return false;
                            rezBox &= (ulong)~(1 << (nodes[boxLine, boxCol].Value - 1));
                        }
                    }

                    if (rezBox != winMask)
                        return false;
                }
            }
            hasSolution = true;
            return true;
        }

        #endregion

        /// <summary>
        /// Parse the puzzle and set the values into the grid
        /// </summary>
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

                        if (column >= possibleNodeValueCount)
                        {
                            throw new ArgumentException("There is a mismatch between the size of the grid and the nodes identified in the puzzle");
                        }

                        // If we actually found a value, set it.
                        if (value > -1)
                        {
                            nodes[line, column].Value = value;
                            nodes[line, column].PartOfPuzzle = true;
                        }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSet(ulong possibleValues, int index)
        {
            return (possibleValues & (ulong)(1 << index)) != (ulong)(1 << index);
        }

        /// <summary>
        /// Create a min heap of the not-yet-fixed nodes from the grid
        /// </summary>
        private Heap<SudokuInternalNode> MinHeapFromGrid()
        {
            Heap<SudokuInternalNode> mh = new MinSudokuHeap(possibleNodeValueCount * possibleNodeValueCount);

            for (int i = 0; i < possibleNodeValueCount; i++)
            {
                for (int j = 0; j < possibleNodeValueCount; j++)
                {
                    if (nodes[i, j].Value == SudokuInternalNode.NO_VALUE)
                    {
                        mh.Insert(nodes[i, j]);
                    }

                }
            }

            return mh;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < possibleNodeValueCount; i++)
            {
                for (int j = 0; j < possibleNodeValueCount; j++)
                {
                    sb.Append(GridHelpers.ValueToChar(nodes[i, j].Value));
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
