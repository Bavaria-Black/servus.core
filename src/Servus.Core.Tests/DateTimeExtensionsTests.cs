using System;
using Xunit;

namespace Servus.Core.Tests;

public class DateTimeExtensionsTests
{
    [Fact]
    public void IsTodayTest()
    {
        Assert.True(DateTime.Now.IsToday());
    }

    [Fact]
    public void IsBetweenTest()
    {
        var dateTime = new DateTime(2019, 9, 18);
        Assert.True(dateTime.IsBetween(new DateTime(2019, 1, 1), new DateTime(2020, 1, 1)));
    }

    [Fact]
    public void IsBetweenInvertedTest()
    {
        var dateTime = new DateTime(2019, 9, 18);
        Assert.True(dateTime.IsBetween(new DateTime(2020, 1, 1), new DateTime(2019, 1, 1)));
    }

    [Fact]
    public void IsBetweenEdgeCasesTest()
    {
        var dateTime = new DateTime(2019, 9, 18);

        // upper edge
        Assert.True(dateTime.IsBetween(new DateTime(2019, 1, 1), new DateTime(2019, 9, 18)));

        // lower edge
        Assert.True(dateTime.IsBetween(new DateTime(2019, 9, 18), new DateTime(2020, 1, 1)));
    }

    [Theory]
    [InlineData(2019, 9, 16, true)]
    [InlineData(2019, 9, 17, true)]
    [InlineData(2019, 9, 18, true)]
    [InlineData(2019, 9, 19, true)]
    [InlineData(2019, 9, 20, true)]
    [InlineData(2019, 9, 21, false)]
    [InlineData(2019, 9, 22, false)]
    public void IsWorkdayTest(int year, int month, int day, bool isWorkday)
    {
        var dateTime = new DateTime(year, month, day);
        Assert.Equal(isWorkday, dateTime.IsWorkday());
    }

    [Theory]
    [InlineData(2019, 9, 16, false)]
    [InlineData(2019, 9, 17, false)]
    [InlineData(2019, 9, 18, false)]
    [InlineData(2019, 9, 19, false)]
    [InlineData(2019, 9, 20, false)]
    [InlineData(2019, 9, 21, true)]
    [InlineData(2019, 9, 22, true)]
    public void IsWeekendTest(int year, int month, int day, bool isWorkday)
    {
        var dateTime = new DateTime(year, month, day);
        Assert.Equal(isWorkday, dateTime.IsWeekend());
    }

    [Fact]
    public void IsPastTest()
    {
        Assert.True(DateTime.Now.IsPast());
        Assert.False(DateTime.Now.AddDays(1).IsPast());
    }

    [Fact]
    public void IsInFutureTest()
    {
        Assert.True(DateTime.Now.AddMinutes(20).IsInFuture());
        Assert.True(DateTime.UtcNow.AddMinutes(20).IsInFuture());
        Assert.False(DateTime.Now.IsInFuture());
    }
}