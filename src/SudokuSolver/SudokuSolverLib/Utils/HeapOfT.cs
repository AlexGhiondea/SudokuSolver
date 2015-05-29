// Copyright (c) Alex Ghiondea. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace SudokuSolverLib
{
    public abstract class Heap<T>
    {
        private const int DEFAULT_STORAGE_SIZE = 32;

        protected abstract bool Sorter(T first, T second);

        private T[] _storage;
        private int _size = 0;

        public Heap()
            : this(DEFAULT_STORAGE_SIZE)
        {
        }

        public Heap(int storageSize)
        {
            if (storageSize < 0)
                throw new ArgumentException("Initial storage size must be a positive number");

            _storage = new T[storageSize];
        }

        public void Insert(T node)
        {
            // If we are running out of space double the size of the storage
            if (_size + 1 > _storage.Length)
            {
                var tempArray = new T[_storage.Length * 2];
                _storage.CopyTo(tempArray, 0);
                _storage = null;
                _storage = tempArray;
            }

            // place the value at the end of the array (cheap to do)
            _storage[_size++] = node;

            int index = _size - 1;
            var parent = (index - 1) / 2;

            // Move the element up until it's parent is smaller than it
            while (index > 0 && !Sorter(_storage[parent], _storage[index]))
            {
                Swap(index, parent);
                index = parent;
                parent = (index - 1) / 2;
            }

        }

        public T GetRoot()
        {
            if (_size == 0)
                throw new InvalidOperationException("No elements in the heap");

            T min = _storage[0];
            _storage[0] = _storage[--_size];

            Heapify(0);
            if (_size < 0)
                _size = 0;

            return min;
        }

        public T PeakAtRoot() { return _storage[0]; }

        public bool IsEmpty { get { return _size == 0; } }

        public void Resort()
        {
            Sort(0);
        }

        public void PrintHeap(int pos = 0, string indent = "")
        {
            if (pos >= _size || _storage[pos] == null)
                return;

            PrintHeap(pos * 2 + 1, indent + " ");

            Console.WriteLine(indent + _storage[pos].ToString());

            PrintHeap(pos * 2 + 2, indent + " ");
        }

        private void Swap(int index, int smallest)
        {
            var temp = _storage[index];
            _storage[index] = _storage[smallest];
            _storage[smallest] = temp;
        }

        private void Heapify(int index)
        {
            //This assumes that the trees under left/right are min-heaps
            int left = index * 2 + 1;
            int right = index * 2 + 2;
            int smallest = index;

            if (left < _size && Sorter(_storage[left], _storage[index]))
                smallest = left;
            if (right < _size && Sorter(_storage[right], _storage[smallest]))
                smallest = right;

            if (smallest != index)
            {
                Swap(index, smallest);
                Heapify(smallest);
            }
        }

        private void Sort(int index)
        {
            //This assumes that the trees under left/right are min-heaps
            int left = index * 2 + 1;
            int right = index * 2 + 2;
            int smallest = index;

            if (left < _size && Sorter(_storage[left], _storage[index]))
                smallest = left;
            if (right < _size && Sorter(_storage[right], _storage[smallest]))
                smallest = right;

            if (smallest != index)
            {
                Swap(index, smallest);
            }

            if (left < _size)
                Sort(left);

            if (right < _size)
                Sort(right);
        }
    }

    public class MinHeap<T> : Heap<T> where T : IComparable<T>
    {
        public MinHeap(int initialStorageSize)
            : base(initialStorageSize)
        {

        }

        public MinHeap()
            : base()
        {

        }
        protected override bool Sorter(T first, T second)
        {
            return first.CompareTo(second) < 0;
        }
    }

    public class MaxHeap<T> : Heap<T> where T : IComparable<T>
    {
        protected override bool Sorter(T first, T second)
        {
            return first.CompareTo(second) > 0;
        }
    }
}
