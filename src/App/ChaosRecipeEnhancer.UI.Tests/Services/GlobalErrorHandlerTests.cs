using System.Reflection;
using ChaosRecipeEnhancer.UI.Services;

namespace ChaosRecipeEnhancer.UI.Tests.Services;

public class GlobalErrorHandlerTests
{
    private static object? InvokePrivateMethod(string methodName, object[] parameters)
    {
        var method = typeof(GlobalErrorHandler).GetMethod(
            methodName,
            BindingFlags.NonPublic | BindingFlags.Static);

        method.Should().NotBeNull($"Method {methodName} should exist");
        return method!.Invoke(null, parameters);
    }

    #region GenerateTimeoutString

    [Fact]
    public void GenerateTimeoutString_GivenSecondsOnly_ReturnsCorrectFormat()
    {
        var result = (string)InvokePrivateMethod("GenerateTimeoutString", new object[] { 30 })!;
        result.Should().Be("30 seconds");
    }

    [Fact]
    public void GenerateTimeoutString_GivenMinutesAndSeconds_ReturnsWithAnd()
    {
        var result = (string)InvokePrivateMethod("GenerateTimeoutString", new object[] { 150 })!;
        result.Should().Contain("2 minutes");
        result.Should().Contain("30 seconds");
        result.Should().Contain("and");
    }

    [Fact]
    public void GenerateTimeoutString_GivenHoursMinutesSeconds_ReturnsFullString()
    {
        var result = (string)InvokePrivateMethod("GenerateTimeoutString", new object[] { 3661 })!;
        result.Should().Contain("1 hour");
        result.Should().Contain("1 minute");
        result.Should().Contain("1 second");
    }

    [Fact]
    public void GenerateTimeoutString_GivenZeroSeconds_ReturnsEmptyString()
    {
        var result = (string)InvokePrivateMethod("GenerateTimeoutString", new object[] { 0 })!;
        result.Should().BeEmpty();
    }

    [Fact]
    public void GenerateTimeoutString_GivenExactlyOneMinute_ReturnsSingular()
    {
        var result = (string)InvokePrivateMethod("GenerateTimeoutString", new object[] { 60 })!;
        result.Should().Be("1 minute");
    }

    [Fact]
    public void GenerateTimeoutString_GivenPluralHours_ReturnsPlural()
    {
        var result = (string)InvokePrivateMethod("GenerateTimeoutString", new object[] { 7200 })!;
        result.Should().Be("2 hours");
    }

    #endregion

    #region TruncateResponseString

    [Fact]
    public void TruncateResponseString_GivenShortString_ReturnsUnchanged()
    {
        string shortString = "This is a short response";
        var result = (string)InvokePrivateMethod("TruncateResponseString", new object[] { shortString })!;
        result.Should().Be(shortString);
    }

    [Fact]
    public void TruncateResponseString_GivenLongString_TruncatesAndAppendsSuffix()
    {
        string longString = new string('a', 600);
        var result = (string)InvokePrivateMethod("TruncateResponseString", new object[] { longString })!;

        // Source: responseString[..500] + "...\n\n(Truncated for brevity)"
        result.Should().StartWith(new string('a', 50));
        result.Should().EndWith("(Truncated for brevity)");
        result.Length.Should().BeLessThan(600);
    }

    [Fact]
    public void TruncateResponseString_GivenExactly500Chars_ReturnsUnchanged()
    {
        string exactString = new string('b', 500);
        var result = (string)InvokePrivateMethod("TruncateResponseString", new object[] { exactString })!;
        result.Should().Be(exactString);
    }

    #endregion
}
