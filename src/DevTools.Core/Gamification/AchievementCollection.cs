using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevTools.Core.Gamification
{
    public class AchievementCollection
    {
        private List<Achievement> _achievements = new List<Achievement>();
        public IEnumerable<Achievement> Achievements => _achievements;

        private readonly List<AchievementProperty> _properties = new List<AchievementProperty>();

        public event EventHandler<string> AchievementUnlocked;

        public void AddProperty(string name, double initialValue, double targetValue, CompareRule compareRule, bool autoReset = false)
        {
            _properties.Add(new AchievementProperty(name, initialValue, targetValue, compareRule, autoReset));
        }

        public void AddAchievement(string name, params string[] propertyName)
        {
            var props = _properties.Where(p => propertyName.Contains(p.Name));
            var achievement = new Achievement(name, props);
            achievement.Unlocked += Achievement_Unlocked;
            _achievements.Add(achievement);
        }

        private void Achievement_Unlocked(object sender, EventArgs e)
        {
            var achievement = (Achievement)sender;
            _achievements.Remove(achievement);
            AchievementUnlocked?.Invoke(this, achievement.Name);
        }

        public void SetValue(string propertyName, double value)
        {
            var prop = _properties.First(p => p.Name == propertyName);
            prop.SetValue(value);
        }
    }
}
