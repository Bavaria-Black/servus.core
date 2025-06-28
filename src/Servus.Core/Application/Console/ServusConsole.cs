using System;
using System.Collections.Generic;
using static System.Console;
namespace Servus.Core.Application.Console;

public class ServusConsole
{
    public static void WriteColored(string value, ConsoleColor color)
    {
        var tmp = ForegroundColor;
        ForegroundColor = color;
        Write(value);
        ForegroundColor = tmp;
    }

    public static void WriteLineColored(string value, ConsoleColor color)
    {
        WriteColored(value + Environment.NewLine, color);
    }

    public static void PrintLine(int length = 80, char lineChar = '=')
    {
        WriteLine(new string(lineChar, length));
    }

    public static void PrintKeyValue<TKey, TValue>(
        KeyValuePair<TKey, TValue> keyValuePair,
        int width = 14,
        ConsoleColor keyColor = ConsoleColor.DarkCyan,
        int indent = 2)
    {
        var key = keyValuePair.Key as string ?? (keyValuePair.Key?.ToString() ?? string.Empty);
        var value = keyValuePair.Value as string ?? (keyValuePair.Value?.ToString() ?? string.Empty);
        
        PrintKeyValue(key, value, width, keyColor, indent);
    }

    public static void PrintKeyValue(
        KeyValuePair<string, string> keyValuePair,
        int width = 14,
        ConsoleColor keyColor = ConsoleColor.DarkCyan,
        int indent = 2)
    {
        PrintKeyValue(keyValuePair.Key, keyValuePair.Value, width, keyColor, indent);
    }

    public static void PrintKeyValue(
        string key,
        string value,
        int width = 14,
        ConsoleColor keyColor = ConsoleColor.DarkCyan,
        int indent = 2)
    {
        Write("[".PadLeft(indent));
        WriteColored(key, keyColor);
        Write("]".PadRight(width - key.Length));
        WriteLine(" => " + value);
    }
}