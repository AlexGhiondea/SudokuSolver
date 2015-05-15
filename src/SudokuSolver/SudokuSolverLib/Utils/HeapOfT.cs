// Copyright (c) Alex Ghiondea. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace SudokuSolverLib
{
    public class Heap<T>
    {
        private const int DEFAULT_STORAGE_SIZE = 32;

        private Func<T, T, bool> _sortFunc;
        private T[] _storage;
        private int _size = 0;

        public Heap(Func<T, T, bool> sortFunc)
            : this(sortFunc, DEFAULT_STORAGE_SIZE)
        {
            _sortFunc = sortFunc;
        }

        public Heap(Func<T, T, bool> sortFunc, int storageSize)
        {
            _storage = new T[storageSize];
            _sortFunc = sortFunc;
        }

        public Heap(IEnumerable<T> items, Func<T, T, bool> sortFunc, int storageSize)
            : this(sortFunc, storageSize)
        {
            foreach (var item in items)
            {
                Insert(item);
            }
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
            while (index > 0 && !_sortFunc(_storage[parent], _storage[index]))
            {
                Swap(index, parent);
                index = parent;
                parent = (index - 1) / 2;
            }

        }

        public T GetRoot()
        {
            T min = _storage[0];
            _storage[0] = _storage[--_size];

            Heapify(0);

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

            if (left < _size && _sortFunc(_storage[left], _storage[index]))
                smallest = left;
            if (right < _size && _sortFunc(_storage[right], _storage[smallest]))
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

            if (left < _size && _sortFunc(_storage[left], _storage[index]))
                smallest = left;
            if (right < _size && _sortFunc(_storage[right], _storage[smallest]))
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
}
