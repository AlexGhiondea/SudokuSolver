// Copyright (c) Alex Ghiondea. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SudokuSolverLib;
using SudokuSolverLib.Helpers;
using System.Diagnostics;

namespace SudokuSolver.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Solve3x3();
        }

        private static void Solve3x3()
        {
            string puzzle = @".6...2..9
.....917.
8.9..3..4
....2....
.....8.4.
5.......2
..2......
..52.....
..8.9.4..
";

            SudokuPuzzle grid = SudokuPuzzle.FromString(puzzle, 3, 3);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (grid.SolveGrid())
            {
                Console.WriteLine("Solved");
                Console.WriteLine(grid.PrettyPrint());
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}
