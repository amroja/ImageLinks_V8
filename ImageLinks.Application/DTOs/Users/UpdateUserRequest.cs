namespace ImageLinks.Application.DTOs.Users;

public sealed record UpdateUserRequest(
    int Rec_ID,
    string User_Name,
    string Password);
