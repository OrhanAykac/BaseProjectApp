namespace BaseProject.Application.Utilities.Results;

public static class ResultExtensions
{
    public static Result WithError(this Result result, string error)
    {
        result.Errors.Add(error);
        return result;
    }

    public static Result WithErrors(this Result result, IEnumerable<string> errors)
    {
        result.Errors.AddRange(errors);
        return result;
    }

    public static IDataResult<T> WithError<T>(this IDataResult<T> result, string error)
    {
        result.Errors.Add(error);
        return result;
    }

    public static IDataResult<T> WithErrors<T>(this IDataResult<T> result, IEnumerable<string> errors)
    {
        result.Errors.AddRange(errors);
        return result;
    }
}