using Application.Data;
using Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Services.Repository
{
    public class EmployeeRepository : Repository<Employee, string>, IEmployeeRepository
    {
        public EmployeeRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Employee?> GetByPhoneAsync(string phone)
        {
            return await _dbSet
                .FirstOrDefaultAsync(e => e.Phone == phone);
        }

        public async Task<IEnumerable<Employee>> GetByNameAsync(string firstName, string lastName)
        {
            return await _dbSet
                .Where(e => e.FirstName == firstName && e.LastName == lastName)
                .ToListAsync();
        }

        public async Task<Employee?> GetWithUserAsync(string userId)
        {
            return await _dbSet
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.UserId == userId);
        }

        public async Task<IEnumerable<Employee>> GetByRoleAsync(string role)
        {
            return await _dbSet
                .Where(e => e.Role == role)
                .ToListAsync();
        }

        public async Task<bool> ExistsByPhoneAsync(string phone)
        {
            return await _dbSet
                .AnyAsync(e => e.Phone == phone);
        }
    }
}