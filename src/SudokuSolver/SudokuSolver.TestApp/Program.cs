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
            var grid = SudokuGrid.CreatePuzzle(2, 2, 7);
            Console.WriteLine(PrettyPrintSolution(grid.GetSolution().ToList(), 2, 2));
            //Console.WriteLine(grid.ToString());

            grid.SolveGrid();
            Console.WriteLine(PrettyPrintSolution(grid.GetSolution().ToList(), 2, 2));

            return;
            if (grid.SolveGrid())
            {
                Console.WriteLine(grid.ToString());
                //Console.WriteLine(PrettyPrintSolution(grid.GetSolution().ToList(), 3, 3));
            }

            return;

            //Solve3x3();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //for (int i = 0; i < 100; i++)
            {
                Solve3x3();
            }
            sw.Stop();

            Console.WriteLine(sw.Elapsed);
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        static string PrettyPrintSolution(List<SudokuPuzzleNode> nodes, int boxWidth, int boxHeight)
        {
            //generate a string representation for the grid.
            var sortedNodes = (from n in nodes
                               orderby n.Column
                               orderby n.Line
                               select n).ToList();

            StringBuilder line1;
            StringBuilder line2;
            StringBuilder line3;

            StringBuilder sb = new StringBuilder();

            StringBuilder s = new StringBuilder();
            for (int i = 0; i < boxHeight; i++)
            {
                s.Append("|-");
                s.Append("-".PadRight(4 * boxWidth, '-'));
            }
            s.Append("|");
            var horizLine = s.ToString();


            sb.AppendLine(horizLine);
            for (int i = 0; i < boxHeight * boxWidth; i++)
            {
                line1 = new StringBuilder();
                line2 = new StringBuilder();
                line3 = new StringBuilder();
                line1.Append("| ");
                line2.Append("| ");
                line3.Append("| ");
                for (int j = 0; j < boxHeight * boxWidth; j++)
                {
                    var node = sortedNodes[i * boxWidth * boxHeight + j];

                    line1.Append("   ");
                    line2.AppendFormat(" {0} ", node.ValueToChar()!='0' ? node.ValueToChar() : '.' );
                    line3.Append("   ");

                    line1.Append(" ");
                    line2.Append(" ");
                    line3.Append(" ");

                    if ((j + 1) % boxWidth == 0)
                    {
                        line1.Append("| ");
                        line2.Append("| ");
                        line3.Append("| ");
                    }

                }
                sb.AppendLine(line1.ToString());
                sb.AppendLine(line2.ToString());
                sb.AppendLine(line3.ToString());
                if ((i + 1) % boxHeight == 0)
                {
                    sb.AppendLine(horizLine);
                }
            }
            return sb.ToString();
        }

        private static void Solve3x3()
        {
            string puzzle = @". . . . . . . . .
. . . . . 3 . 8 5
. . 1 . 2 . . . .
. . . 5 . 7 . . .
. . 4 . . . 1 . .
. 9 . . . . . . .
5 . . . . . . 7 3
. . 2 . 1 . . . .
. . . . 4 . . . 9
";

            SudokuGrid grid = SudokuGrid.CreateGrid(puzzle, 3, 3);

            if (grid.SolveGrid())
            {
                //Console.WriteLine("Solved");
                //Console.WriteLine(PrettyPrintSolution(grid.GetSolution().ToList(), grid.BoxWidth, grid.BoxHeight));
            }
        }
    }
}
