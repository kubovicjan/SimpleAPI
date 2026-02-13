using SimpleAPI.Core;
using SimpleAPI.Core.Configuration;
using Xunit;

namespace SimpleAPI.Tests.Core;

public class ApiKeyValidatorTests
{
    [Theory]
    [InlineData("apikey-123", true)]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("OtherKey-123", false)]
    public void IsValid_ReturnsExpectedResult(string? apiKey, bool expected)
    {
        var config = new ApiConfiguration("https://example.test", "ApiKey-");
        var validator = new ApiKeyValidator(config);

        var result = validator.IsValid(apiKey);

        Assert.Equal(expected, result);
    }
}