namespace Servus.Core.Diagnostics;

[Obsolete("Will not be supported after migration from Servus.Core to Servus lib.")]
public class ActivitySourceKeyAttribute(Type sourceKey) : Attribute
{
    public Type SourceKey { get; } = sourceKey;
}