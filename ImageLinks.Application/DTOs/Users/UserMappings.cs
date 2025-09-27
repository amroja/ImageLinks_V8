using ImageLinks.Domain.Users;

namespace ImageLinks.Application.DTOs.Users;

public static class UserMappings
{
    public static UserResponse ToUserResponse(this User user)
        => new(user.Rec_ID, user.User_Name, user.Password);

    public static User ToDomain(this CreateUserRequest request)
        => new()
        {
            Rec_ID = request.Rec_ID ?? 0,
            User_Name = request.User_Name,
            Password = request.Password,
        };

    public static void ApplyUpdates(this UpdateUserRequest request, User user)
    {
        user.User_Name = request.User_Name;
        user.Password = request.Password;
    }

    public static UpdateUserRequest ToUpdateRequest(this User user)
        => new(user.Rec_ID, user.User_Name, user.Password);
}
