using System.ComponentModel.DataAnnotations;

namespace Formix.Models.ViewModels.Account
{
    public class ConfirmationEmailViewModel
    {
        public string Login {  get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; } = false;
        [Required]
        public string[] Code { get; set; } 
    }
}
