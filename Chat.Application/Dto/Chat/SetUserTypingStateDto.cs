using Chat.Application.Enums;
using FluentValidation;

namespace Chat.Application.Dto.Chat;

public record SetUserTypingStateDto(Guid ChatId, Guid UserId, ChatUserTypingState State);

public record SetUserTypingStateRequestDto(Guid ChatId, ChatUserTypingState State);

public class SetUserTypingStateRequestDtoValidator : AbstractValidator<SetUserTypingStateRequestDto>
{
    public SetUserTypingStateRequestDtoValidator()
    {
        return;
        
        RuleFor(x => x.State)
            .NotEqual(ChatUserTypingState.None);
    }
}