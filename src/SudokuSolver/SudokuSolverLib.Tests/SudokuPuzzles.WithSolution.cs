// Copyright (c) Alex Ghiondea. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SudokuSolverLib;
using Xunit;

namespace SudokuSolverTests
{
    public class Puzzles
    {
        [Fact]
        private static void Solve1x1()
        {
            var puzzle = @"1";
            var solutionString = @"1";

            CheckSolution(puzzle, solutionString, 1, 1);
        }

        [Fact]
        private static void Solve2x2()
        {
            var puzzle = @"..3.
.1..
..4.
.4..
";
            var solutionString = @"4231
3124
1342
2413";

            CheckSolution(puzzle, solutionString, 2, 2);
        }
        [Fact]
        private static void Solve3x2()
        {
            var puzzle = @"..3...
..41..
1...6.
.3...2
..65..
...6..";

            var solution = @"513246
264153
142365
635412
426531
351624";
            CheckSolution(puzzle, solution, 3, 2);
        }
        [Fact]
        private static void Solve2x3()
        {
            var puzzle = @"5..3..
.6..3.
1.4...
...4.3
.4..2.
..5..6
";
            string solution = @"521364
462531
134652
256413
643125
315246";
            CheckSolution(puzzle, solution, 2, 3);
        }
        [Fact]
        private static void Solve3x3_1()
        {
            var puzzle = @". . . . . . . . .
. . . . . 3 . 8 5
. . 1 . 2 . . . .
. . . 5 . 7 . . .
. . 4 . . . 1 . .
. 9 . . . . . . .
5 . . . . . . 7 3
. . 2 . 1 . . . .
. . . . 4 . . . 9
";
            string solution = @"987654321
246173985
351928746
128537694
634892157
795461832
519286473
472319568
863745219";

            CheckSolution(puzzle, solution, 3, 3);
        }
        [Fact]
        private static void Solve3x3_2()
        {
            var puzzle = @"1 2.  3. .  . . 4
3 5.  . . .  1. .
. . 4. . .  . . .
. . 5  4. .  2. .
6. .  . 7.  . . .
. . .  . . 8. 9.
. . 3  1. .  5. .
. . .  . . 9. 7.
. . .  . 6.  . . 8 ";

            string solution = @"126395784
359847162
874621953
985416237
631972845
247538691
763184529
418259376
592763418
";

            CheckSolution(puzzle, solution, 3, 3);
        }
        [Fact]
        private static void Solve3x3_3()
        {
            var puzzle = @". . .  . . .  . 3 9  
. . .  . . 1  . . 5  
. . 3  . 5 .  8 . .  
. . 8  . 9 .  . . 6  
. 7 .  . . 2  . . .  
1 . .  4 . .  . . .  
. . 9  . 8 .  . 5 .  
. 2 .  . . .  6 . .  
4 . .  7 . .  . . . 
";

            string solution = @"751846239
892371465
643259871
238197546
974562318
165438927
319684752
527913684
486725193
";

            CheckSolution(puzzle, solution, 3, 3);
        }
        [Fact]
        private static void Solve3x3_4()
        {
            var puzzle = @". . 3  . . .  . . .  
4 . .  . 8 .  . 3 6  
. . 8  . . .  1 . .  
. 4 .  . 6 .  . 7 3  
. . .  9 . .  . . .  
. . .  . . 2  . . 5  
. . 4  . 7 .  . 6 8  
6 . .  . . .  . . .  
7 . .  6 . .  5 . .
";

            string solution = @"123456789
457189236
968327154
249561873
576938412
831742695
314275968
695814327
782693541
";

            CheckSolution(puzzle, solution, 3, 3);
        }

        [Fact]
        private static void Solve3x3_5()
        {
            var puzzle = @"..6.8.27.
...7...3.
......1.5
5...1..6.
...572...
.3..4...2
1.4......
.9...6...
.78.5.4..
";

            string solution = @"956183274
421795638
783264195
542318967
619572843
837649512
164827359
295436781
378951426
";

            CheckSolution(puzzle, solution, 3, 3);
        }

        [Fact]
        private static void Solve3x4()
        {
            var puzzle = @"..159....c..
.94....6...7
83...1...6..
c...a...4.5.
4..1....8.c.
..a..b9..7.1
.....c..b...
.8.....13b..
...b1.a4.32.
5.b..a.29...
...8c...5.46
.4...3....b.
";

            string solution = @"6A1594B87C32
B94C3256A817
8357B1C926A4
C276A813495B
4B91576A82C3
26A34B95C781
15398C27B46A
78CA26413B95
978B15A4632C
5CB46A329178
3128C97B5A46
A462738C15B9";

            CheckSolution(puzzle, solution, 3, 4);
        }
        [Fact]
        private static void Solve4x4()
        {
            string puzzle = @"..gf...3.4.716..
....c.b.9....gf3
9.e..27...1..5..
8...61f..5...a49
.f.84...7....e.b
..24..8.f.56g...
.3ad.ge..b4.5...
e........g.....c
.1..8c....b.4.a.
d..6..3e.a..b2.f
..3..f1.4.....d.
b....6.........7
6....d9.37....c.
fc17a....9......
.5.b....c.g.9.2.
.d.e3..7.1.5...6
";

            string solution = @"5BGFE9A3D4C71682
4AD1C5B8926E7GF3
96E3G274A81FC5BD
827C61FDB53GEA49
GF684A5173DC2E9B
19247B8CFE56GD3A
C3ADFGE62B495178
E7B5D3291GA8F46C
71F28CDG56B349AE
D4C6973EGA81B25F
AE39BF154C7268DG
B85G264AEF9D3C17
6G4A5D9237EB8FC1
FC17A8GB6924D3E5
358B1E6FCDGA9724
2D9E34C781F5ABG6";

            CheckSolution(puzzle, solution, 4, 4);
        }

        private static void CheckSolution(string puzzle, string solutionString, int boxWidth, int boxHeight)
        {
            SudokuGrid grid = SudokuGrid.FromPuzzle(puzzle, boxWidth, boxHeight);

            Assert.True(grid.SolveGrid());
            Assert.Equal<string>(grid.ToString(), SudokuGrid.FromPuzzle(solutionString, boxWidth, boxHeight).ToString());
        }
    }
}
