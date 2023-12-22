namespace Chat.Application.Enums;

public enum ChatMessageType : byte
{
    Default = 0,
    Quoted = 1,
    Forwarded = 2,
}