using System.ComponentModel.DataAnnotations;

namespace Formix.Models.ViewModels.Account
{
    public class    SinginViewModel
    {
        [Required]
        [Display(Name = "Login/Email")]
        public string? LoginOrEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6,ErrorMessage = "Password is too short.")]
        [MaxLength(30, ErrorMessage = "Your password is crazy")]
        public string? Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
