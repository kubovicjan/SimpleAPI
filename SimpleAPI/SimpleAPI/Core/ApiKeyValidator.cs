using SimpleAPI.Core.Configuration;

namespace SimpleAPI.Core;

public class ApiKeyValidator : IApiKeyValidator
{
    private string ValidApiKeyPrefix { get; }

    public ApiKeyValidator(ApiConfiguration apiConfig)
    {
        ValidApiKeyPrefix = apiConfig.ApiKey;
    }

    public bool IsValid(string? apiKey)
    {
        return !string.IsNullOrEmpty(apiKey) &&
            apiKey.StartsWith(ValidApiKeyPrefix, StringComparison.OrdinalIgnoreCase);
    }
}
