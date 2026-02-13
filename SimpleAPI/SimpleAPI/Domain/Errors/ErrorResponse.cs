namespace SimpleAPI.Domain.Errors;

public class ErrorResponse
{
    public required string ErrorMessage { get; init; }
    public int ErrorCode { get; init; }
}
