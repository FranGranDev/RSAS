using Application.Areas.Identity.Data;
using Application.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Repository
{
    public class EFEmployeeStore : IEmployeeStore
    {
        private readonly AppDbContext _context;

        public EFEmployeeStore(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Employee> All => _context.Employees;

        public Employee Get(string id)
        {
            return _context.Employees
                .Include(e => e.User)
                .FirstOrDefault(e => e.UserId == id);
        }

        public void Save(Employee entity)
        {
            if (string.IsNullOrEmpty(entity.UserId))
            {
                _context.Employees.Add(entity);
            }
            else
            {
                _context.Entry(entity).State = EntityState.Modified;
            }

            _context.SaveChanges();
        }

        public void Delete(Employee entity)
        {
            _context.Employees.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .Include(e => e.User)
                .ToListAsync();
        }

        public async Task<Employee> GetByIdAsync(string id)
        {
            return await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.UserId == id);
        }

        public async Task<Employee> SaveAsync(Employee entity)
        {
            if (string.IsNullOrEmpty(entity.UserId))
            {
                _context.Employees.Add(entity);
            }
            else
            {
                _context.Entry(entity).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(string id)
        {
            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.UserId == id);
                
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Employee> GetByUserIdAsync(string userId)
        {
            return await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.UserId == userId);
        }
    }
}