using FluentValidation;

namespace Chat.Application.Dto.Chat;

public record DeleteChatMessagesDto(Guid ChatId, IEnumerable<Guid> Messages);
public record DeleteChatMessagesRequestDto(
    Guid ChatId,
    
    HashSet<Guid> MessageIds
);

public class DeleteChatMessagesRequestDtoValidator : AbstractValidator<DeleteChatMessagesRequestDto>
{
    public DeleteChatMessagesRequestDtoValidator()
    {
        RuleFor(x => x.MessageIds)
            .Must(x => x.Count >= 1 && x.Count <= 100);
    }
}