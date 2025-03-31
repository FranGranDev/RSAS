using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Application.Areas.Identity.Data;
using Application.Services;

namespace Application.Data.Repository
{
    public class EFClientStore : IClientsStore
    {
        public EFClientStore(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        private readonly AppDbContext dbContext;


        public Client Get(AppUser user)
        {
            if (user == null)
                return null;
            return dbContext.Clients.FirstOrDefault(x => x.UserId == user.Id);
        }
        public void Save(AppUser user, Client client)
        {
            if(client == null)
            {
                return;
            }

            client.UserId = user.Id;

            Client other = dbContext.Clients.FirstOrDefault(x => x.UserId == client.UserId);
            if(other != null)
            {
                other.FirstName = client.FirstName;
                other.LastName = client.LastName;
            }
            else
            {                
                dbContext.Clients.Add(client);
            }

            dbContext.SaveChanges();
        }
    }
}
