using BaseProject.Application.Common.Results;

namespace BaseProject.WebAPI.Extentions;

public static class ResultExtentions
{
    public static IResult ToResult(this Result result)
    {
        if (result.IsSuccess)
            return Results.Ok(result);
        else
            return Results.BadRequest(result);
    }

    public static IResult ToResult<T>(this DataResult<T?> dataResult)
        where T : class
    {
        if (dataResult.IsSuccess)
            return Results.Ok(dataResult);
        else if (dataResult.Data is null)
            return Results.NotFound(dataResult);
        else
            return Results.BadRequest(dataResult);
    }
    public static IResult ToResult<T>(this PagedDataResult<T?> dataResult)
    where T : class
    {
        if (dataResult.IsSuccess)
            return Results.Ok(dataResult);
        else if (dataResult.Data is null)
            return Results.NotFound(dataResult);
        else
            return Results.BadRequest(dataResult);
    }
}
