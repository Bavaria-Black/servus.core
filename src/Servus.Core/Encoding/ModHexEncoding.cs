namespace Servus.Core.Encoding;

public class ModHexEncoding : System.Text.Encoding
{
    private static readonly char[] Alphabet =
    {
        //0   1    2    3    4    5    6    7    8    9    a    b    c    d    e    f
        'c', 'b', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'n', 'r', 't', 'u', 'v'
    };

    private static readonly Lazy<ModHexEncoding> LazyEncoding = new Lazy<ModHexEncoding>(() => new ModHexEncoding());
    public static ModHexEncoding ModHex => LazyEncoding.Value;

    public static string ConvertFromAscii(string ascii)
    {
        var bytes = ASCII.GetBytes(ascii);
        return ModHex.GetString(bytes);
    }

    public static string ConvertToAscii(string modHex)
    {
        var bytes = ModHex.GetBytes(modHex);
        return ASCII.GetString(bytes);
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

            bytes[byteIndex + counter++] = (byte)Alphabet[i1];
            bytes[byteIndex + counter++] = (byte)Alphabet[i2];
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

        for (int i = byteIndex; i < maxCount; i += 2)
        {
            var index1 = Array.IndexOf(Alphabet, (char)bytes[i]);
            var index2 = Array.IndexOf(Alphabet, (char)bytes[i + 1]);

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