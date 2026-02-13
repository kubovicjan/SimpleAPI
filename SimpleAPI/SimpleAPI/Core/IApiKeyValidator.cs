namespace SimpleAPI.Core;

public interface IApiKeyValidator
{
    bool IsValid(string? apiKey);
}