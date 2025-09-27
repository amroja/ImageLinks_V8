using ImageLinks.Application.DTOs.Users;
using ImageLinks.Domain.Results;

namespace ImageLinks.Application.Interfaces
{
    public interface IUsersService
    {
        Task<Result<IReadOnlyList<UserDto>>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<Result<UserDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<Result<UserDto>> CreateAsync(CreateUserDto user, CancellationToken cancellationToken = default);

        Task<Result<Updated>> UpdateAsync(int id, UpdateUserDto user, CancellationToken cancellationToken = default);

        Task<Result<Deleted>> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
