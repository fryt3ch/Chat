namespace Chat.Domain.Common.Results;

public interface IResultError
{
    string Error { get; }
    string Code { get; }
}