using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ascon.Pilot.Web.Extensions
{
    public static class StringExtensions
    {
        public static bool IsContainWhiteSpace(this string value)
        {
            if (value == null)
                return false;

            return value.Any(t => Char.IsWhiteSpace(t));
        }

        public static string LimitCharacters(this string text, int length)
        {
            if (String.IsNullOrEmpty(text))
            {
                return String.Empty;
            }

            // If text in shorter or equal to length, just return it
            if (text.Length <= length)
            {
                return text;
            }

            // Text is longer, so try to find out where to cut
            char[] delimiters = { ' ', '.', ',', ':', ';' };
            int index = text.LastIndexOfAny(delimiters, length - 3);

            if (index > (length / 2))
            {
                return text.Substring(0, index) + "\u2026";
            }
            return text.Substring(0, length - 3) + "\u2026";
        }

        public static string RemoveInvalidFileNameCharacters(this string str)
        {
            return ReplaceInvalidFileNameCharacters(str, String.Empty);
        }

        public static string ReplaceInvalidFileNameCharacters(this string str)
        {
            return ReplaceInvalidFileNameCharacters(str, "_");
        }

        private static string ReplaceInvalidFileNameCharacters(this string str, string strToReplace)
        {
            var invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var result = str;
            foreach (var c in invalid)
            {
                result = result.Replace(c.ToString(), strToReplace);
            }
            return result;
        }

        public static string GetXamlName(this string value)
        {
            return new String(value.Where(c => Char.IsLetterOrDigit(c)).ToArray());
        }

        public static string TrimQuotes(this string value)
        {
            return value.Replace("\"", "");
        }

        public static IEnumerable<string> Split(this string str, int chunkSize)
        {
            var lastWhitespace = 0;
            for (var i = 0; i < str.Length; i += chunkSize)
            {
                if (lastWhitespace != -1)
                    i = lastWhitespace;

                var substring = str.Substring(i, Math.Min(chunkSize, str.Length - i));

                var indexWs = substring.LastIndexOf(" ", StringComparison.Ordinal);
                if (indexWs == -1 || substring.Length < chunkSize)
                {
                    lastWhitespace = indexWs;
                    yield return substring;
                    continue;
                }

                lastWhitespace = lastWhitespace + indexWs;
                var substr = str.Substring(i, Math.Min(indexWs, str.Length - i));

                yield return substr;
            }
        }
    }
}