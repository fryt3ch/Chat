using FluentValidation;

namespace Chat.Application.Dto.Chat;

public record ChatReadRequestDto(Guid ChatId, DateTime LastReadMessageSentAt);

public record ChatReadDto(Guid ChatId, DateTime LastReadMessageSentAt, int UnreadMessagesAmount);

public class ChatReadRequestDtoValidator : AbstractValidator<ChatReadRequestDto>
{
    public ChatReadRequestDtoValidator()
    {
        RuleFor(x => x.LastReadMessageSentAt).Must(x => x.Kind == DateTimeKind.Utc);
    }
}