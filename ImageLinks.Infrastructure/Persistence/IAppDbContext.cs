using ImageLinks.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace ImageLinks.Infrastructure.Persistence
{
    public interface IAppDbContext
    {
        public DbSet<User> Customers { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
