// Copyright (c) Alex Ghiondea. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SudokuSolverLib
{
    /// <summary>
    /// This represents a solution node 
    /// </summary>
    public class SudokuPuzzleNode
    {
        public int Line { get; set; }
        public int Column { get; set; }
        public int Value { get; set; }
        public bool PartOfPuzzle { get; set; }

        public SudokuPuzzleNode(int line, int column)
        {
            Line = line;
            Column = column;
            PartOfPuzzle = false;
        }

        public char ValueToChar()
        {
            if (Value < 10)
            {
                return (char)('0' + Value);
            }
            else
            {
                return (char)('A' + (Value - 10));
            }
        }
    }
}
