namespace Chat.Domain.Common.Results;

public interface IResult
{
    IResultError? Error { get; }
    string Message { get; }

    bool Succeeded { get; }
}

public interface IResult<out T> : IResult
{
    T Data { get; }
}