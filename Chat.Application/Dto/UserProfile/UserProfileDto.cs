using Chat.Application.Enums;
using Chat.Domain.Enums;

namespace Chat.Application.Dto.UserProfile;

public record UserProfileDto(
    Guid Id, 
    string Username, 
    string Name, 
    string Surname, 
    Gender Gender, 
    Country Country, 
    DateTime BirthDate, 
    string? AvatarPhotoId, 
    ProfileColor Color
);

public record UserProfileFullDto(
    Guid Id, 
    string Username, 
    string Name, 
    string Surname, 
    Gender Gender, 
    Country Country, 
    DateTime BirthDate, 
    string? AvatarPhotoId, 
    ProfileColor Color
) : UserProfileDto(Id, Username, Name, Surname, Gender, Country, BirthDate, AvatarPhotoId, Color);