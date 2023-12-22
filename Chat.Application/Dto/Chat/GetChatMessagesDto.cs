using FluentValidation;

namespace Chat.Application.Dto.Chat;

public record GetChatMessagesDto(IEnumerable<ChatMessageDto> Messages);

public record GetChatMessagesRequestDto(
    Guid? ChatId,
    
    int Limit,
    Guid? OffsetId,
    DateTime? OffsetDate,
    int? Offset,
    Guid? MaxId,
    Guid? MinId,
    DateTime? MaxDate,
    DateTime? MinDate,
    
    string? ContainsText,
    Guid? SenderId
);

public class GetChatMessagesRequestDtoValidator : AbstractValidator<GetChatMessagesRequestDto>
{
    public GetChatMessagesRequestDtoValidator()
    {
        RuleFor(x => x.Limit)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(100);
        
        RuleFor(x => x.OffsetId)
            .Empty()
            .When(x => x.OffsetDate != null);

        RuleFor(x => x.OffsetDate)
            .Empty()
            .When(x => x.OffsetId != null);

        RuleFor(x => x)
            .Must(x => x.OffsetDate != null || x.OffsetId != null);
        
        RuleFor(x => x.OffsetDate)
            .Must(x => x!.Value.Kind == DateTimeKind.Utc)
            .When(x => x.OffsetDate != null);
    }
}