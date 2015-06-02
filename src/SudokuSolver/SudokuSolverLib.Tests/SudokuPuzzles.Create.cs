// Copyright (c) Alex Ghiondea. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace SudokuSolverLib.Tests
{
    public class SudokuPuzzles
    {
        [Fact]
        public void CreatePuzzle_NoHints()
        {
            var grid = SudokuPuzzle.Create(2, 2, 0);

            Assert.Equal(true, grid.SolveGrid());
            CheckHintsCount(grid, 0);
        }

        [Fact]
        public void CreatePuzzle_1x1_1hint()
        {
            var grid = SudokuPuzzle.Create(1, 1, 1);

            Assert.Equal(true, grid.SolveGrid());
            CheckHintsCount(grid, 1);
        }

        [Fact]
        public void CreatePuzzle_1x1_0hint()
        {
            var grid = SudokuPuzzle.Create(1, 1, 0);

            Assert.Equal(true, grid.SolveGrid());
            CheckHintsCount(grid, 0);
        }

        [Fact]
        public void CreatePuzzle_SomeHints()
        {
            var grid = SudokuPuzzle.Create(2, 2, 5);

            Assert.Equal(true, grid.SolveGrid());
            CheckHintsCount(grid, 5);
        }

        [Fact]
        public void CreatePuzzle_NonSquare()
        {
            var grid = SudokuPuzzle.Create(1, 3, 0);

            Assert.Equal(true, grid.SolveGrid());
            CheckHintsCount(grid, 0);
        }

        private void CheckHintsCount(SudokuPuzzle grid, int requestedCount)
        {
            int count = 0;
            foreach (var node in grid.GetNodes())
            {
                if (node.PartOfPuzzle)
                {
                    count++;
                }
            }
            Assert.Equal(count, requestedCount);
        }
    }
}
