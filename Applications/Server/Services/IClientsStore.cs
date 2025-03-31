using Application.Areas.Identity.Data;

namespace Application.Services
{
    public interface IClientsStore
    {
        public Client Get(AppUser user);
        public void Save(AppUser user, Client client);
    }
}
