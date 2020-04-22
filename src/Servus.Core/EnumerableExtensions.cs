using System;
using System.Collections.Generic;
using System.Linq;

namespace Servus.Core
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Selects items from an IEnumerable which are distinct by the given property / properties.
        /// </summary>
        /// <typeparam name="T">The element type of the IEnumerable</typeparam>
        /// <typeparam name="TKey">The type of the key</typeparam>
        /// <param name="items">The IEnumerable to select distinct items from</param>
        /// <param name="property">The property / properties of T which will be taken into account when checking for distinctness</param>
        /// <returns>An IEnumerable which is distinct by the given property / properties</returns>
        /// <example>
        /// This sample shows how to call the <see cref="DistinctBy{T, TKey}" /> method.
        /// <code>
        /// var list = new List{Point}();
        /// var distinct = list.DistinctBy(p => p.X);
        /// </code>
        /// You can also select multiple properties of the item type T like this:
        /// <code>
        /// var list = new List{Point}();
        /// var distinct = list.DistinctBy(p => new { p.X, p.Y });
        /// </code>
        /// </example>
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
        {
            return items.GroupBy(property).Select(x => x.First());
        }
    }
}