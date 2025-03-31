using Application.Areas.Identity.Data;

namespace Application.Services
{
    public interface ICompanyRepository
    {
        public void Save(Company company);
    }
}
