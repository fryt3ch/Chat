using FluentValidation;

namespace Chat.Application.Dto.Chat;

public record MessagesNewDto(IEnumerable<ChatMessageDto> Messages);

public record SendChatMessageRequestDto(
    Guid ChatId,
    
    string Content, 
    
    Guid? QuotedMessageId,
    
    IList<Guid>? ForwardedMessages,
    Guid? SourceChatId
);

public class SendChatMessageRequestDtoValidator : AbstractValidator<SendChatMessageRequestDto>
{
    public SendChatMessageRequestDtoValidator()
    {
        RuleFor(x => x.QuotedMessageId)
            .Empty()
            .When(x => x.ForwardedMessages != null);

        RuleFor(x => x.ForwardedMessages)
            .Empty()
            .When(x => x.QuotedMessageId != null);

        RuleFor(x => x.ForwardedMessages)
            .Must(x => x.Count >= 1 && x.Count <= 100)
            .When(x => x.ForwardedMessages != null);

        RuleFor(x => x.SourceChatId)
            .NotEmpty()
            .When(x => x.ForwardedMessages != null);
    }
}