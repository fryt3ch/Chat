using Chat.Application.Dto.UserProfile;
using Chat.Application.Enums;

namespace Chat.Application.Dto.Chat;

public abstract record ChatPreviewDto(Guid Id, ChatType ChatType, ChatLastMessagePreviewDto? LastMessagePreview);

public record UserChatPreviewDto(Guid Id, UserProfileDto UserProfileDto, ChatLastMessagePreviewDto? LastMessagePreview) : ChatPreviewDto(Id, ChatType.User, LastMessagePreview);