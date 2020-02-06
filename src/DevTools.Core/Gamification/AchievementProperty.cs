using System;

namespace DevTools.Core.Gamification
{
    public class AchievementProperty
    {
        public string Name { get; }

        private double _value;
        public double Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnValueChanged();
                }
            }
        }

        public double TargetValue { get; }
        public CompareRule CompareRule { get; }

        public bool AutoReset { get; set; }

        public bool IsActive { get; private set; }

        public event EventHandler Activated;

        public AchievementProperty(string name, double initialValue, double targetValue, CompareRule compareRule, bool autoReset)
        {
            Name = name;
            CompareRule = compareRule;
            TargetValue = targetValue;
            AutoReset = autoReset;
            Value = initialValue;
        }

        private void OnValueChanged()
        {
            if (!IsActive && Compare())
            {
                IsActive = true;
                Activated?.Invoke(this, EventArgs.Empty);
            }
            else if (AutoReset && IsActive)
            {
                IsActive = false;
            }
        }

        private bool Compare()
        {
            switch (CompareRule)
            {
                case CompareRule.Equals:
                    return Value == TargetValue;
                case CompareRule.GreaterThen:
                    return Value > TargetValue;
                case CompareRule.LowerThen:
                    return Value < TargetValue;
                case CompareRule.GreaterOrEquals:
                    return Value >= TargetValue;
                case CompareRule.LowerOrEquals:
                    return Value <= TargetValue;
            }

            return false;
        }
    }
}
