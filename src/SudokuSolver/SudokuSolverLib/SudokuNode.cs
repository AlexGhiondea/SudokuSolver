// Copyright (c) Alex Ghiondea. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolverLib
{
    internal class SudokuNode
    {
        public SudokuPuzzleNode Node { get; set; }

        public bool HasValue; // { get; set; }

        public SudokuNode[] neighbours;

        public int PossibleValuesCount;// { get; set; }

        public ulong PossibleValues = ulong.MaxValue;

        private int MaxNodeValues;

        public void SetValue(int value)
        {
            Node.Value = value;
            HasValue = true;
            Node.PartOfPuzzle = true;
        }

        public SudokuNode(int line, int column, int possibleValuesCount)
        {
            Node = new SudokuPuzzleNode(line, column);
            MaxNodeValues = possibleValuesCount;

            PossibleValuesCount = MaxNodeValues;
        }

        public SudokuNode(int line, int column, int maxPossibleValues, int value)
            : this(line, column, maxPossibleValues)
        {
            Node.Value = value;
            Node.PartOfPuzzle = true;
            HasValue = true;
        }

        public void AddPossibleValue(int value)
        {
            if (!IsSet(value - 1))
                return;

            PossibleValuesCount++;
            Set(value - 1);
        }

        public bool RemovePossibleValue(int value)
        {
            //if we already don't have that value, nothing to do
            if (IsSet(value - 1))
                return false;

            PossibleValuesCount--;
            Clear(value - 1);
            return true;
        }

        public void SetNeighbours(IEnumerable<SudokuNode> nodes)
        {
            neighbours = nodes.ToArray();
        }

        public List<SudokuNode> RemoveValueFromNeighbours(int value)
        {
            var changed = new List<SudokuNode>();
            for (int i = 0; i < neighbours.Length; i++)
            {
                if (!neighbours[i].HasValue)
                {
                    // if the node removed the value it means it was part of its list of potential values.
                    if (neighbours[i].RemovePossibleValue(value))
                        changed.Add(neighbours[i]);
                }
            }
            return changed;
        }

        public override string ToString()
        {
            if (!HasValue)
            {
                string values = string.Empty;
                for (int i = 0; i < MaxNodeValues; i++)
                {
                    if (!IsSet(i))
                    {
                        values += i;
                    }
                }

                return string.Format("{0} - ({1},{2}) --> values:{3}", PossibleValuesCount, Node.Line, Node.Column, values);
            }
            else
            {
                return string.Format("({0},{1}) --> value:{2}", Node.Line, Node.Column, Node.Value);
            }
        }

        #region Bitwise helpers
        private void Clear(int index)
        {
            PossibleValues &= (ulong)~(1 << index);
        }

        private void Set(int index)
        {
            PossibleValues |= (ulong)(1 << index);
        }

        private bool IsSet(int index)
        {
            return (PossibleValues & (ulong)(1 << index)) != (ulong)(1 << index);
        }
        #endregion
    }
}
