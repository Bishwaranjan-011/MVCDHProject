using System.ComponentModel.DataAnnotations;
namespace MVCDHProject.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Id")]
        public string? Email { get; set; }

        [Required]
        [RegularExpression("^[6-9]\\d{9}", ErrorMessage = "Mobile number must be 10 digits.")]
        public string? Mobile { get; set; }
    }
}
