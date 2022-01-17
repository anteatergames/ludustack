using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace LuduStack.Domain.Core.Extensions
{
    public static class ListExtensions
    {
        public static List<T> Safe<T>(this List<T> list)
        {
            return list ?? new List<T>();
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                byte[] box;
                do
                {
                    box = RandomNumberGenerator.GetBytes(1);
                }
                while (!(box[0] < n * (byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static IEnumerable<List<T>> SplitList<T>(this List<T> locations)
        {
            return SplitList(locations, 10);
        }

        public static IEnumerable<List<T>> SplitList<T>(this List<T> locations, int nSize)
        {
            for (int i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }
    }
}