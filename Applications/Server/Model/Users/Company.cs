using System.ComponentModel.DataAnnotations;

namespace Application.Areas.Identity.Data
{
    public class Company
    {
        [Key] public string UserId { get; set; }


        public string Name { get; set; }

        public string Inn { get; set; }
        public string Kpp { get; set; }
        public string BankName { get; set; }
        public string BankBic { get; set; }
        public string BankAccount { get; set; }


        public string Email { get; set; }
        public string Phone { get; set; }


        public virtual AppUser User { get; set; }
    }
}