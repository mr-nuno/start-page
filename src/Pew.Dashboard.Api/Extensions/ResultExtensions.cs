using Ardalis.Result;
using Pew.Dashboard.Application.Common.Models;

namespace Pew.Dashboard.Api.Extensions;

public static class ResultExtensions
{
    public static ApiResponse<T> ToApiResponse<T>(this Result<T> result) => result.Status switch
    {
        ResultStatus.Ok => new ApiResponse<T> { Success = true, Data = result.Value },
        ResultStatus.NotFound => new ApiResponse<T> { Success = false, Error = result.Errors.FirstOrDefault() ?? "Not found" },
        ResultStatus.Invalid => new ApiResponse<T>
        {
            Success = false,
            ValidationErrors = result.ValidationErrors
                .Select(e => new Pew.Dashboard.Application.Common.Models.ValidationError(e.Identifier, e.ErrorMessage))
                .ToList()
        },
        ResultStatus.Conflict => new ApiResponse<T> { Success = false, Error = result.Errors.FirstOrDefault() ?? "Conflict" },
        _ => new ApiResponse<T> { Success = false, Error = result.Errors.FirstOrDefault() ?? "An error occurred" }
    };

    public static int ToHttpStatusCode<T>(this Result<T> result) => result.Status switch
    {
        ResultStatus.Ok => 200,
        ResultStatus.NotFound => 404,
        ResultStatus.Invalid => 400,
        ResultStatus.Conflict => 409,
        _ => 500
    };

    public static ApiResponse<object> ToApiResponse(this Result result) => result.Status switch
    {
        ResultStatus.Ok => new ApiResponse<object> { Success = true },
        ResultStatus.NotFound => new ApiResponse<object> { Success = false, Error = result.Errors.FirstOrDefault() ?? "Not found" },
        ResultStatus.Invalid => new ApiResponse<object>
        {
            Success = false,
            ValidationErrors = result.ValidationErrors
                .Select(e => new Pew.Dashboard.Application.Common.Models.ValidationError(e.Identifier, e.ErrorMessage))
                .ToList()
        },
        ResultStatus.Conflict => new ApiResponse<object> { Success = false, Error = result.Errors.FirstOrDefault() ?? "Conflict" },
        _ => new ApiResponse<object> { Success = false, Error = result.Errors.FirstOrDefault() ?? "An error occurred" }
    };

    public static int ToHttpStatusCode(this Result result) => result.Status switch
    {
        ResultStatus.Ok => 200,
        ResultStatus.NotFound => 404,
        ResultStatus.Invalid => 400,
        ResultStatus.Conflict => 409,
        _ => 500
    };
}
