namespace Servus.Core.Diagnostics;

[Obsolete("Will not be supported after migration from Servus.Core to Servus lib.")]
public class ActivitySourceNameAttribute(string name) : Attribute
{
    public string ActivitySourceName { get; } = name;
}