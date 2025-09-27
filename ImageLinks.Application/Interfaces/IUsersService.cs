using System.Collections.Generic;
using ImageLinks.Application.DTOs.Users;
using ImageLinks.Domain.Results;

namespace ImageLinks.Application.Interfaces;

public interface IUsersService
{
    Task<Result<IReadOnlyList<UserResponse>>> GetAllAsync(CancellationToken ct = default);

    Task<Result<UserResponse>> GetByIdAsync(int recId, CancellationToken ct = default);

    Task<Result<UserResponse>> CreateAsync(CreateUserRequest request, CancellationToken ct = default);

    Task<Result<UserResponse>> UpdateAsync(UpdateUserRequest request, CancellationToken ct = default);

    Task<Result<Deleted>> DeleteAsync(int recId, CancellationToken ct = default);
}
