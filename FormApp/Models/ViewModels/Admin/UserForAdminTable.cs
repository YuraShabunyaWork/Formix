namespace Formix.Models.ViewModels.Admin
{
    public class UserForAdminTable
    {
        public string Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string UserRole { get; set; }
        public bool IsLocked { get; set; }
    }
}
