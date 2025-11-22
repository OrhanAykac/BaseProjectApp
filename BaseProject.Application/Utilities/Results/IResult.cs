namespace BaseProject.Application.Utilities.Results;

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
