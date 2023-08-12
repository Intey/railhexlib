using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RailHexLib
{
    internal static class Utils
    {

        internal static List<Tuple<T, T>> MakePairs<T>(List<T> list)
        {
            Debug.Assert(list.ToList().Count > 1);
            List<Tuple<T, T>> result = new();
            for (int j = 0; j < list.Count() - 1; j++)
            {
                for (int i = j + 1; i < list.Count(); i++)
                {
                    result.Add(new Tuple<T, T>(list[j], list[i]));
                }
            }
            return result;
        }
    }
}
