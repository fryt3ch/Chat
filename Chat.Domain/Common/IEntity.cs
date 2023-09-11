namespace Chat.Domain.Common;

public interface IEntity<T> where T : struct
{
    public T Id { get; protected set; }
}