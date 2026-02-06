using Microsoft.EntityFrameworkCore;
using Users.API.Models;

namespace Users.API.Application.Interfaces;

public interface IUsersDbContext
{
    DbSet<User> Users { get; }
    DbSet<T> Set<T>() where T : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

