namespace Application.Models.Results;

public class DeserializationResult<T> where T : class
{
    public bool Success { get; }
    public T? Data { get; }
    public string? ErrorMessage { get; }
    public string RawContent { get; }

    private DeserializationResult(bool success, T? data, string? errorMessage, string rawContent)
    {
        Success = success;
        Data = data;
        ErrorMessage = errorMessage;
        RawContent = rawContent;
    }

    public static DeserializationResult<T> Successful(T data, string rawContent) =>
        new(true, data, null, rawContent);

    public static DeserializationResult<T> Failed(string errorMessage, string rawContent) =>
        new(false, null, errorMessage, rawContent);
}