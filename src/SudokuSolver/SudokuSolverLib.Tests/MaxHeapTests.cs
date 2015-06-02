// Copyright (c) Alex Ghiondea. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SudokuSolverLib.Helpers;
using System;
using Xunit;

namespace SudokuSolverLib.Tests
{
    public class MaxHeapTests
    {
        [Fact]
        public void MaxHeap_TestInsert()
        {
            MaxHeap<int> mh = new MaxHeap<int>();
            Assert.True(mh.IsEmpty);
            for (int i = 0; i < 10; i++)
            {
                mh.Insert(i);
                Assert.False(mh.IsEmpty);
            }

            for (int i = 9; i >= 0; i--)
            {
                int peakValue = mh.PeakAtRoot();

                int value = mh.GetRoot();
                Assert.Equal(peakValue, value);
                Assert.Equal(i, value);
            }

            Assert.True(mh.IsEmpty);
        }

        [Fact]
        public void MaxHeap_TestPopFromEmpty()
        {
            MaxHeap<int> mh = new MaxHeap<int>();

            Assert.True(mh.IsEmpty);
            Assert.Throws<InvalidOperationException>(() => mh.GetRoot());
        }

        [Fact]
        public void MaxHeap_TestPopUntilEmpty()
        {
            MaxHeap<int> mh = new MaxHeap<int>(1);
            mh.Insert(1);

            Assert.Equal(1, mh.GetRoot());
            Assert.True(mh.IsEmpty);
            Assert.Throws<InvalidOperationException>(() => mh.GetRoot());
        }

        [Fact]
        public void MaxHeap_TestPushPopWithEmptyHeap()
        {
            MaxHeap<int> mh = new MaxHeap<int>();
            mh.Insert(1);
            Assert.Equal(1, mh.GetRoot());
            Assert.True(mh.IsEmpty);

            mh.Insert(2);
            Assert.Equal(2, mh.GetRoot());
            Assert.True(mh.IsEmpty);

            Assert.Throws<InvalidOperationException>(() => mh.GetRoot());
        }

        [Fact]
        public void MaxHeap_NegativeInitialStorage_Negative()
        {
            Assert.Throws<ArgumentException>(() => new MaxHeap<int>(-1));
        }

        [Fact]
        public void MaxHeap_ForceResize()
        {
            MaxHeap<int> heap = new MaxHeap<int>(1);
            heap.Insert(2);
            heap.Insert(1);

            Assert.Equal(2, heap.GetRoot());
            Assert.Equal(1, heap.GetRoot());
        }

    }
}
