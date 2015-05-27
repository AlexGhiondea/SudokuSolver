// Copyright (c) Alex Ghiondea. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SudokuSolverLib
{
    internal class MinSudokuHeap : Heap<SudokuNode>
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
}
