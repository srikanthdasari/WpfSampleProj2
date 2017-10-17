using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfSampleProj2.Lib.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Is sequence empty or null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <returns>True if empty or null, else false</returns>
        public static bool IsEnumEmpty<T>(this IEnumerable<T> sequence)
        {
            return sequence.IsNull() || !sequence.Any();
        }


        /// <summary>
        /// Is sequence not empty and not null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <returns>False if empty or null, else true</returns>
        public static bool IsEnumNotEmpty<T>(this IEnumerable<T> sequence)
        {
            return sequence.IsNotNull() && sequence.Any();
        }


        /// <summary>
        /// Call Action<sequence>() if sequence is not null && not empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="action"></param>
        /// <returns>Always return same sequence</returns>
        public static IEnumerable<T> IfEnumNotEmpty<T>(this IEnumerable<T> sequence, Action<IEnumerable<T>> action)
        {
            if (sequence.IsEnumNotEmpty())
                action(sequence);

            return sequence.IsNull() ? new T[] { } : sequence;
        }


        /// <summary>
        /// Calls Func< sequence<T>,  sequence<TRet> >() when T seq is not null AND not empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TRet"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="action"></param>
        /// <returns>Sequence of TRet</returns>
        public static IEnumerable<TRet> IfEnumNotEmpty<T, TRet>(this IEnumerable<T> sequence, Func<IEnumerable<T>, IEnumerable<TRet>> action)
        {
            return sequence.IsEnumNotEmpty() ? action(sequence) : new TRet[] { };
        }


        /// <summary>
        /// Call Action<sequence>() if sequence is null OR empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="action"></param>
        /// <returns>Always return same sequence. If null, returns converts to empty.</returns>
        public static IEnumerable<T> IfEnumEmpty<T>(this IEnumerable<T> sequence, Action action)
        {
            if (sequence.IsEnumEmpty())
                action();

            return sequence.IsNull() ? new T[] { } : sequence;
        }


        /// <summary>
        /// Calls Func< sequence<T>,  sequence<TRet> >() when T seq is null OR empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TRet"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="action"></param>
        /// <returns>Sequence of TRet</returns>
        public static IEnumerable<TRet> IfEnumEmpty<T, TRet>(this IEnumerable<T> sequence, Func<IEnumerable<TRet>> action)
        {
            IEnumerable<TRet> rtn = null;

            if (sequence.IsEnumEmpty())
                rtn = action();

            return rtn ?? new TRet[] { };
        }

        /// <summary>
        /// Iterate through enumerable collection
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="sequence">original sequence</param>
        /// <param name="action">Action method to call for each item in the sequence</param>
        /// <returns>Always the same input sequence. If input sequence is null, returns empty IEnumerable<TItem> collection.</returns>
        public static IEnumerable<TItem> ForEach<TItem>(this IEnumerable<TItem> sequence, System.Action<TItem> action)
        {
            var enumerable = sequence ?? new TItem[] { };

            foreach (var item in enumerable)
                action(item);

            return enumerable;
        }

        ///// <summary>
        ///// Do (similar to Rx extension). Perfrome single action in Action< IEnumerable<TItem> >
        ///// </summary>
        ///// <typeparam name="TItem"></typeparam>
        ///// <param name="sequence">original sequence</param>
        ///// <param name="action">Action to perfrom in collection</param>
        ///// <returns>Always the same input sequence.</returns>
        //public static IEnumerable<TItem> Do<TItem>(this IEnumerable<TItem> sequence, Action<IEnumerable<TItem>> action)
        //{
        //    action(sequence);

        //    return sequence;
        //}
    }
}
