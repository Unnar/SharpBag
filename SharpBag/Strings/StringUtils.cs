﻿#if DOTNET4
using System.Diagnostics.Contracts;
#endif

using System.Linq;
using System.Text;

namespace SharpBag.Strings
{
    /// <summary>
    /// A static class with string utilities.
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// Concatenate the specified strings.
        /// </summary>
        /// <param name="separator">A separator between the strings.</param>
        /// <param name="strings">The strings to concatenate.</param>
        /// <returns>The new string.</returns>
        public static string Concat(string separator, params string[] strings)
        {
#if DOTNET4
            Contract.Requires(separator != null);
            Contract.Requires(strings != null);
#endif
            StringBuilder sb = new StringBuilder(strings.Sum(s => (s ?? "").Length) + ((strings.Length - 1) * separator.Length));
            bool first = true;

            foreach (string s in strings)
            {
                if (!first) sb.Append(separator);
                else first = false;

                sb.Append(s);
            }

            return sb.ToString();
        }
    }
}