using Application.Areas.Identity.Data;
using Application.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Repository
{
    public class EFClientStore : IClientsStore
    {
        private readonly AppDbContext _context;

        public EFClientStore(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Client> All => _context.Clients;

        public Client Get(string id)
        {
            return _context.Clients
                .Include(c => c.User)
                .FirstOrDefault(c => c.UserId == id);
        }

        public void Save(Client entity)
        {
            if (string.IsNullOrEmpty(entity.UserId))
            {
                _context.Clients.Add(entity);
            }
            else
            {
                _context.Entry(entity).State = EntityState.Modified;
            }

            _context.SaveChanges();
        }

        public void Delete(Client entity)
        {
            _context.Clients.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            return await _context.Clients
                .Include(c => c.User)
                .ToListAsync();
        }

        public async Task<Client> GetByIdAsync(string id)
        {
            return await _context.Clients
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == id);
        }

        public async Task<Client> SaveAsync(Client entity)
        {
            if (string.IsNullOrEmpty(entity.UserId))
            {
                _context.Clients.Add(entity);
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
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Client> GetByUserIdAsync(string userId)
        {
            return await _context.Clients
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
    }
}