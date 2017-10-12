using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsLibrary.Trees
{
    public static class OperationExtensions
    {
        /// <summary>
        /// Returns the higher item or the first one.
        /// </summary>
        /// <typeparam name="T">Comparable value</typeparam>
        /// <param name="item1">First item to compare</param>
        /// <param name="item2">Second item to compare</param>
        /// <returns></returns>
        public static T Max<T>(T item1, T item2) where T : IComparable<T>
        {
            return item1.CompareTo(item2) < 0 ? item2 : item1;
        }

        /// <summary>
        /// Adds two generic values.
        /// </summary>
        /// <typeparam name="T">Value type to add should be able to perform + operation</typeparam>
        /// <param name="in1">Value to add</param>
        /// <param name="in2">Value to add</param>
        /// <returns></returns>
        public static T Add<T>(T in1, T in2)
        {
            var d1 = Convert.ToDouble(in1);
            var d2 = Convert.ToDouble(in2);
            return (T)(dynamic)(d1 + d2);
        }

        public static T Sub<T>(T item, T about)
        {
            var d1 = Convert.ToDouble(item);
            var d2 = Convert.ToDouble(about);
            return (T)(dynamic)(d1 - d2);
        }
    }
}
