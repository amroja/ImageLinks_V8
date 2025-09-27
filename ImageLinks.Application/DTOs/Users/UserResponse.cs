namespace ImageLinks.Application.DTOs.Users;

public sealed record UserResponse(
    int Rec_ID,
    string User_Name,
    string Password);
