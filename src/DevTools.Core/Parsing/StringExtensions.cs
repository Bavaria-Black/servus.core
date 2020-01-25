using System.Collections.Generic;
using System.IO;

namespace DevTools.Core.Parsing
{
    public static class StringExtensions
    {
        /// <summary>
        /// Reads lines of characters from the provided string and
        /// returns them as IEnumerable for a line by line iteration.
        /// </summary>
        /// <param name="value">String that should be split into lines.</param>
        /// <returns>IEnumerable of lines</returns>
        public static IEnumerable<string> GetLines(this string value)
        {
            if (value == null)
            {
                yield break;
            }

            using (var stringReader = new StringReader(value))
            {
                string line;
                while ((line = stringReader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}