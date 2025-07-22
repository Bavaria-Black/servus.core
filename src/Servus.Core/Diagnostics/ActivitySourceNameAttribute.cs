namespace Servus.Core.Diagnostics;

public class ActivitySourceNameAttribute(string name) : Attribute
{
    public string ActivitySourceName { get; } = name;
}