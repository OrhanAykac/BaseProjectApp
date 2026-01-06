namespace BaseProject.Application.Common.Results;

public class Result
{
    public bool IsSuccess { get; }
    public IReadOnlyList<string> Errors { get; }

    protected Result(bool isSuccess, IReadOnlyList<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Errors = errors ?? [];
    }

    #region Base

    public static Result Success() => new(true);

    public static Result Fail() => new(false);

    public static Result Fail(string error) =>
        new(false, [error]);

    public static Result Fail(params string[] errors) =>
        new(false, errors);

    public static Result Fail(Exception exception) =>
        new(false, [exception.Message]);

    #endregion

    #region DataResult

    public static DataResult<T?> Success<T>(T data) =>
        new(data, true);

    public static DataResult<T?> Fail<T>(string error) =>
        new(default, false, [error]);

    public static DataResult<T?> Fail<T>(IReadOnlyList<string> errors) =>
        new(default, false, errors);

    #endregion

    #region PagedDataResult

    public static PagedDataResult<T?> Success<T>(
        T data,
        int pageIndex,
        int pageSize,
        int totalCount)
        => new(data, pageIndex, pageSize, totalCount, true);

    public static PagedDataResult<T?> FailPaged<T>(string error) =>
        new(default, 0, 0, 0, false, [error]);

    public static PagedDataResult<T?> FailPaged<T>(IReadOnlyList<string> errors) =>
        new(default, 0, 0, 0, false, errors);

    #endregion
}


public class DataResult<T>(
    T? data,
    bool isSuccess,
    IReadOnlyList<string>? errors = null)
{
    public T? Data { get; } = data;
    public bool IsSuccess { get; } = isSuccess;
    public IReadOnlyList<string> Errors { get; } = errors ?? [];
}

public class PagedDataResult<T>(
    T? data,
    int pageIndex,
    int pageSize,
    int totalCount,
    bool isSuccess,
    IReadOnlyList<string>? errors = null)
{

    public T? Data { get; } = data;
    public bool IsSuccess { get; } = isSuccess;
    public IReadOnlyList<string> Errors { get; } = errors ?? [];

    public int PageIndex { get; } = pageIndex;
    public int PageSize { get; } = pageSize;
    public int TotalCount { get; } = totalCount;

    public int TotalPages =>
        PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}