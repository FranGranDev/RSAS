using Application.Areas.Identity.Data;
using Application.Services;

namespace Application.Data.Repository
{
    public class EFCompanyStore : ICompanyStore
    {
        public EFCompanyStore(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        private readonly AppDbContext dbContext;



        public Company Get(AppUser user)
        {
            if (user == null)
                return null;
            return dbContext.Companies.FirstOrDefault(x => x.UserId == user.Id);
        }
        public void Save(AppUser user, Company company)
        {
            if (company == null)
            {
                return;
            }

            company.UserId = user.Id;

            Company other = dbContext.Companies.FirstOrDefault(x => x.UserId == company.UserId);
            if (other != null)
            {
                other.Name = company.Name;
                other.Inn = company.Inn;
                other.Kpp = company.Kpp;
                other.BankName = company.BankName;
                other.BankBic = company.BankBic;
                other.BankAccount = company.BankAccount;
                other.Phone = company.Phone;
                other.Email = company.Email;
            }
            else
            {
                dbContext.Companies.Add(company);
            }

            dbContext.SaveChanges();
        }
    }
}
