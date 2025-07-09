using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Servus.Core.Tests;

public static partial class StringExtensions
{
    [GeneratedRegex(@"(?<=[a-z0-9])(?=[A-Z])" +     // camelCase → camel_Case
                    @"|(?<=[A-Z])(?=[A-Z][a-z])" +         // IOName → IO_Name
                    @"|(?<=[a-z])(?=\d)" +                 // NameV1 → Name_V1
                    @"|(?<=\d)(?=[a-zA-Z])")]              // 1Name -> 1_name
    private static partial Regex SnakeCaseRegex();
    
    public static string ToSnakeCase(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var parts = value.Replace(" ", "_").Split('_')
            .Where(p => !string.IsNullOrEmpty(p))
            .Select(part => SnakeCaseRegex().Replace(part, "_").ToLower());

        var result = string.Join("_", parts);
        
        return char.IsDigit(result[0]) ? "_" + result : result;
    }
}
