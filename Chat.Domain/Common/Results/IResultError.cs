namespace Chat.Domain.Common.Results;

public interface IResultError
{
    string Message { get; }
    string Code { get; }
}