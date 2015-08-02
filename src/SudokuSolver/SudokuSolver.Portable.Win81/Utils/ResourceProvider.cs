using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace SudokuSolverLib.Utils
{
    public static class ResourceProvider
    {
        private static ResourceLoader resourceLoader = new ResourceLoader();
        public static string GetString(string resourceID)
        {
            return resourceLoader.GetString(resourceID);
        }
    }
}
