using Pharmacy.Application.Common.Models;

namespace Pharmacy.Application.ResultFactorys;
public class ResultFactory : IResultFactory
{
    public ResultDto<T> Failure<T>(string messaga, object? error = null)
    {
        return ResultDto<T>.Failure(messaga, error);
    }

    public ResultDto<T> Success<T>(T data, string message)
    {
        return ResultDto<T>.Success(data, message);
    }
}
