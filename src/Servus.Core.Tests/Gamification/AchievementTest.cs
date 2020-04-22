using Servus.Core.Gamification;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Servus.Core.Tests.Gamification
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

        [TestMethod]
        public void DoubleValueCalculatedEvalAchievementTest()
        {
            var achievements = new AchievementCollection();
            achievements.AddProperty("retries", 0d, 0.3d, CompareRule.Equals, true);
            achievements.AddAchievement("calculated 0.2+0.1=0.3", "retries");


            bool achievementUnlocked = false;
            achievements.AchievementUnlocked += (s, e) =>
            {
                if (e == "calculated 0.2+0.1=0.3")
                {
                    achievementUnlocked = true;
                }
            };

            // the sum of both double values are not exact, due to the nature of a double
            // CompareRule equals should only be used when integers are used or for booleans
            achievements.SetValue("retries", (0.2d + 0.1d));
            Assert.IsFalse(achievementUnlocked);
        }

        [TestMethod]
        public void IntegerValueCalculatedEvalAchievementTest()
        {
            var achievements = new AchievementCollection();
            achievements.AddProperty("retries", 0d, 3d, CompareRule.Equals, true);
            achievements.AddAchievement("calculated 2+1=3", "retries");


            bool achievementUnlocked = false;
            achievements.AchievementUnlocked += (s, e) =>
            {
                if (e == "calculated 2+1=3")
                {
                    achievementUnlocked = true;
                }
            };

            achievements.SetValue("retries", 2 + 1);
            Assert.IsTrue(achievementUnlocked);
        }

        [TestMethod]
        public void GreaterThenTest()
        {
            var achievements = new AchievementCollection();
            achievements.AddProperty("retries", 0d, 3d, CompareRule.GreaterThen, true);
            achievements.AddAchievement("calculated", "retries");


            bool achievementUnlocked = false;
            achievements.AchievementUnlocked += (s, e) =>
            {
                if (e == "calculated")
                {
                    achievementUnlocked = true;
                }
            };

            achievements.SetValue("retries", 2 + 1);
            Assert.IsFalse(achievementUnlocked);

            achievements.SetValue("retries", 2 + 2);
            Assert.IsTrue(achievementUnlocked);
        }

        [TestMethod]
        public void GreaterOrEqualsTest()
        {
            var achievements = new AchievementCollection();
            achievements.AddProperty("retries", 0d, 3d, CompareRule.GreaterOrEquals, true);
            achievements.AddProperty("retries2", 0d, 3d, CompareRule.GreaterOrEquals, true);
            achievements.AddAchievement("calculated", "retries");
            achievements.AddAchievement("calculated2", "retries");


            bool achievementUnlocked = false;
            bool achievementUnlocked2 = false;
            achievements.AchievementUnlocked += (s, e) =>
            {
                if (e == "calculated")
                {
                    achievementUnlocked = true;
                }
                else if (e == "calculated2")
                {
                    achievementUnlocked2 = true;
                }
            };

            achievements.SetValue("retries", 2 + 1);
            Assert.IsTrue(achievementUnlocked);

            achievements.SetValue("retries2", 2 + 500);
            Assert.IsTrue(achievementUnlocked2);
        }

        [TestMethod]
        public void LowerThenTest()
        {
            var achievements = new AchievementCollection();
            achievements.AddProperty("retries", 500d, 3d, CompareRule.LowerThen, true);
            achievements.AddAchievement("calculated", "retries");


            bool achievementUnlocked = false;
            achievements.AchievementUnlocked += (s, e) =>
            {
                if (e == "calculated")
                {
                    achievementUnlocked = true;
                }
            };

            achievements.SetValue("retries", 3);
            Assert.IsFalse(achievementUnlocked);

            achievements.SetValue("retries", 2);
            Assert.IsTrue(achievementUnlocked);
        }

        [TestMethod]
        public void LowerOrEqualsTest()
        {
            var achievements = new AchievementCollection();
            achievements.AddProperty("retries", 500d, 3d, CompareRule.LowerOrEquals, true);
            achievements.AddProperty("retries2", 500d, 3d, CompareRule.LowerOrEquals, true);
            achievements.AddAchievement("calculated", "retries");
            achievements.AddAchievement("calculated2", "retries");

            bool achievementUnlocked = false;
            bool achievementUnlocked2 = false;
            achievements.AchievementUnlocked += (s, e) =>
            {
                if (e == "calculated")
                {
                    achievementUnlocked = true;
                }
                else if (e == "calculated2")
                {
                    achievementUnlocked2 = true;
                }
            };

            achievements.SetValue("retries", 3);
            Assert.IsTrue(achievementUnlocked);

            achievements.SetValue("retries2", 2);
            Assert.IsTrue(achievementUnlocked2);
        }
    }
}
