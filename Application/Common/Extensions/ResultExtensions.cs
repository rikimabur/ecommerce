using Application.Common.Models;
using Error = Application.Common.Models.Error;

namespace Application.Common.Extensions
{
    public static class ResultExtensions
    {
        public static T Match<T>(
        this Result result,
        Func<T> onSuccess,
        Func<Error, T> onFailure)
        {
            return result.IsSuccess ? onSuccess() : onFailure(result.Error);
        }
    }
}
