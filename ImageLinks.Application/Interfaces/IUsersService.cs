using ImageLinks.Domain.Users;

namespace ImageLinks.Application.Interfaces
{
    public interface IUsersService
    {
        Task<IEnumerable<User>> GetUsersAsync(
            string? connectionString = null,
            CancellationToken ct = default);

        Task<User?> GetUserByIdAsync(
            int recId,
            string? connectionString = null,
            CancellationToken ct = default);

        Task<int> CreateUserAsync(
            User user,
            string? connectionString = null,
            CancellationToken ct = default);

        Task<bool> UpdateUserAsync(
            User user,
            string? connectionString = null,
            CancellationToken ct = default);

        Task<bool> DeleteUserAsync(
            int recId,
            string? connectionString = null,
            CancellationToken ct = default);
    }
}
