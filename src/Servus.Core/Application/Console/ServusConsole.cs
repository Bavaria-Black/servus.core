using static System.Console;
namespace Servus.Core.Application.Console;

/// <summary>
/// Provides enhanced console output methods with color support and formatted display utilities.
/// </summary>
public class ServusConsole
{
    /// <summary>
    /// Writes text to the console in the specified color, then restores the original foreground color.
    /// </summary>
    /// <param name="value">The text to write to the console.</param>
    /// <param name="color">The foreground color to use for the text.</param>
    public static void WriteColored(string value, ConsoleColor color)
    {
        var tmp = ForegroundColor;
        ForegroundColor = color;
        Write(value);
        ForegroundColor = tmp;
    }

    /// <summary>
    /// Writes a line of text to the console in the specified color, then restores the original foreground color.
    /// </summary>
    /// <param name="value">The text to write to the console.</param>
    /// <param name="color">The foreground color to use for the text.</param>
    public static void WriteLineColored(string value, ConsoleColor color)
    {
        WriteColored(value + Environment.NewLine, color);
    }

    /// <summary>
    /// Prints a horizontal line of repeated characters to the console.
    /// </summary>
    /// <param name="length">The length of the line in characters. Default is 80.</param>
    /// <param name="lineChar">The character to repeat for the line. Default is '='.</param>
    public static void PrintLine(int length = 80, char lineChar = '=')
    {
        WriteLine(new string(lineChar, length));
    }

    /// <summary>
    /// Prints a key-value pair in a formatted manner with colored key display.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="keyValuePair">The key-value pair to display.</param>
    /// <param name="width">The minimum width for the key field. Default is 14.</param>
    /// <param name="keyColor">The color to use for the key text. Default is DarkCyan.</param>
    /// <param name="indent">The number of spaces to indent the output. Default is 2.</param>
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

    /// <summary>
    /// Prints a string key-value pair in a formatted manner with colored key display.
    /// </summary>
    /// <param name="keyValuePair">The string key-value pair to display.</param>
    /// <param name="width">The minimum width for the key field. Default is 14.</param>
    /// <param name="keyColor">The color to use for the key text. Default is DarkCyan.</param>
    /// <param name="indent">The number of spaces to indent the output. Default is 2.</param>
    public static void PrintKeyValue(
        KeyValuePair<string, string> keyValuePair,
        int width = 14,
        ConsoleColor keyColor = ConsoleColor.DarkCyan,
        int indent = 2)
    {
        PrintKeyValue(keyValuePair.Key, keyValuePair.Value, width, keyColor, indent);
    }

    /// <summary>
    /// Prints a key and value in a formatted manner with colored key display.
    /// </summary>
    /// <param name="key">The key text to display.</param>
    /// <param name="value">The value text to display.</param>
    /// <param name="width">The minimum width for the key field. Default is 14.</param>
    /// <param name="keyColor">The color to use for the key text. Default is DarkCyan.</param>
    /// <param name="indent">The number of spaces to indent the output. Default is 2.</param>
    /// <remarks>
    /// Output format: [key] => value, where the key is displayed in the specified color and properly aligned.
    /// </remarks>
    // ReSharper disable once MemberCanBePrivate.Global
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