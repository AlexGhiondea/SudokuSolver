// Copyright (c) Alex Ghiondea. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SudokuSolverLib.Helpers;
using System;
using Xunit;

namespace SudokuSolverLib.Tests
{
    public class MinHeapTests
    {
        [Fact]
        public void MinHeap_TestInsert()
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
        public void MinHeap_TestPopFromEmpty()
        {
            MinHeap<int> mh = new MinHeap<int>();

            Assert.True(mh.IsEmpty);
            Assert.Throws<InvalidOperationException>(()=> mh.GetRoot());
        }

        [Fact]
        public void MinHeap_TestPopUntilEmpty()
        {
            MinHeap<int> mh = new MinHeap<int>(1);
            mh.Insert(1);

            Assert.Equal(1, mh.GetRoot());
            Assert.True(mh.IsEmpty);
            Assert.Throws<InvalidOperationException>(() => mh.GetRoot());
        }

        [Fact]
        public void MinHeap_TestPushPopWithEmptyHeap()
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
        public void MinHeap_NegativeInitialStorage_Negative()
        {
            Assert.Throws<ArgumentException>(() => new MinHeap<int>(-1));
        }

        [Fact]
        public void MinHeap_ForceResize()
        {
            MinHeap<int> heap = new MinHeap<int>(1);
            heap.Insert(1);
            heap.Insert(2);

            Assert.Equal(1, heap.GetRoot());
            Assert.Equal(2, heap.GetRoot());
        }
    }
}
