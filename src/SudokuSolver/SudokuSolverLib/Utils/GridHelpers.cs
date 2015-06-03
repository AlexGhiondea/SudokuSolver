// Copyright (c) Alex Ghiondea. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Text;

namespace SudokuSolverLib.Helpers
{
    public static class GridHelpers
    {
        public static char ValueToChar(int Value)
        {
            if (Value < 0)
                return '.';

            if (Value < 10)
            {
                return (char)('0' + Value);
            }
            return (char)('A' + (Value - 10));
        }

        public static string PrettyPrint(this SudokuPuzzle grid)
        {
            //generate a string representation for the grid.
            var sortedNodes = (from n in grid.GetNodes()
                               orderby n.Column
                               orderby n.Line
                               select n).ToList();

            StringBuilder line1;
            StringBuilder line2;
            StringBuilder line3;

            StringBuilder sb = new StringBuilder();

            StringBuilder s = new StringBuilder();
            for (int i = 0; i < grid.BoxHeight; i++)
            {
                s.Append("|-");
                s.Append("-".PadRight(4 * grid.BoxWidth, '-'));
            }
            s.Append("|");
            var horizLine = s.ToString();


            sb.AppendLine(horizLine);
            for (int i = 0; i < grid.BoxHeight * grid.BoxWidth; i++)
            {
                line1 = new StringBuilder();
                line2 = new StringBuilder();
                line3 = new StringBuilder();
                line1.Append("| ");
                line2.Append("| ");
                line3.Append("| ");
                for (int j = 0; j < grid.BoxHeight * grid.BoxWidth; j++)
                {
                    var node = sortedNodes[i * grid.BoxWidth * grid.BoxHeight + j];

                    line1.Append("   ");
                    line2.AppendFormat(" {0} ", ValueToChar(node.Value));
                    line3.AppendFormat(" {0} ", node.PartOfPuzzle ? "-" : " ");

                    line1.Append(" ");
                    line2.Append(" ");
                    line3.Append(" ");

                    if ((j + 1) % grid.BoxWidth == 0)
                    {
                        line1.Append("| ");
                        line2.Append("| ");
                        line3.Append("| ");
                    }

                }
                sb.AppendLine(line1.ToString());
                sb.AppendLine(line2.ToString());
                sb.AppendLine(line3.ToString());
                if ((i + 1) % grid.BoxHeight == 0)
                {
                    sb.AppendLine(horizLine);
                }
            }
            return sb.ToString();

        }
    }
}
