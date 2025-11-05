using StackExchange.Redis;

namespace Pharmacy.Application.Common.Models;
public class ResultDto<T>
{
    public int StatusCode { get; set; }
    public string? Timestamp { get; set; }

    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public object? Error { get; set; }

    public static ResultDto<T> Success(T? data, string Message = "Succes", int statusCode = 200)
         => new() { Succeeded = true, Message = Message, Data = data, StatusCode = statusCode};

    public static ResultDto<T> Failure(string Message, object? error = null, int statusCode = 400)
         => new() { Succeeded = false, Message = Message, Error = error, StatusCode = statusCode};
}

