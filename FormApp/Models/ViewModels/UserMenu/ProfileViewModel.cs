using System.ComponentModel.DataAnnotations;

namespace Formix.Models.ViewModels.UserMenu
{
    public class ProfileViewModel
    {
        [Display(Name = "First Name:")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name:")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Login:")]
        public string Login { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email:")]
        public string Email { get; set; }
        [Display(Name = "Birthday:")]
        public DateTime BirthDay { get; set; }
        [Display(Name = "Phone number:")]
        public string? PhoneNumber { get; set; }
        public string UrlPhoto { get; set; }
        public IFormFile FilePhoto { get; set; }
    }
}
