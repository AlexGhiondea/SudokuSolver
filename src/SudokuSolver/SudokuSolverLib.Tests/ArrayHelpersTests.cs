// Copyright (c) Alex Ghiondea. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SudokuSolverLib.Helpers;
using System;
using Xunit;

namespace SudokuSolverLib.Tests
{
    public class ArrayHelpersTests
    {
#if !PUBLISH
        [Fact]
        public void TestArrayRange()
        {
            int[] range = ArrayHelpers.Range(10);
            Assert.Equal(10, range.Length);

            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(i, range[i]);
            }
        }

        [Fact]
        public void TestArrayRange_ZeroElements()
        {
            int[] range = ArrayHelpers.Range(0);
            Assert.Equal(range.Length, 0);
        }

        [Fact]
        public void TestArrayRandomizedRange()
        {
            Random r = new Random(123);
            int[] range = ArrayHelpers.RandomizedRange(10, r);
            int[] expected = new int[] {6,3,7, 8,2,9,1,0,4,5};

            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(range[i], expected[i]);
            }
        }
#endif
    }
}
