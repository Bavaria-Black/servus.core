using System.Text.RegularExpressions;

namespace Servus.Core.Text;

public static partial class StringExtensions
{
    [GeneratedRegex(@"(?<=[a-z0-9])(?=[A-Z])" +     // camelCase → camel_Case
                    @"|(?<=[A-Z])(?=[A-Z][a-z])" +         // IOName → IO_Name
                    @"|(?<=[a-z])(?=\d)" +                 // NameV1 → Name_V1
                    @"|(?<=\d)(?=[a-zA-Z])")]              // 1Name -> 1_name
    private static partial Regex SnakeCaseRegex();

    public static string ToSnakeCase(this string value) => ToAnyCase(value, "_");
    public static string ToDotCase(this string value) => ToAnyCase(value, ".");
    public static string ToKebabCase(this string value) => ToAnyCase(value, "-");
    
    private static string ToAnyCase(string value, string delimiter)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var parts = value.Replace(" ", delimiter).Split(delimiter)
            .Where(p => !string.IsNullOrEmpty(p))
            .Select(part => SnakeCaseRegex().Replace(part, delimiter).ToLower());

        var result = string.Join(delimiter, parts);
        
        return char.IsDigit(result[0]) ? delimiter + result : result;
    }
}
