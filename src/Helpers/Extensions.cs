using System.Collections.Generic;
using System.Linq;

namespace RoyalQuiz.Helpers
{
    public static class Extensions
    {
        /// <summary>
        ///     Shuffles the list.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            var cnt = list.Count;

            while (cnt > 1)
            {
                cnt--;
                var k = Program.Random.Next(cnt + 1);
                var value = list[k];
                list[k] = list[cnt];
                list[cnt] = value;
            }
        }

        /// <summary>
        ///     Shuffles the dictionary.
        /// </summary>
        public static Dictionary<TKey, TValue> Shuffle<TKey, TValue>(
            this Dictionary<TKey, TValue> source)
        {
            return source.OrderBy(x => Program.Random.Next())
                .ToDictionary(item => item.Key, item => item.Value);
        }
    }
}