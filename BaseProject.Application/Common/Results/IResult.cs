namespace BaseProject.Application.Common.Results;

public interface IResult
{
    bool IsSuccess { get; }
    string Message { get; }
    List<string> Errors { get; }
}

public interface IDataResult<out T> : IResult
{
    T Data { get; }
}

public interface IPaggingDataResult<out T> : IDataResult<T>, IResult
{
    int PageIndex { get; set; }
    int PageSize { get; set; }
    int TotalCount { get; set; }
    int TotalPages { get; }
}
