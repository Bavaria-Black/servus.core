# Achievements

An achievement is a named bundle of **properties** that all have to be "active" for the achievement to unlock. A property tracks a numeric value against a target with a comparison rule (`Equals`, `GreaterThen`, etc.). Change property values as the user plays; when all properties of an achievement are active, you get an `AchievementUnlocked` event.

## Quick start

```csharp
using Servus.Core.Gamification;

var achievements = new AchievementCollection();

// 1) Declare properties
achievements.AddProperty("kills",         initialValue: 0, targetValue: 100, CompareRule.GreaterOrEquals);
achievements.AddProperty("no-damage",     initialValue: 1, targetValue: 1,   CompareRule.Equals);
achievements.AddProperty("seconds-alive", initialValue: 0, targetValue: 60,  CompareRule.GreaterOrEquals);

// 2) Group them into achievements
achievements.AddAchievement("First Blood",  "kills");
achievements.AddAchievement("Flawless Run", "kills", "no-damage");
achievements.AddAchievement("Survivor",     "seconds-alive");

// 3) Subscribe to unlocks
achievements.AchievementUnlocked += (_, name) => Console.WriteLine($"🏆 {name}!");

// 4) Feed gameplay values
achievements.SetValue("kills", 100);          // triggers "First Blood"
achievements.SetValue("seconds-alive", 75);   // triggers "Survivor"
// "Flawless Run" still pending: kills is fine, no-damage still 1 (OK)
// Any damage will set "no-damage" to 0 — achievement stays locked
```

## `CompareRule`

```csharp
public enum CompareRule
{
    Equals,             // value == targetValue
    GreaterThen,        // value >  targetValue
    LowerThen,          // value <  targetValue
    GreaterOrEquals,    // value >= targetValue
    LowerOrEquals       // value <= targetValue
}
```

## `AchievementProperty`

One trackable metric. Becomes **active** when the current value satisfies the rule against the target.

```csharp
public class AchievementProperty
{
    public string Name { get; }
    public double Value { get; set; }
    public double TargetValue { get; }
    public CompareRule CompareRule { get; }
    public bool AutoReset { get; set; }
    public bool IsActive { get; }

    public event EventHandler? Activated;

    public AchievementProperty(
        string name,
        double initialValue,
        double targetValue,
        CompareRule compareRule,
        bool autoReset);
}
```

`AutoReset = true` flips the property back to inactive as soon as its rule stops holding — useful for "hold this state for X" mechanics.

## `Achievement`

A named bundle of properties. `IsUnlocked` is `true` iff every property is active.

```csharp
public class Achievement
{
    public string Name { get; }
    public bool IsUnlocked { get; }

    public event EventHandler? Unlocked;

    public Achievement(string name, IEnumerable<AchievementProperty> properties);
}
```

## `AchievementCollection`

The orchestrator — you'll usually only touch this type.

```csharp
public class AchievementCollection
{
    public IEnumerable<Achievement> Achievements { get; }

    public event EventHandler<string>? AchievementUnlocked;

    public void AddProperty(
        string name, double initialValue, double targetValue,
        CompareRule compareRule, bool autoReset = false);

    public void AddAchievement(string name, params string[] propertyName);

    public void SetValue(string propertyName, double value);
}
```
