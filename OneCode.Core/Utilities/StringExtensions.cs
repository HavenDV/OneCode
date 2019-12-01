using System;
using System.Collections.Generic;

#nullable enable

namespace OneCode.Core.Utilities
{
    /// <summary>
    /// Extensions that work with <see langword="string"/>
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Retrieves the string between the starting fragment and the ending.
        /// The first available fragment is retrieved.
        /// Returns <see langword="null"/> if nothing is found.
        /// If <paramref name="end"/> is not specified, the end is the end of the string.
        /// Version: 1.0.0.0
        /// </summary>
        /// <param name="text"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string? Extract(this string text, string start, string? end = null)
        {
            text = text ?? throw new ArgumentNullException(nameof(text));
            start = start ?? throw new ArgumentNullException(nameof(start));

            var index1 = text.IndexOf(start, StringComparison.Ordinal);
            if (index1 < 0)
            {
                return null;
            }

            index1 += start.Length;
            if (end == null)
            {
                return text.Substring(index1);
            }

            var index2 = text.IndexOf(end, index1, StringComparison.Ordinal);
            if (index2 < 0)
            {
                return null;
            }

            return text.Substring(index1, index2 - index1);
        }

        /// <summary>
        /// Retrieves the strings between the starting fragment and the ending.
        /// All available fragments are retrieved.
        /// Returns empty <see cref="List{T}"/> if nothing is found.
        /// Version: 1.0.0.0
        /// </summary>
        /// <param name="text"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static List<string> ExtractAll(this string text, string start, string end)
        {
            text = text ?? throw new ArgumentNullException(nameof(text));
            start = start ?? throw new ArgumentNullException(nameof(start));
            end = end ?? throw new ArgumentNullException(nameof(end));

            var values = new List<string>();

            while (true)
            {
                var index1 = text.IndexOf(start, StringComparison.Ordinal);
                if (index1 < 0)
                {
                    return values;
                }

                var index2 = text.IndexOf(end, index1 + start.Length, StringComparison.Ordinal);
                if (index2 < 0)
                {
                    return values;
                }

                values.Add(text.Substring(index1, index2 - index1));
            }
        }
    }
}
