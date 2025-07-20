namespace Servus.Core.Application;

public static class ServusApplication
{
    public static string? GetEnvironmentVariable(string name)
    {
        return Environment.GetEnvironmentVariable("SERVUS_" + name.ToUpper());
    } 

    public static bool IsEnvironmentVariableSetTo(string name, string value)
    {
        var environmentVariable = GetEnvironmentVariable(name);
        return environmentVariable != null && environmentVariable.Equals(value, StringComparison.OrdinalIgnoreCase);
    }
}