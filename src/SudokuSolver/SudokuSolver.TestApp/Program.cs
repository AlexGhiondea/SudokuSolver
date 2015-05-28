// Copyright (c) Alex Ghiondea. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SudokuSolverLib;
using System.Diagnostics;

namespace SudokuSolver.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {

            Solve3x3();
            //long max = -1;
            //string puzzle = string.Empty;
            //Stopwatch sw = new Stopwatch();


            //for (int i = 0; i < 10000; i++)
            //{
            //    sw.Restart();
            //    Console.CursorTop = 0;
            //    Console.CursorLeft = 0;
            //    var grid = SudokuPuzzle.Create(3, 3, 21);

            //    Console.WriteLine(grid.PrettyPrint());
            //    grid.SolveGrid();
            //    Console.WriteLine(grid.PrettyPrint());
            //    sw.Stop();

            //    if (max < sw.ElapsedTicks)
            //    {
            //        max = sw.ElapsedTicks;
            //        puzzle = puzzle.ToString();
            //    }
            //}
            ////Console.WriteLine(grid.ToString());

            //Console.WriteLine(max);
            //Console.WriteLine(puzzle);

            //return;
            ////if (grid.SolveGrid())
            ////{
            ////    Console.WriteLine(grid.ToString());
            ////    //Console.WriteLine(PrettyPrintSolution(grid.GetSolution().ToList(), 3, 3));
            ////}

            //return;

            ////Solve3x3();
            ////Stopwatch sw = new Stopwatch();
            //sw.Start();
            ////for (int i = 0; i < 100; i++)
            //{
            //    Solve3x3();
            //}
            //sw.Stop();

            //Console.WriteLine(sw.Elapsed);
            //Console.WriteLine(sw.ElapsedMilliseconds);
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
