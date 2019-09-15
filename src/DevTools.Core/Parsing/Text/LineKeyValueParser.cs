using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DevTools.Core.Parsing.Text
{
    public class LineKeyValueParser
    {
        private readonly string _splitRegex;

        public LineKeyValueParser(char splitChar)
        {
            _splitRegex = splitChar.ToString();
        }

        public LineKeyValueParser(string splitRegex)
        {
            _splitRegex = splitRegex;
        }

        public IEnumerable<KeyValuePair<string, string>> Parse(string value)
        {
            string[] lines = Regex.Split(value, "\r\n");
            foreach (string line in lines)
            {
                yield return ParseLine(line);
            }
        }

        public KeyValuePair<string, string> ParseLine(string value)
        {
            if (value.Contains('\n') || value.Contains('\r'))
            {
                throw new ArgumentException("value must not contain linebreaks");
            }

            var result = Regex.Split(value, _splitRegex);
            var key = result[0];
            return new KeyValuePair<string, string>(key, value.Substring(key.Length + 1));
        }
    }
}
