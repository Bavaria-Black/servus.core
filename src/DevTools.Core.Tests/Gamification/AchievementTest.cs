using DevTools.Core.Gamification;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Core.Tests.Gamification
{
    [TestClass]
    public class AchievementTest
    {
        [TestMethod]
        public void BasicAchievementTest()
        {
            var achievements = new AchievementCollection();
            achievements.AddProperty("points", 0, 500, CompareRule.GreaterOrEquals);
            achievements.AddProperty("retries", 0, 0, CompareRule.Equals, true);

            achievements.AddAchievement("leberkas", "points");
            achievements.AddAchievement("leberkasSupreme", "points", "retries");

            bool leberkasUnlocked = false;
            bool leberkasSupremeUnlocked = false;
            achievements.AchievementUnlocked += (s, e) =>
            {
                if (e == "leberkas")
                {
                    leberkasUnlocked = true;
                }
                else if (e == "leberkasSupreme")
                {
                    leberkasSupremeUnlocked = true;
                }
            };

            Assert.IsFalse(leberkasUnlocked);
            Assert.IsFalse(leberkasSupremeUnlocked);

            achievements.SetValue("retries", 1);
            achievements.SetValue("points", 400);
            
            Assert.IsFalse(leberkasUnlocked);
            Assert.IsFalse(leberkasSupremeUnlocked);

            achievements.SetValue("points", 500);

            Assert.IsTrue(leberkasUnlocked);
            Assert.IsFalse(leberkasSupremeUnlocked);

            achievements.SetValue("retries", 0);

            Assert.IsTrue(leberkasUnlocked);
            Assert.IsTrue(leberkasSupremeUnlocked);

            leberkasUnlocked = false;
            leberkasSupremeUnlocked = false;

            achievements.SetValue("points", 300);
            Assert.IsFalse(leberkasUnlocked);
            Assert.IsFalse(leberkasSupremeUnlocked);

            achievements.SetValue("points", 3000);
            Assert.IsFalse(leberkasUnlocked);
            Assert.IsFalse(leberkasSupremeUnlocked);
        }
    }
}
