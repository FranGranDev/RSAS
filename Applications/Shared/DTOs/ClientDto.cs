using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class ClientDto
    {
        [Display(Name = "ID клиента")]
        [Required(ErrorMessage = "ID клиента обязателен")]
        public string Id { get; set; }

        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Имя клиента обязательно")]
        [StringLength(50, ErrorMessage = "Имя не должно превышать 50 символов")]
        public string FirstName { get; set; }

        [Display(Name = "Фамилия")]
        [Required(ErrorMessage = "Фамилия клиента обязательна")]
        [StringLength(50, ErrorMessage = "Фамилия не должна превышать 50 символов")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email клиента обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string Email { get; set; }

        [Display(Name = "Телефон")]
        [Required(ErrorMessage = "Телефон клиента обязателен")]
        [Phone(ErrorMessage = "Некорректный формат телефона")]
        public string Phone { get; set; }

        [Display(Name = "Полное имя")]
        public string FullName => $"{FirstName} {LastName}";
    }

    public class CreateClientDto
    {
        [Required(ErrorMessage = "Имя клиента обязательно")]
        [StringLength(50, ErrorMessage = "Имя не должно превышать 50 символов")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Фамилия клиента обязательна")]
        [StringLength(50, ErrorMessage = "Фамилия не должна превышать 50 символов")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email клиента обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Телефон клиента обязателен")]
        [Phone(ErrorMessage = "Некорректный формат телефона")]
        public string Phone { get; set; }
    }

    public class UpdateClientDto
    {
        [Required(ErrorMessage = "Имя клиента обязательно")]
        [StringLength(50, ErrorMessage = "Имя не должно превышать 50 символов")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Фамилия клиента обязательна")]
        [StringLength(50, ErrorMessage = "Фамилия не должна превышать 50 символов")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email клиента обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Телефон клиента обязателен")]
        [Phone(ErrorMessage = "Некорректный формат телефона")]
        public string Phone { get; set; }
    }
}