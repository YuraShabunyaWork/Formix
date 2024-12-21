using Microsoft.AspNetCore.Identity;
using System.Data;

namespace Formix.Models.DB
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime BirthDay { get; set; }
        public string UrlPhoto { get; set; } = "/AvaDef.png";
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public List<Tamplate> Tamplates { get; set; }
    }
}
