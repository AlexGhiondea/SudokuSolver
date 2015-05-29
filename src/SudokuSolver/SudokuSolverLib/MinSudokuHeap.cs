// Copyright (c) Alex Ghiondea. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SudokuSolverLib.Helpers
{
    internal class MinSudokuHeap : Heap<SudokuInternalNode>
    {
        public MinSudokuHeap(int storageSize)
            : base(storageSize)
        {

        }
        protected override bool Sorter(SudokuInternalNode left, SudokuInternalNode right)
        {
            return left.PossibleValuesCount < right.PossibleValuesCount;
        }
    }
}
