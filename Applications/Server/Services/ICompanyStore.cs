using Application.Areas.Identity.Data;

namespace Application.Services
{
    public interface ICompanyStore
    {
        public Company Get(AppUser user);
        public void Save(AppUser user, Company company);
    }
}
