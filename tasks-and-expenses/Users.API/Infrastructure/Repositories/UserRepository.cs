using Microsoft.EntityFrameworkCore;
using Users.API.Application.Interfaces;
using Users.API.Models;

namespace Users.API.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(IUsersDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        // Ahora que password_hash existe en la DB, podemos usar EF normalmente
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
}
