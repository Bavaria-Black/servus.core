using System;
using System.Collections.Generic;
using System.Linq;

namespace Servus.Core.Gamification;

public class Achievement
{
    private readonly IEnumerable<AchievementProperty> _properties;

    public string Name { get; }
    public bool IsUnlocked { get; private set; }

    public event EventHandler? Unlocked;

    public Achievement(string name, IEnumerable<AchievementProperty> properties)
    {
        Name = name;
        _properties = properties.ToList();
        foreach(var prop in _properties)
        {
            prop.Activated += Prop_Activated;
        }
    }

    private void Prop_Activated(object? sender, EventArgs e)
    {
        if (!_properties.All(p => p.IsActive)) return;
        
        IsUnlocked = true;
        Unlocked?.Invoke(this, EventArgs.Empty);
    }
}