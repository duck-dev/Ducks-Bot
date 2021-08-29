using System;

namespace DucksBot
{
    /// <summary>
    /// All extension methods
    /// </summary>
    public static class Extensions
    {
        #region string
        /// <summary>
        /// Get the part from a specified index to the first occurrence of a specified string inside a string
        /// </summary>
        /// <param name="text">The entire string</param>
        /// <param name="until">The occurrence that we're searching for</param>
        /// <param name="from">0 by default to start from the very beginning, but it may be useful to set this number if
            /// you don't want it to start at the beginning of 'text' (especially used by 'GetFromUntil' method).</param>
        /// <returns>The selected part of the string</returns>
        public static string GetUntilOrEmpty(this string text, string until, int from = 0)
        {
            if (string.IsNullOrWhiteSpace(text)) 
                return string.Empty;
            
            int location = text.Length - from;
            if (until != null)
                location = text.IndexOf(until, StringComparison.Ordinal) - from - until.Length + 1;
            
            if (location > 0)
                return text.Substring(from, location);

            return string.Empty;
        }

        /// <summary>
        /// Get the part from the first occurrence of a specified string until the first occurrence of another
        /// specified string. If 'until' is not set, it will just go to the end of the entire string.
        /// </summary>
        /// <param name="text">The entire string</param>
        /// <param name="from">First occurrence that we're searching for</param>
        /// <param name="until">'null' by default, meaning that it will go from 'from' to the end of the entire string.</param>
        /// <returns>The selected part of the string</returns>
        public static string GetFromUntil(this string text, string from, string until = null)
        {
            int location = text.IndexOf(from, StringComparison.Ordinal) + from.Length;
            return GetUntilOrEmpty(text, until, location);
        }
        
        /// <summary>
        /// Get the part from the first occurrence of a specified string until the first occurrence of another
        /// specified string. If 'until' is not set, it will just go to the end of the entire string.
        /// </summary>
        /// <param name="text">The entire string</param>
        /// <param name="from">Index of the first occurrence that we're searching for</param>
        /// <param name="until">'null' by default, meaning that it will go from 'from' to the end of the entire string.</param>
        /// <returns>The selected part of the string</returns>
        public static string GetFromUntil(this string text, int from, string until = null)
        {
            return GetUntilOrEmpty(text, until, from);
        }

        /// <summary>
        /// Get the part from the first occurrence of a specified string until the specified index.
        /// </summary>
        /// <param name="text">The entire string</param>
        /// <param name="from">First occurrence that we're searching for</param>
        /// <param name="until">The index, which specifies the end of the returned string.</param>
        /// <returns>The selected part of the string</returns> 
        public static string GetFromUntil(this string text, string from, int until)
        {
            int location = text.IndexOf(from, StringComparison.Ordinal) + from.Length;
            return text.Substring(location, until - location);
        }
        #endregion
        
        #region TimeSpan
        /// <summary>
        /// Returns a "human-readable" string from a TimeSpan.
        /// </summary>
        /// <returns>The "human-readable" string, representing a TimeSpan</returns> 
        public static string ToHumanTimeString(this TimeSpan span)
        {
            return null;
        }
        #endregion
    }
}