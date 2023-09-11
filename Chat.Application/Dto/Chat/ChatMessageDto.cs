namespace Chat.Application.Dto.Chat;

public class ChatMessageDto
{
    public Guid ReceiverId { get; set; }
    
    public string Message { get; set; }
}