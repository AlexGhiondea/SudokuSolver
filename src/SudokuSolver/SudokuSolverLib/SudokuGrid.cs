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
        private List<SudokuNode> Nodes;
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

            List<SudokuNode> nodes = ParsePazzleToNodes(puzzle, boxWidth, boxHeight).ToList();

            if (nodes.Count != (boxWidth * boxHeight) * (boxWidth * boxHeight))
                throw new ArgumentException("There is a mismatch between the number of nodes specified and the nodes that were passed in");

            return new SudokuGrid(boxWidth, boxHeight, nodes);
        }

        public bool SolveGrid()
        {
            Heap<SudokuNode> mh = new Heap<SudokuNode>(
                (left, right) => left.PossibleValuesCount < right.PossibleValuesCount,
                boxHeight * boxWidth * boxHeight * boxWidth
                );

            foreach (var node in Nodes)
            {
                if (!node.HasValue)
                {
                    mh.Insert(node);
                }
            }

            return solveGrid(mh);
        }

        public List<SudokuPuzzleNode> GetSolution()
        {
            if (!hasSolution)
            {
                return null;
            }

            return Nodes.ConvertAll(x => x.Node);
        }

        private SudokuGrid(int width, int height, IEnumerable<SudokuNode> nodes)
        {
            this.boxHeight = height;
            this.boxWidth = width;
            this.winMask = ulong.MaxValue << (width * height);
            this.Nodes = nodes.ToList();
        }

        private static IEnumerable<SudokuNode> ParsePazzleToNodes(string puzzle, int width, int height)
        {
            var nodes = CreateGridNodes(puzzle, width, height);

            ComputeNeighbours(width, height, nodes);

            return nodes;
        }

        private static List<SudokuNode> CreateGridNodes(string puzzle, int width, int height)
        {
            List<SudokuNode> l = new List<SudokuNode>();
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

                        column++;
                        textCol++;

                        l.Add(node);
                    }
                    line++;
                }
            }
            return l;
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

        private static void ComputeNeighbours(int width, int height, IEnumerable<SudokuNode> allNodes)
        {
            //For each node, we must give it its neighbours
            foreach (var node in allNodes)
            {
                //identify neighbours
                var box_Line = (node.Node.Line / height) * height;
                var box_Column = (node.Node.Column / width) * width;

                #region Useful when debugging
                //var nei = from n in Nodes
                //          where n != node
                //          where n.Node.Line == node.Node.Line
                //          select n;

                //var nei2 = from n in Nodes
                //           where n != node
                //           where n.Node.Column == node.Node.Column
                //           select n;

                //var nei3 = from n in Nodes
                //           where n != node
                //           where (n.Node.Column >= box_Column && n.Node.Column < box_Column + boxWidth && n.Node.Line >= box_Line && n.Node.Line < box_Line + boxHeight)
                //           select n;
                #endregion

                var neighbours = from n in allNodes
                                 where n != node //not the node itself
                                 where n.Node.Line == node.Node.Line ||
                                       n.Node.Column == node.Node.Column ||
                                       ((n.Node.Column >= box_Column &&
                                         n.Node.Column < box_Column + width &&
                                         n.Node.Line >= box_Line &&
                                         n.Node.Line < box_Line + height)) // on the same line or column or same box
                                 select n;

                node.SetNeighbours(neighbours);

                if (node.HasValue)
                {
                    node.RemoveValueFromNeighbours(node.Node.Value);
                }
            }
        }

        private bool ValidateSolution()
        {
            for (int i = 0; i < boxWidth * boxHeight; i++)
            {
                var line = from n in Nodes
                           where n.Node.Line == i
                           select n;

                var column = from n in Nodes
                             where n.Node.Column == i
                             select n;

                var lineAndColumnOk = ValidateRelatedNodes(line) && ValidateRelatedNodes(column);
                if (lineAndColumnOk == false)
                    return false;
            }

            //validate boxes
            for (int i = 0; i < boxWidth; i++)
            {
                for (int j = 0; j < boxHeight; j++)
                {
                    var box = from n in Nodes
                              where n.Node.Line >= i * boxHeight && n.Node.Line < (i * boxHeight) + boxHeight
                              where n.Node.Column >= j * boxWidth && n.Node.Column < (j * boxWidth) + boxWidth
                              select n;

                    var validateBox = ValidateRelatedNodes(box);
                    if (validateBox == false)
                        return false;
                }
            }

            hasSolution = true;

            return true;
        }

        private bool ValidateRelatedNodes(IEnumerable<SudokuNode> lineOrCol)
        {
            ulong rez = ulong.MaxValue;

            foreach (var node in lineOrCol)
            {
                var val = node.Node.Value;
                // we haven't finished
                if (val < 0)
                    return false;

                rez &= (ulong)~(1 << (val - 1));
            }

            // this will equal (int32.MaxValue << <values> )
            return rez == winMask;
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

            foreach (var value in node.PossibleValues())
            {
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
            //generate a string representation for the grid.
            var sortedNodes = (from n in Nodes
                               orderby n.Node.Column
                               orderby n.Node.Line
                               select n).ToList();

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < boxHeight * boxWidth; i++)
            {
                for (int j = 0; j < boxHeight * boxWidth; j++)
                {
                    var node = sortedNodes[i * boxWidth * boxHeight + j];

                    sb.Append(node.HasValue ? node.Node.ValueToChar() : '.');
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
