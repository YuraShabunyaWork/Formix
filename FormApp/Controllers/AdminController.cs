using Formix.Models.DB;
using Formix.Models.ViewModels.Admin;
using Formix.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Formix.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminController(UserManager<AppUser> userManager,
            ITemplateRepository templateRepository)
        {
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> RunUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<UserForAdminTable>();
            foreach (var user in users)
            {
                if (user != null)
                {
                    var role = await _userManager.IsInRoleAsync(user, "admin") ? "admin" : "user";
                    result.Add(new UserForAdminTable
                    {
                        Id = user.Id,
                        Login = user.UserName,
                        Email = user.Email,
                        UserRole = role,
                        IsLocked = await _userManager.IsLockedOutAsync(user)
                    });
                }
            }
            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> RunUsers(List<string> selectedUsers, string action)
        {
            var users = new List<AppUser>();
            foreach(var id in selectedUsers)
            {
                var user = await _userManager.FindByIdAsync(id);
                if(user != null)
                    users.Add(user);
            }
            foreach (var user in users)
            {
                if (user != null)
                {
                    switch (action)
                    {
                        case "lock":
                            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                            break;
                        case "unLock":
                            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now);
                            break;
                        case "makeAdmin":
                            await _userManager.AddToRoleAsync(user, "admin");
                            break;
                        case "makeUser":
                            await _userManager.RemoveFromRoleAsync(user, "admin");
                            break;
                        case "delete":
                            await _userManager.DeleteAsync(user);
                            break;
                    }
                }
            }
            TempData["ToastMessage"] = "Data saved successfully!";
            return await RunUsers();
        }

    }
}
