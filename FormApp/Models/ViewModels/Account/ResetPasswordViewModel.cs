using System.ComponentModel.DataAnnotations;

namespace Formix.Models.ViewModels.Account
{
    public class ResetPasswordViewModel
    {
        public string? Code { get; set; }
        public string? Login {  get; set; }
        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password is too short.")]
        [MaxLength(30, ErrorMessage = "Your password is crazy")]
        public string? Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage = "")]
        [Display(Name = "Confirm Password")]
        public string? ConfirmPassword { get; set; }
    }
}
