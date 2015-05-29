// Copyright (c) Alex Ghiondea. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using SudokuSolverLib;
using Xunit;

namespace SudokuSolverLib.Tests
{
    public class HeapTests
    {
        [Fact]
        public void TestInsert()
        {
            MinHeap<int> mh = new MinHeap<int>();
            Assert.True(mh.IsEmpty);
            for (int i = 10; i > 0; i--)
            {
                mh.Insert(i);
                Assert.False(mh.IsEmpty);
            }

            for (int i = 1; i <= 10; i++)
            {
                int peakValue = mh.PeakAtRoot();

                int value = mh.GetRoot();
                Assert.Equal(peakValue, value);
                Assert.Equal(i, value);
            }

            Assert.True(mh.IsEmpty);
        }

        [Fact]
        public void TestPopFromEmpty()
        {
            MinHeap<int> mh = new MinHeap<int>();

            Assert.True(mh.IsEmpty);
            Assert.Throws<InvalidOperationException>(()=> mh.GetRoot());
        }

        [Fact]
        public void TestPopUntilEmpty()
        {
            MinHeap<int> mh = new MinHeap<int>();
            mh.Insert(1);

            Assert.Equal(1, mh.GetRoot());
            Assert.True(mh.IsEmpty);
            Assert.Throws<InvalidOperationException>(() => mh.GetRoot());
        }

        [Fact]
        public void TestPushPopWithEmptyHeap()
        {
            MinHeap<int> mh = new MinHeap<int>();
            mh.Insert(1);
            Assert.Equal(1, mh.GetRoot());
            Assert.True(mh.IsEmpty);

            mh.Insert(2);
            Assert.Equal(2, mh.GetRoot());
            Assert.True(mh.IsEmpty);

            Assert.Throws<InvalidOperationException>(() => mh.GetRoot());
        }

        [Fact]
        public void NegativeInitialStorage_Negative()
        {
            Assert.Throws<ArgumentException>(() => new MinHeap<int>(-1));
        }

    }
}
