namespace ImageLinks.Application.DTOs.Users;

public sealed record CreateUserRequest(
    int? Rec_ID,
    string User_Name,
    string Password);
