using Application.Data;
using Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Services.Repository
{
    public class ClientRepository : Repository<Client, string>, IClientRepository
    {
        public ClientRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Client?> GetByPhoneAsync(string phone)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Phone == phone);
        }

        public async Task<IEnumerable<Client>> GetByNameAsync(string firstName, string lastName)
        {
            return await _dbSet
                .Where(c => c.FirstName == firstName && c.LastName == lastName)
                .ToListAsync();
        }

        public async Task<Client?> GetWithUserAsync(string userId)
        {
            return await _dbSet
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<bool> ExistsByPhoneAsync(string phone)
        {
            return await _dbSet
                .AnyAsync(c => c.Phone == phone);
        }
    }
}