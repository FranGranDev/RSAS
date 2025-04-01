using Application.Areas.Identity.Data;

namespace Application.Services.Repository
{
    public interface IClientsStore : IStringKeyStore<Client>
    {
        Task<Client> GetByUserIdAsync(string userId);
    }
}