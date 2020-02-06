using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DevTools.Core.Tests
{
    [TestClass]
    public class DateTimeExtensionsTests
    {
        [TestMethod]
        public void IsTodayTest()
        {
            Assert.IsTrue(DateTime.Now.IsToday());
        }

        [TestMethod]
        public void IsBetweenTest()
        {
            var dateTime = new DateTime(2019, 9, 18);
            Assert.IsTrue(dateTime.IsBetween(new DateTime(2019, 1, 1), new DateTime(2020, 1, 1)));
        }

        [TestMethod]
        public void IsBetweenInvertedTest()
        {
            var dateTime = new DateTime(2019, 9, 18);
            Assert.IsTrue(dateTime.IsBetween(new DateTime(2020, 1, 1), new DateTime(2019, 1, 1)));
        }

        [TestMethod]
        public void IsBetweenEdgeCasesTest()
        {
            var dateTime = new DateTime(2019, 9, 18);

            // upper edge
            Assert.IsTrue(dateTime.IsBetween(new DateTime(2019, 1, 1), new DateTime(2019, 9, 18)));

            // lower edge
            Assert.IsTrue(dateTime.IsBetween(new DateTime(2019, 9, 18), new DateTime(2020, 1, 1)));
        }

        [TestMethod]
        [DataRow(2019, 9, 16, true)]
        [DataRow(2019, 9, 17, true)]
        [DataRow(2019, 9, 18, true)]
        [DataRow(2019, 9, 19, true)]
        [DataRow(2019, 9, 20, true)]
        [DataRow(2019, 9, 21, false)]
        [DataRow(2019, 9, 22, false)]
        public void IsWorkdayTest(int year, int month, int day, bool isWorkday)
        {
            var dateTime = new DateTime(year, month, day);
            Assert.AreEqual(isWorkday, dateTime.IsWorkday());
        }

        [TestMethod]
        [DataRow(2019, 9, 16, false)]
        [DataRow(2019, 9, 17, false)]
        [DataRow(2019, 9, 18, false)]
        [DataRow(2019, 9, 19, false)]
        [DataRow(2019, 9, 20, false)]
        [DataRow(2019, 9, 21, true)]
        [DataRow(2019, 9, 22, true)]
        public void IsWeekendTest(int year, int month, int day, bool isWorkday)
        {
            var dateTime = new DateTime(year, month, day);
            Assert.AreEqual(isWorkday, dateTime.IsWeekend());
        }

        [TestMethod]
        public void IsPastTest()
        {
            Assert.IsTrue(DateTime.Now.IsPast());
            Assert.IsFalse(DateTime.Now.AddDays(1).IsPast());
        }

        [TestMethod]
        public void IsInFutureTest()
        {
            Assert.IsTrue(DateTime.Now.AddMinutes(20).IsInFuture());
            Assert.IsTrue(DateTime.UtcNow.AddMinutes(20).IsInFuture());
            Assert.IsFalse(DateTime.Now.IsInFuture());
        }
    }
}
