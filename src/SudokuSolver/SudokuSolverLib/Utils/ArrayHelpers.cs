// Copyright (c) Alex Ghiondea. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace SudokuSolverLib
{
    internal static class ArrayHelpers
    {
        public static int[] CreateOrderedArray(int arrayLength)
        {
            int[] arr = new int[arrayLength];
            for (int i = 0; i < arrayLength; i++)
            {
                arr[i] = i;
            }

            return arr;
        }

        public static int[] CreateArrayOfRandomValues(Random r, int arrayLength)
        {
            int[] arr = CreateOrderedArray(arrayLength);

            for (int i = 0; i < arrayLength; i++)
            {
                var val = r.Next(0, arrayLength);

                int temp = arr[i];
                arr[i] = arr[val];
                arr[val] = temp;
            }
            return arr;
        }

    }
}
