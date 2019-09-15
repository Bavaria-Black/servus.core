using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Core.Encoding
{
    public class ModHex
    {
        private static readonly char[] _alphabet =
        {
            //0   1    2   3   4   5   6   7   8   9  a    b   c   d   e   f
            'c', 'b', 'd','e','f','g','h','i','j','k','l','n','r','t','u','v'
        };

        public static string EncodeFromAscii(string ascii)
        {
            var bytes = System.Text.Encoding.ASCII.GetBytes(ascii);
            return Encode(bytes);
        }

        public static string Encode(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                var i1 = (bytes[i] >> 4) & 0xf;
                var i2 = bytes[i] & 0xf;

                builder.Append(_alphabet[i1]);
                builder.Append(_alphabet[i2]);
            }

            return builder.ToString();
        }

        public static string DecodeToAscii(string modhex)
        {
            var bytes = Decode(modhex);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        public static byte[] Decode(string modhex)
        {
            if (modhex.Length % 2 != 0)
            {
                throw new ArgumentException("modhex contains an invalid encoding");
            }

            byte[] value = new byte[modhex.Length / 2];
            for (int i = 0; i < modhex.Length; i = i + 2)
            {
                var index1 = Array.IndexOf(_alphabet, modhex[i]);
                var index2 = Array.IndexOf(_alphabet, modhex[i + 1]);
                value[i / 2] = (byte)(index1 << 4 | index2);
            }

            return value;
        }
    }
}
