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
            Assert.Throws<ArgumentException>(() => SudokuPuzzle.FromString(string.Empty, 2, 2));
            Assert.Throws<ArgumentException>(() => SudokuPuzzle.FromString(null, 2, 2));
        }

        [Fact]
        public static void InvalidCharactersInPuzzle()
        {
            Assert.Throws<FormatException>(() => SudokuPuzzle.FromString("$", 2, 2));
            Assert.Throws<FormatException>(() => SudokuPuzzle.FromString(".$", 2, 2));
            Assert.Throws<FormatException>(() => SudokuPuzzle.FromString(@"....
&...", 2, 2));
            Assert.Throws<FormatException>(() => SudokuPuzzle.FromString("1 .      . %", 2, 2));
        }

        [Fact]
        public static void InvalidPuzzleSize()
        {
            Assert.Throws<ArgumentException>(() => SudokuPuzzle.FromString("1", 0, 0));
            Assert.Throws<ArgumentException>(() => SudokuPuzzle.FromString("1", -1, 0));
            Assert.Throws<ArgumentException>(() => SudokuPuzzle.FromString("1", 0, -1));
            Assert.Throws<ArgumentException>(() => SudokuPuzzle.FromString("1", 17, 17));
        }

        [Fact]
        public static void ParseInvalidPuzzle()
        {
            Assert.Throws<ArgumentException>(() => SudokuPuzzle.FromString(@"1...
2...", 2, 2));
        }

        [Fact]
        public static void NotEnoughNodesSpecified()
        {
            Assert.Throws<ArgumentException>(() => SudokuPuzzle.FromString("          ", 16, 16));
            Assert.Throws<ArgumentException>(() => SudokuPuzzle.FromString("          ", 2, 2));
            Assert.Throws<ArgumentException>(() => SudokuPuzzle.FromString("123", 2, 2));
            Assert.Throws<ArgumentException>(() => SudokuPuzzle.FromString(@"....
1...
4321
....", 4, 4));

            Assert.Throws<ArgumentException>(() => SudokuPuzzle.FromString(@"....
1...
4321
....", 1, 1));

            Assert.Throws<ArgumentException>(() => SudokuPuzzle.FromString(@"....3
1...3
43213
....3", 2, 2));
        }

        [Fact]
        public static void InvalidCreateParameters()
        {
            Assert.Throws<ArgumentException>(() => SudokuPuzzle.Create(0, 0, 0));
            Assert.Throws<ArgumentException>(() => SudokuPuzzle.Create(2, 2, -1));
            Assert.Throws<ArgumentException>(() => SudokuPuzzle.Create(2, 2, 32));
            Assert.Throws<ArgumentException>(() => SudokuPuzzle.Create(-2, 2, 2));
            Assert.Throws<ArgumentException>(() => SudokuPuzzle.Create(2, -2, 2));

            Assert.Throws<ArgumentException>(() => SudokuPuzzle.Create(16, 16, 200));
        }
    }
}
