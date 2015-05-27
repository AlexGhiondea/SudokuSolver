// Copyright (c) Alex Ghiondea. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SudokuSolverLib;
using Xunit;

namespace SudokuSolverTests
{
    public class PuzzlesWithNoSolution
    {
        [Fact]
        private static void CouldNotSolve2x2()
        {
            string puzzle= @"..3.
.1..
..2.
.43.
";

            CheckSolution(puzzle, 2, 2);
        }


        [Fact]
        private static void CouldNotSolve3x3()
        {
            string puzzle= @". . . . . . . . .
. . . . . 3 . 9 5
. . 1 . 2 . . 2 .
. . . 5 . 7 . . .
. . 4 . . . 1 . .
. 9 . . . . . . .
5 . . . . . . 7 3
. . 2 . 1 . . . .
. . . . 4 . . . 9
";

            CheckSolution(puzzle, 3, 3);
        }

        private static void CheckSolution(string puzzle, int boxWidth, int boxHeight)
        {
            SudokuGrid grid = SudokuGrid.FromPuzzle(puzzle, boxWidth, boxHeight);

            Assert.False(grid.SolveGrid());
        }
    }
}