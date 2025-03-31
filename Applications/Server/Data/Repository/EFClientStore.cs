using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Application.Areas.Identity.Data;
using Application.Services;
using Application.Model.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Data.Repository
{
    public class EFClientStore : IClientsStore
    {
        private readonly AppDbContext dbContext;

        public EFClientStore(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IQueryable<Client> All => dbContext.Clients;

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            return await dbContext.Clients.ToListAsync();
        }

        public async Task<Client> GetByIdAsync(int id)
        {
            return await dbContext.Clients.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Client Get(int id)
        {
            return dbContext.Clients.FirstOrDefault(x => x.Id == id);
        }

        public void Save(Client client)
        {
            if (client == null)
            {
                dbContext.SaveChanges();
                return;
            }

            if (client.Id == default)
                dbContext.Entry(client).State = EntityState.Added;
            else
                dbContext.Entry(client).State = EntityState.Modified;

            dbContext.SaveChanges();
        }

        public void Delete(Client client)
        {
            if (client.Id == default)
            {
                return;
            }
            dbContext.Entry(client).State = EntityState.Deleted;
            dbContext.SaveChanges();
        }

        public async Task CreateAsync(Client client)
        {
            await dbContext.Clients.AddAsync(client);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Client client)
        {
            dbContext.Entry(client).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var client = await GetByIdAsync(id);
            if (client != null)
            {
                dbContext.Clients.Remove(client);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
