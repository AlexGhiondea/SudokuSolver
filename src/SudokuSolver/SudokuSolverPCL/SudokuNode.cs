// Copyright (c) Alex Ghiondea. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SudokuSolverLib
{
    /// <summary>
    /// This represents a solution node 
    /// </summary>
    public struct SudokuNode
    {
        public readonly int Line;
        public readonly int Column;
        public readonly int Value;
        public readonly bool PartOfPuzzle;

        public SudokuNode(int line, int column, int value, bool partOfPuzzle) 
        {
            Line = line;
            Column = column;
            Value = value;
            PartOfPuzzle = partOfPuzzle;
        }
    }
}
