using Formix.Helper;
using Formix.Models.DB;
using Formix.Models.ViewModels.Account;
using Formix.Models.ViewModels.Answer;
using Formix.Models.ViewModels.Question;
using Formix.Models.ViewModels.Template;
using Formix.Models.ViewModels.UserMenu;
using Formix.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Formix.Controllers
{
    [Authorize]
    public class UserMenuController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITemplateRepository _templateRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly GenerateEmail _generateEmail;

        public UserMenuController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITemplateRepository templateRepository,
            ICloudinaryService cloudinaryService,
            GenerateEmail generateEmail)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _templateRepository = templateRepository;
            _cloudinaryService = cloudinaryService;
            _generateEmail = generateEmail;
        }
        
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser != null)
            {
                var user = new ProfileViewModel
                {
                    FirstName = appUser.FirstName,
                    LastName = appUser.LastName,
                    Login = appUser.UserName!,
                    Email = appUser.Email,
                    BirthDay = appUser.BirthDay,
                    PhoneNumber = appUser.PhoneNumber,
                    UrlPhoto = appUser.UrlPhoto
                };
                return View(user);
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> OpenUserTemplates()
        {
            var userId = User.IsInRole("admin")
                ? null
                : (await _userManager.GetUserAsync(User))?.Id;

            var templates = userId == null
                ? await _templateRepository.GetTemplatesAsync()
                : await _templateRepository.GetTemplatesForUserAsync(userId);

            var templateView = new List<TemplateViewModel>();

            foreach (var template in templates)
            {
                var answers = new List<UsersAnsrewForTemplate>();
                foreach (var answer in template.Answers)
                {
                    var user = await _userManager.FindByIdAsync(answer.AppUserId);
                    if (user != null)
                    {
                        answers.Add(new UsersAnsrewForTemplate
                        {
                            Login = user!.UserName!,
                            Email = user.Email!,
                            DateTime = answer.DataAnswer
                        });
                    }
                }

                templateView.Add(new TemplateViewModel
                {
                    Id = template.Id,
                    Title = template.Title,
                    Description = template.Description,
                    UrlPhoto = template.UrlPhoto,
                    ListUsersAnsrews = answers.OrderByDescending(a => a.DateTime).ToList(),
                    ListReviews = template.Reviews.Select(r => new ReviewViewModel
                    {
                        UrlPhoto = r.UrlPhoto,
                        Login = r.Login,
                        Comment = r.Comment,
                        Rating = r.Rating
                    }).ToList()
                });
            }

            return View(templateView);
        }

        [HttpGet]
        public async Task <IActionResult> OpenAnswerUser([FromQuery]int templateId, [FromQuery]string login)
        {
            var template = await _templateRepository.GetTemplateAsync(templateId);
            var user = await _userManager.FindByNameAsync(login);
            if (template != null && user != null)
            {
                var answerList = template.Answers.Where(a => a.AppUserId == user.Id)
                            .Select(a => a.AnswersUser).FirstOrDefault();
                if (answerList != null)
                {
                    var userAnswer = new UserAnswerViewModel
                    {
                        Login = user.UserName!,
                        Email = user.Email!,
                        Title = template.Title!,
                        Description = template.Description,
                        UrlPhoto = template.UrlPhoto,
                        Questions = template.Questions.Select(q => new QuestionViewModel
                        {
                            Title = q.Title,
                            TypeQuestion = q.TypeQuestion,
                            OptionsAnswer = q.OptionsAnswerList
                        }).ToList(),
                        Answers = answerList.OrderBy(a => a.NumberQuestion)
                                            .Select(a => a.ResponseList)
                                            .ToList()
                    };
                    return View(userAnswer);
                }
            }
            return RedirectToAction("Singin","Account");
        }
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var userApp = await _userManager.GetUserAsync(User);
            if (userApp == null)
                return RedirectToAction("", "");
            var editView = new ProfileViewModel
            {
                FirstName = userApp.FirstName,
                LastName = userApp.LastName,
                Login = userApp.UserName!,
                Email = userApp.Email,
                BirthDay = userApp.BirthDay,
                PhoneNumber = userApp.PhoneNumber,
                UrlPhoto = userApp.UrlPhoto,
            };
            return View(editView);
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile(ProfileViewModel profileView)
        {
            if(!ModelState.IsValid)
            {
                return View(profileView);
            }
            var userApp = await _userManager.GetUserAsync(User);
            if (userApp == null)
            {
                return RedirectToAction("Singup", "Account");
            }   

            userApp.FirstName = profileView.FirstName;
            userApp.LastName = profileView.LastName;
            if (await _userManager.FindByNameAsync(profileView.Login) == null)
            {
                userApp.UserName = profileView.Login;
            }
            else
            {
                ModelState.AddModelError("Login", "Login is exist");
                return View(profileView);
            }
            userApp.BirthDay = profileView.BirthDay;
            userApp.PhoneNumber = profileView.PhoneNumber;
            if (profileView.FilePhoto != null && profileView.FilePhoto.Length > 0)
            {
                profileView.UrlPhoto = await _cloudinaryService.UploadPhotoAsync(profileView.FilePhoto);
            }
            userApp.UrlPhoto = profileView.UrlPhoto;
            var result = await _userManager.UpdateAsync(userApp);
            if (result.Succeeded)
            {
                var userPrincipal = await _signInManager.CreateUserPrincipalAsync(userApp);
                var claimsIdentity = userPrincipal.Identity as ClaimsIdentity;
                claimsIdentity?.AddClaim(new Claim("ProfilePhotoUrl", userApp.UrlPhoto ?? "/AvaDef.png"));

                await _signInManager.Context.SignInAsync(
                    IdentityConstants.ApplicationScheme,
                    userPrincipal);
                TempData["ToastMessage"] = "The profile has been changed successfully!";
                return View("Profile", profileView);
            }

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var userApp = await _userManager.GetUserAsync(User);
            
            if (userApp == null)
            {
                ModelState.AddModelError("User", "User does not exist");
                return RedirectToAction("Singup","Account");
            }
            var settingView = new SettingsViewModel
            {
                Email = userApp.Email!,
                UrlPhoto = userApp.UrlPhoto,
                IsTwoFactor = userApp.TwoFactorEnabled
            };
            return View(settingView);
        }
        [HttpPost]
        public async Task<IActionResult> TwoFactor(SettingsViewModel settingsView)
        {
            var user = await _userManager.FindByEmailAsync(settingsView.Email);
            if (user == null)
            {
                ModelState.AddModelError("User", "User does not exist");
                return RedirectToAction("Singup", "Account");
            }
            user.TwoFactorEnabled = settingsView.IsTwoFactor;
            var result = await _userManager.UpdateAsync(user);
            if (user.TwoFactorEnabled)
            {
                TempData["ToastMessage"] = "Two-factor authorization is enabled!";
            }
            else
            {
                TempData["ToastMessage"] = "Two-factor authorization is disabled!";
            }
            return View("Settings", settingsView);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(SettingsViewModel settingsView)
        {
            if(!ModelState.IsValid)
            {
                return View("Settings", settingsView);
            }
            var user = await _userManager.FindByEmailAsync(settingsView.Email);
            if (user == null)
            {
                ModelState.AddModelError("User", "User does not exist");
                return RedirectToAction("Singup", "Account");
            }

            var result = await _userManager.ChangePasswordAsync(user, settingsView.OldPassword, settingsView.NewPassword);
            if (result.Succeeded)
            {
                TempData["ToastMessage"] = "The password has been changed!";
                return View("Settings", settingsView);
            }
            else
            {
                var error = result.Errors.FirstOrDefault();
                ModelState.AddModelError(error.Code, error.Description);
                return View("Settings", settingsView);
            }
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangeEmail(SettingsViewModel settingsView)
        {
            string email = settingsView.Email;
            if (await _userManager.FindByEmailAsync(email) != null)
            {
                ModelState.AddModelError("Email", "Such an Email already exists");
                return View("Settings", settingsView);
            }
            var user = await _userManager.GetUserAsync(User);
            TempData["ConfirmationCode"] = await _generateEmail.CodeAsync(email);
            TempData["Action"] = "ChangeEmail";
            var confirmationEmailView = new ConfirmationEmailViewModel
            {
                Email = email
            };

            return View("ConfirmationEmail", confirmationEmailView);
        }
    }
}
