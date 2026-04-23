namespace Servus.Core.Diagnostics;

/// <summary>
/// Identifies a named trace channel. Use the predefined static fields or create custom
/// categories with <c>new ServusTraceCategory("MyCategory")</c>.
/// </summary>
public readonly struct ServusTraceCategory : IEquatable<ServusTraceCategory>
{
    /// <summary>The unique name that identifies this category.</summary>
    public string Name { get; }

    /// <summary>Creates a custom trace category with the given name.</summary>
    public ServusTraceCategory(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        Name = name;
    }

    /// <inheritdoc />
    public bool Equals(ServusTraceCategory other) =>
        string.Equals(Name, other.Name, StringComparison.Ordinal);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is ServusTraceCategory other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => Name?.GetHashCode(StringComparison.Ordinal) ?? 0;

    /// <inheritdoc />
    public override string ToString() => Name ?? string.Empty;

    public static bool operator ==(ServusTraceCategory left, ServusTraceCategory right) => left.Equals(right);

    public static bool operator !=(ServusTraceCategory left, ServusTraceCategory right) => !left.Equals(right);

    public static implicit operator string(ServusTraceCategory category) => category.Name;
    public static explicit operator ServusTraceCategory(string name) => new(name);
}