// Copyright (c) Alex Ghiondea. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SudokuSolverLib.Helpers;
using Xunit;

namespace SudokuSolverLib.Tests
{
    public class GridHelpersTests
    {
        [Fact]
        public void TestGridHelpers_PrettyPrint()
        {
            var grid = SudokuPuzzle.FromString(@"..6.8.27.
...7...3.
......1.5
5...1..6.
...572...
.3..4...2
1.4......
.9...6...
.78.5.4..", 3, 3);

            string prettyPrintGrid = @"|-------------|-------------|-------------|
|             |             |             | 
|  .   .   6  |  .   8   .  |  2   7   .  | 
|          -  |      -      |  -   -      | 
|             |             |             | 
|  .   .   .  |  7   .   .  |  .   3   .  | 
|             |  -          |      -      | 
|             |             |             | 
|  .   .   .  |  .   .   .  |  1   .   5  | 
|             |             |  -       -  | 
|-------------|-------------|-------------|
|             |             |             | 
|  5   .   .  |  .   1   .  |  .   6   .  | 
|  -          |      -      |      -      | 
|             |             |             | 
|  .   .   .  |  5   7   2  |  .   .   .  | 
|             |  -   -   -  |             | 
|             |             |             | 
|  .   3   .  |  .   4   .  |  .   .   2  | 
|      -      |      -      |          -  | 
|-------------|-------------|-------------|
|             |             |             | 
|  1   .   4  |  .   .   .  |  .   .   .  | 
|  -       -  |             |             | 
|             |             |             | 
|  .   9   .  |  .   .   6  |  .   .   .  | 
|      -      |          -  |             | 
|             |             |             | 
|  .   7   8  |  .   5   .  |  4   .   .  | 
|      -   -  |      -      |  -          | 
|-------------|-------------|-------------|
";

            string gridString = grid.PrettyPrint();

            Assert.Equal(prettyPrintGrid, gridString);
        }
    }
}
