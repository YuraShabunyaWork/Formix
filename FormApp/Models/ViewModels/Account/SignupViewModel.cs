using System.ComponentModel.DataAnnotations;

namespace Formix.Models.ViewModels.Account
{
    public class SignupViewModel
    {
        [Required]
        [RegularExpression("^[a-z0-9\\-._@]+$",
            ErrorMessage = "The login must contain only small letters, digits and special characters( - . _ )")]
        public string? Login { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password is too short.")]
        [MaxLength(30, ErrorMessage = "Your password is crazy")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [Display(Name = "Confirm Password")]
        public string? ConfirmPassword { get; set; }
    }
}
