using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Core.Encoding
{
    public class ModHexEncoding : System.Text.Encoding
    {
        private static readonly char[] _alphabet =
        {
            //0   1    2    3    4    5    6    7    8    9    a    b    c    d    e    f
            'c', 'b', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'n', 'r', 't', 'u', 'v'
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
            return ASCII.GetString(bytes);
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

        public override int GetByteCount(char[] chars, int index, int count)
        {
            return count * 2;
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            var maxCount = charCount + charIndex;
            int counter = 0;

            for (int i = charIndex; i < maxCount; i++)
            {
                var i1 = (chars[i] >> 4) & 0xf;
                var i2 = chars[i] & 0xf;

                bytes[byteIndex + counter++] = (byte)_alphabet[i1];
                bytes[byteIndex + counter++] = (byte)_alphabet[i2];
            }

            return counter;
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return count / 2;
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            var maxCount = byteCount + byteIndex;
            int counter = 0;

            for (int i = byteIndex; i < maxCount; i = i + 2)
            {
                var index1 = Array.IndexOf(_alphabet, (char)bytes[i]);
                var index2 = Array.IndexOf(_alphabet, (char)bytes[i + 1]);

                chars[charIndex + counter++] = (char)(index1 << 4 | index2);
            }

            return counter;
        }

        public override int GetMaxByteCount(int charCount)
        {
            return charCount * 2;
        }

        public override int GetMaxCharCount(int byteCount)
        {
            return byteCount / 2;
        }
    }
}
