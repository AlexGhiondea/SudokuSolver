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
        public const int NO_VALUE = -1;
        public readonly int Line;
        public readonly int Column;
        public int Value;
        public bool PartOfPuzzle;
        public int PossibleValuesCount;
        public ulong PossibleValues = ulong.MaxValue;

        public SudokuInternalNode(int line, int column, int possibleValuesCount)
        {
            Line = line;
            Column = column;
            Value = NO_VALUE;
            PossibleValuesCount = possibleValuesCount;
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
