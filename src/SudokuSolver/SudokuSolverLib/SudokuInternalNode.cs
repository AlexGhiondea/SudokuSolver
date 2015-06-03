// Copyright (c) Alex Ghiondea. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolverLib
{
    internal class SudokuInternalNode
    {
        public readonly int Line;
        public readonly int Column;
        public int Value;
        public bool PartOfPuzzle;

        public bool HasValue;

        public int PossibleValuesCount;

        public ulong PossibleValues = ulong.MaxValue;

        private int MaxNodeValues;

        public void SetValue(int value)
        {
            Value = value;
            HasValue = true;
            PartOfPuzzle = true;
        }

        public SudokuInternalNode(int line, int column, int possibleValuesCount)
        {
            Line = line;
            Column = column;
            MaxNodeValues = possibleValuesCount;

            PossibleValuesCount = MaxNodeValues;
        }

        public void AddPossibleValue(int value)
        {
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

        //public override string ToString()
        //{
        //    if (!HasValue)
        //    {
        //        string values = string.Empty;
        //        for (int i = 0; i < MaxNodeValues; i++)
        //        {
        //            if (!IsSet(i))
        //            {
        //                values += i;
        //            }
        //        }

        //        return string.Format("{0} - ({1},{2}) --> values:{3}", PossibleValuesCount, Line, Column, values);
        //    }
        //    else
        //    {
        //        return string.Format("({0},{1}) --> value:{2}", Line, Column, Value);
        //    }
        //}

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
