// Copyright (c) Alex Ghiondea. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SudokuSolverLib.Helpers;
using System;

namespace SudokuSolverLib.Utils
{
    internal struct ValuesProvider
    {
        private readonly int _size;
        private readonly int[] _orderedValues;
        private readonly Random _randomizer;

        public ValuesProvider(int size, Random randomizer)
        {
            _randomizer = randomizer;
            _size = size;

            _orderedValues = ArrayHelpers.Range(_size);
        }

        public int[] GetValues()
        {
            if (_randomizer == null)
                return _orderedValues;

            // we need to randomize some stuff...
            return ArrayHelpers.RandomizedRange(_size, _randomizer);
        }
    }
}
