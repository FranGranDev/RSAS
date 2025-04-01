using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }

    public class CreateEmployeeDto
    {
        [Required(ErrorMessage = "Имя сотрудника обязательно")]
        [StringLength(50, ErrorMessage = "Имя не должно превышать 50 символов")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Фамилия сотрудника обязательна")]
        [StringLength(50, ErrorMessage = "Фамилия не должна превышать 50 символов")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Должность сотрудника обязательна")]
        [StringLength(100, ErrorMessage = "Должность не должна превышать 100 символов")]
        public string Role { get; set; }

        [Required(ErrorMessage = "Email сотрудника обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Телефон сотрудника обязателен")]
        [Phone(ErrorMessage = "Некорректный формат телефона")]
        public string Phone { get; set; }
    }

    public class UpdateEmployeeDto
    {
        [Required(ErrorMessage = "Имя сотрудника обязательно")]
        [StringLength(50, ErrorMessage = "Имя не должно превышать 50 символов")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Фамилия сотрудника обязательна")]
        [StringLength(50, ErrorMessage = "Фамилия не должна превышать 50 символов")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Должность сотрудника обязательна")]
        [StringLength(100, ErrorMessage = "Должность не должна превышать 100 символов")]
        public string Role { get; set; }

        [Required(ErrorMessage = "Email сотрудника обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Телефон сотрудника обязателен")]
        [Phone(ErrorMessage = "Некорректный формат телефона")]
        public string Phone { get; set; }
    }
}