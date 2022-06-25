using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace HokmGame.Util
{
    public static class Extensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list)
        {
            var r = new Random((int)DateTime.Now.Ticks);
            var shuffledList = list.Select(x => new { Number = r.Next(), Item = x }).OrderBy(x => x.Number).Select(x => x.Item);
            return shuffledList.ToList();
        }

        public static void ReplaceKey<T, U>(this Dictionary<T, U> source, T key, T newKey)
        {
            if (!source.TryGetValue(key, out var value))
                throw new ArgumentException("Key does not exist", nameof(key));
            source.Remove(key);
            source.Add(newKey, value);
        }
    }
}