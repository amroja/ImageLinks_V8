using ImageLinks.Application.DTOs.Users;
using ImageLinks.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ImageLinks.Api.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : ApiController
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
        {
            var result = await _usersService.GetAllAsync(cancellationToken);

            return result.Match<IActionResult>(
                users => Ok(users),
                errors => Problem(errors));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUser(int id, CancellationToken cancellationToken)
        {
            var result = await _usersService.GetByIdAsync(id, cancellationToken);

            return result.Match<IActionResult>(
                user => Ok(user),
                errors => Problem(errors));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto request, CancellationToken cancellationToken)
        {
            var result = await _usersService.CreateAsync(request, cancellationToken);

            return result.Match<IActionResult>(
                user => CreatedAtAction(nameof(GetUser), new { id = user.Id }, user),
                errors => Problem(errors));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto request, CancellationToken cancellationToken)
        {
            var result = await _usersService.UpdateAsync(id, request, cancellationToken);

            return result.Match<IActionResult>(
                _ => NoContent(),
                errors => Problem(errors));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id, CancellationToken cancellationToken)
        {
            var result = await _usersService.DeleteAsync(id, cancellationToken);

            return result.Match<IActionResult>(
                _ => NoContent(),
                errors => Problem(errors));
        }
    }
}
