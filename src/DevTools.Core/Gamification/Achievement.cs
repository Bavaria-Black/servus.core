using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevTools.Core.Gamification
{
    public class Achievement
    {
        private readonly IEnumerable<AchievementProperty> _properties;

        public string Name { get; }
        public bool IsUnlocked { get; private set; }

        public event EventHandler Unlocked;

        public Achievement(string name, IEnumerable<AchievementProperty> properties)
        {
            Name = name;
            _properties = properties;
            foreach(var prop in properties)
            {
                prop.Activated += Prop_Activated;
            }
        }

        private void Prop_Activated(object sender, EventArgs e)
        {
            if(_properties.All(p => p.IsActive))
            {
                IsUnlocked = true;
                Unlocked?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
