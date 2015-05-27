// Copyright (c) Alex Ghiondea. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SudokuSolverLib;
using System;
using Xunit;

namespace SudokuSolverLibTests
{
    public class InputValidationTests
    {
        [Fact]
        public static void EmptyOrNullPuzzle()
        {
            Assert.Throws<ArgumentException>(() => SudokuGrid.FromPuzzle(string.Empty, 2, 2));
            Assert.Throws<ArgumentException>(() => SudokuGrid.FromPuzzle(null, 2, 2));
        }

        [Fact]
        public static void InvalidCharactersInPuzzle()
        {
            Assert.Throws<FormatException>(() => SudokuGrid.FromPuzzle("$", 2, 2));
            Assert.Throws<FormatException>(() => SudokuGrid.FromPuzzle(".$", 2, 2));
            Assert.Throws<FormatException>(() => SudokuGrid.FromPuzzle(@"....
&...", 2, 2));
            Assert.Throws<FormatException>(() => SudokuGrid.FromPuzzle("1 .      . %", 2, 2));
        }

        [Fact]
        public static void InvalidPuzzleSize()
        {
            Assert.Throws<ArgumentException>(() => SudokuGrid.FromPuzzle("1", 0, 0));
            Assert.Throws<ArgumentException>(() => SudokuGrid.FromPuzzle("1", -1, 0));
            Assert.Throws<ArgumentException>(() => SudokuGrid.FromPuzzle("1", 0, -1));
            Assert.Throws<ArgumentException>(() => SudokuGrid.FromPuzzle("1", 17, 17));
        }

        [Fact]
        public static void NotEnoughNodesSpecified()
        {
            Assert.Throws<ArgumentException>(() => SudokuGrid.FromPuzzle("          ", 16, 16));
            Assert.Throws<ArgumentException>(() => SudokuGrid.FromPuzzle("          ", 2, 2));
            Assert.Throws<ArgumentException>(() => SudokuGrid.FromPuzzle("123", 2, 2));
            Assert.Throws<ArgumentException>(() => SudokuGrid.FromPuzzle(@"....
1...
4321
....", 4, 4));

            Assert.Throws<ArgumentException>(() => SudokuGrid.FromPuzzle(@"....
1...
4321
....", 1, 1));

            Assert.Throws<ArgumentException>(() => SudokuGrid.FromPuzzle(@"....3
1...3
43213
....3", 2, 2));
        }
    }
}
