using System;
using System.Collections.Generic;
using System.Linq;
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
            return value.GetLines().Select(ParseLine);
        }

        public KeyValuePair<string, string> ParseLine(string value)
        {
            if (value.Contains('\n') || value.Contains('\r'))
            {
                throw new ArgumentException("value shall not contain linebreaks.");
            }

            var result = Regex.Split(value, _splitRegex);
            var key = result[0];
            return new KeyValuePair<string, string>(key, value.Substring(key.Length + 1));
        }
    }
}
