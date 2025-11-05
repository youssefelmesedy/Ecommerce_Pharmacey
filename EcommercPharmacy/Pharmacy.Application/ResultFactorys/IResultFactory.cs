using Pharmacy.Application.Common.Models;

namespace Pharmacy.Application.ResultFactorys;
public interface IResultFactory
{
    ResultDto<T> Success<T>(T data, string message);
    ResultDto<T> Failure<T>(string messaga, object? error = null);
}
