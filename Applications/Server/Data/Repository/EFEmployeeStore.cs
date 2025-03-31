using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Application.Areas.Identity.Data;
using Application.Services;
using Application.Model.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Data.Repository
{
    public class EFEmployeeStore : IEmployeeStore
    {
        private readonly AppDbContext dbContext;

        public EFEmployeeStore(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IQueryable<Employee> All => dbContext.Employees;

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await dbContext.Employees.ToListAsync();
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            return await dbContext.Employees.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Employee Get(int id)
        {
            return dbContext.Employees.FirstOrDefault(x => x.Id == id);
        }

        public void Save(Employee employee)
        {
            if (employee == null)
            {
                dbContext.SaveChanges();
                return;
            }

            if (employee.Id == default)
                dbContext.Entry(employee).State = EntityState.Added;
            else
                dbContext.Entry(employee).State = EntityState.Modified;

            dbContext.SaveChanges();
        }

        public void Delete(Employee employee)
        {
            if (employee.Id == default)
            {
                return;
            }
            dbContext.Entry(employee).State = EntityState.Deleted;
            dbContext.SaveChanges();
        }

        public async Task CreateAsync(Employee employee)
        {
            await dbContext.Employees.AddAsync(employee);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Employee employee)
        {
            dbContext.Entry(employee).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var employee = await GetByIdAsync(id);
            if (employee != null)
            {
                dbContext.Employees.Remove(employee);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
