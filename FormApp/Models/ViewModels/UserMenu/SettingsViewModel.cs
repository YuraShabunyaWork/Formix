using Formix.Models.ViewModels.Account;
using System.ComponentModel.DataAnnotations;

namespace Formix.Models.ViewModels.UserMenu
{
    public class SettingsViewModel
    {
        public string UrlPhoto { get; set; }
        public string Email { get; set; }
        [Display(Name = "On/Off")]
        public bool IsTwoFactor { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        [MinLength(6, ErrorMessage = "Password is too short.")]
        [MaxLength(30, ErrorMessage = "Your password is crazy")]
        public string? OldPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [MinLength(6, ErrorMessage = "Password is too short.")]
        [MaxLength(30, ErrorMessage = "Your password is crazy")]
        public string? NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        [Display(Name = "Confirm New Password")]
        public string? ConfirmPassword { get; set; }
    }
}
