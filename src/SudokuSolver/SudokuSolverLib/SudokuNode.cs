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

        //public IEnumerable<int> PossibleValues()
        //{
        //    for (int i = 0; i < MaxNodeValues; i++)
        //    {
        //        if (!IsSet(i))
        //            yield return i + 1;
        //    }
        //}

        public void Print(StringBuilder line1, StringBuilder line2, StringBuilder line3)
        {
            StringBuilder current = line1;

            if (HasValue)
            {
                line1.Append("   ");
                line3.Append("   ");
                current = line2;

                current.AppendFormat(" {0} ", Node.ValueToChar());
            }
            else
            {
                string l1 = "", l2 = "", l3 = "";

                int pos = 0;
                for (int i = 0; i < MaxNodeValues; i++)
                {
                    if (!IsSet(i))
                    {
                        int item = i + 1;
                        pos++;
                        if (pos <= 3)
                            l1 += item.ToString();
                        else if (pos > 6)
                            l3 += item.ToString();
                        else if (pos > 3)
                            l2 += item.ToString();
                    }
                }

                l1 = l1.PadRight(3, ' ');
                l2 = l2.PadRight(3, ' ');
                l3 = l3.PadRight(3, ' ');

                line1.Append(l1);
                line2.Append(l2);
                line3.Append(l3);
            }
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
