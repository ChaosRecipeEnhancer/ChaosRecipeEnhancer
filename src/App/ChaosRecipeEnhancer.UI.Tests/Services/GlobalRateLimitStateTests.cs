using ChaosRecipeEnhancer.UI.Services;

namespace ChaosRecipeEnhancer.UI.Tests.Services;

/// <summary>
/// Tests for GlobalRateLimitState static class.
/// 
/// NOTE: CheckForBan(), SetRateLimitExpiresOn(), and GetSecondsToWait() all access
/// Properties.Settings.Default.RateLimitExpiresOn which requires WPF Application infrastructure
/// and throws NullReferenceException in unit test context. These methods need the static
/// dependency on Settings.Default to be refactored to an injected IUserSettings before
/// they can be unit tested. The same limitation affects PoeApiService tests since every
/// API call routes through CheckForBan().
/// </summary>
[Collection("GlobalRateLimitState")]
public class GlobalRateLimitStateTests
{
    private void ResetGlobalState()
    {
        GlobalRateLimitState.RateLimitState = new int[3];
        GlobalRateLimitState.RateLimitExceeded = false;
        GlobalRateLimitState.BanTime = 0;
        GlobalRateLimitState.RequestCounter = 0;
        GlobalRateLimitState.ResponseSeconds = 0;
    }

    #region DeserializeRateLimits

    [Fact]
    public void DeserializeRateLimits_GivenValidStateString_UpdatesRateLimitStateArray()
    {
        // Arrange
        ResetGlobalState();

        // Act
        GlobalRateLimitState.DeserializeRateLimits("", "5:60:0,10:120:0");

        // Assert
        GlobalRateLimitState.RateLimitState.Should().HaveCount(3);
        GlobalRateLimitState.RateLimitState[0].Should().Be(5);
        GlobalRateLimitState.RateLimitState[1].Should().Be(60);
        GlobalRateLimitState.RateLimitState[2].Should().Be(0);
    }

    [Fact]
    public void DeserializeRateLimits_GivenBanState_SetsRateLimitState2()
    {
        // Arrange
        ResetGlobalState();

        // Act
        GlobalRateLimitState.DeserializeRateLimits("", "15:300:1,20:600:0");

        // Assert
        GlobalRateLimitState.RateLimitState[0].Should().Be(15);
        GlobalRateLimitState.RateLimitState[1].Should().Be(300);
        GlobalRateLimitState.RateLimitState[2].Should().Be(1);
    }

    #endregion

    #region DeserializeResponseSeconds

    [Fact]
    public void DeserializeResponseSeconds_GivenValidDateString_ParsesSeconds()
    {
        // Arrange
        ResetGlobalState();

        // Act
        GlobalRateLimitState.DeserializeResponseSeconds("Mon, 01 Jan 2024 12:30:45 GMT");

        // Assert
        GlobalRateLimitState.ResponseSeconds.Should().Be(45);
    }

    [Fact]
    public void DeserializeResponseSeconds_GivenDifferentTime_ExtractsCorrectSeconds()
    {
        // Arrange
        ResetGlobalState();

        // Act
        GlobalRateLimitState.DeserializeResponseSeconds("Fri, 15 Mar 2024 08:15:30 GMT");

        // Assert
        GlobalRateLimitState.ResponseSeconds.Should().Be(30);
    }

    #endregion

    #region Property State Tests

    [Fact]
    public void MaximumRequests_DefaultValue_Is45()
    {
        GlobalRateLimitState.MaximumRequests.Should().Be(45);
    }

    [Fact]
    public void RateLimitState_AfterReset_IsArrayOfThreeZeroes()
    {
        ResetGlobalState();
        GlobalRateLimitState.RateLimitState.Should().HaveCount(3);
        GlobalRateLimitState.RateLimitState.Should().AllBeEquivalentTo(0);
    }

    [Fact]
    public void RateLimitExceeded_CanBeToggled()
    {
        ResetGlobalState();
        GlobalRateLimitState.RateLimitExceeded = true;
        GlobalRateLimitState.RateLimitExceeded.Should().BeTrue();
        GlobalRateLimitState.RateLimitExceeded = false;
        GlobalRateLimitState.RateLimitExceeded.Should().BeFalse();
    }

    [Fact]
    public void BanTime_CanBeSetAndRetrieved()
    {
        ResetGlobalState();
        GlobalRateLimitState.BanTime = 120;
        GlobalRateLimitState.BanTime.Should().Be(120);
        GlobalRateLimitState.BanTime = 0;
    }

    #endregion
}
