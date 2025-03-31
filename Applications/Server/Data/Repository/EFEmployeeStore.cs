using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Application.Areas.Identity.Data;
using Application.Services;

namespace Application.Data.Repository
{
    public class EFEmployeeStore : IEmployeeStore
    {
        public EFEmployeeStore(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        private readonly AppDbContext dbContext;


        public Employee Get(AppUser user)
        {
            if (user == null)
                return null;
            return dbContext.Employees.FirstOrDefault(x => x.UserId == user.Id);
        }
        public void Save(AppUser user, Employee client)
        {
            if (client == null)
            {
                return;
            }

            client.UserId = user.Id;

            Employee other = dbContext.Employees.FirstOrDefault(x => x.UserId == client.UserId);
            if (other != null)
            {
                other.FirstName = client.FirstName;
                other.LastName = client.LastName;
                other.Role = client.Role;
                other.Phone = client.Phone;
            }
            else
            {
                dbContext.Employees.Add(client);
            }

            dbContext.SaveChanges();
        }
    }
}
