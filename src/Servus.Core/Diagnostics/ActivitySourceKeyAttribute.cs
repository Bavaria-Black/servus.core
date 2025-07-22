namespace Servus.Core.Diagnostics;

public class ActivitySourceKeyAttribute(Type sourceKey) : Attribute
{
    public Type SourceKey { get; } = sourceKey;
}