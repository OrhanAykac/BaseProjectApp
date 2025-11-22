namespace BaseProject.Application.Common.Results;

public class Result : IResult
{
    public bool IsSuccess { get; protected set; }
    public string Message { get; protected set; }
    public List<string> Errors { get; protected set; }

    protected Result(bool isSuccess = true, string? message = null, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Message = message ?? string.Empty;
        Errors = errors ?? [];
    }

    public static IResult Success() => new Result();

    public static IResult Success(string message) => new Result(true, message);

    public static IResult Fail() => new Result(false);

    public static IResult Fail(string message) => new Result(false, message);

    public static IResult Fail(List<string> errors) => new Result(false, null, errors);

    public static IResult Fail(string message, List<string> errors) => new Result(false, message, errors);

    public static IDataResult<T> Success<T>(T data) => new DataResult<T>(data, true);

    public static IDataResult<T> Success<T>(T data, string message) => new DataResult<T>(data, true, message);

    public static IDataResult<T> Success<T>(T data, int pageIndex, int pageSize, int totalCount) =>
        new PaggingDataResult<T>(data, pageIndex, pageSize, totalCount, true);

    public static IDataResult<T> Success<T>(T data, int pageIndex, int pageSize, int totalCount, string message) =>
    new PaggingDataResult<T>(data, pageIndex, pageSize, totalCount, true, message);


    public static IDataResult<T> Fail<T>() => new DataResult<T>(default!, false);

    public static IDataResult<T> Fail<T>(string message) => new DataResult<T>(default!, false, message);

    public static IDataResult<T> Fail<T>(List<string> errors) => new DataResult<T>(default!, false, null, errors);

    public static IDataResult<T> Fail<T>(string message, List<string> errors) => new DataResult<T>(default!, false, message, errors);
}

public class DataResult<T> : Result, IDataResult<T>
{
    public T Data { get; private set; }

    internal DataResult(T data, bool isSuccess = true, string? message = null, List<string>? errors = null)
        : base(isSuccess, message, errors)
    {
        Data = data;
    }
}
public class PaggingDataResult<T> : DataResult<T>, IPaggingDataResult<T>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)System.Math.Ceiling(TotalCount / (double)PageSize);

    internal PaggingDataResult(
        T data, int pageIndex, int pageSize, int totalCount, bool isSuccess = true, string? message = null, List<string>? errors = null)
        : base(data, isSuccess, message, errors)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
}