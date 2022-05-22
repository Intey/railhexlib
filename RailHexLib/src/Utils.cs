using System.Collections.Generic;
using System.Linq;

namespace RailHexLib
{
    internal static class Utils
    {

        internal static List<List<T>> MakePairs<T>(List<T> list)
        {
            List<List<T>> result = new List<List<T>>();
            for (int j = 0; j < list.Count() - 1; j++)
            {
                for (int i = j + 1; i < list.Count(); i++)
                {
                    result.Add(new List<T>() { list[j], list[i] });
                }
            }
            return result;
        }
    }
}
