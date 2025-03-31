using Application.Areas.Identity.Data;

namespace Application.Services
{
    public interface IEmployeeStore
    {
        public Employee Get(AppUser user);
        public void Save(AppUser user, Employee employee);
    }
}
