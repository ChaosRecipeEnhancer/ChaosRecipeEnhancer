using ChaosRecipeEnhancer.UI.Services;
using ChaosRecipeEnhancer.UI.Utilities;
using Moq;
using Serilog;

namespace ChaosRecipeEnhancer.UI.Tests.Utilities;

public class HttpHeaderUtilitiesTests
{
    [Fact]
    public void ProcessRateLimitHeaders_GivenNullResponse_DoesNotThrow()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var requestUri = new Uri("https://api.example.com/test");

        // Act
        var action = () => HttpHeaderUtilities.ProcessRateLimitHeaders(null, mockLogger.Object, requestUri);

        // Assert
        action.Should().NotThrow();
    }

    [Fact]
    public void ProcessRateLimitHeaders_GivenResponseWithHeaders_UpdatesGlobalState()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var requestUri = new Uri("https://api.example.com/test");

        var response = new HttpResponseMessage();
        response.Headers.Add("X-Rate-Limit-Account-State", "5:60:0,10:120:0");
        response.Headers.Add("X-Rate-Limit-Account", "1:4");
        response.Headers.Add("Date", "Mon, 01 Jan 2024 12:30:45 GMT");

        // Act
        HttpHeaderUtilities.ProcessRateLimitHeaders(response, mockLogger.Object, requestUri);

        // Assert
        GlobalRateLimitState.RateLimitState.Should().NotBeNull();
        GlobalRateLimitState.RateLimitState.Should().HaveCount(3);
    }

    [Fact]
    public void ProcessRateLimitHeaders_GivenResponseWithoutHeaders_DoesNotThrow()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var requestUri = new Uri("https://api.example.com/test");
        var response = new HttpResponseMessage();

        // Act
        var action = () => HttpHeaderUtilities.ProcessRateLimitHeaders(response, mockLogger.Object, requestUri);

        // Assert
        action.Should().NotThrow();
    }

    [Fact]
    public void ProcessRateLimitHeaders_GivenResponseWithPartialHeaders_HandlesGracefully()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var requestUri = new Uri("https://api.example.com/test");

        var response = new HttpResponseMessage();
        response.Headers.Add("X-Rate-Limit-Account-State", "5:60:0,10:120:0");

        // Act
        var action = () => HttpHeaderUtilities.ProcessRateLimitHeaders(response, mockLogger.Object, requestUri);

        // Assert
        action.Should().NotThrow();
    }
}
