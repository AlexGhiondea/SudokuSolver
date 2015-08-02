using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverLib.Utils
{
    public static class ResourceProvider
    {
        public static string GetString(string resourceID)
        {
            return stringResources.ResourceManager.GetString(resourceID);
        }
    }
}
