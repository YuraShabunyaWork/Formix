using Formix.Helper;
using Formix.Models.DB;
using Formix.Models.ViewModels.Account;
using Formix.Models.ViewModels.Answer;
using Formix.Models.ViewModels.Question;
using Formix.Models.ViewModels.Tamplate;
using Formix.Models.ViewModels.UserMenu;
using Formix.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Formix.Controllers
{
    [Authorize]
    public class UserMenuController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITamplateRepository _tamplateRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly GenerateEmail _generateEmail;

        public UserMenuController(UserManager<AppUser> userManager,
            ITamplateRepository tamplateRepository,
            ICloudinaryService cloudinaryService,
            GenerateEmail generateEmail)
        {
            _userManager = userManager;
            _tamplateRepository = tamplateRepository;
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
                    Login = appUser.UserName,
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
        public async Task<IActionResult> OpenUserTamplates()
        {
            List<Tamplate> tamplates;
            if (User.IsInRole("admin"))
            {
                tamplates = await _tamplateRepository.GetTamplatesAsync();
            }
            else 
            { 
                var appUser = await _userManager.GetUserAsync(User);
                tamplates =  await _tamplateRepository.GetTamplatesForUserAsync(appUser.Id);
            }
            if (tamplates == null)
            {
                return View();
            }
            else
            {
                var tamplateView = tamplates.Select(t => new TamplateViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    UrlPhoto = t.UrlPhoto,
                    ListUsersAnsrews = t.Answers
                    .Select(a => new UsersAnsrewForTamplate
                    {
                        Login = _userManager.FindByIdAsync(a.AppUserId).Result.UserName,
                        Email = _userManager.FindByIdAsync(a.AppUserId).Result.Email,
                        DateTime = a.DataAnswer
                    })
                    .OrderByDescending(d => d.DateTime)
                    .ToList(),
                    ListReviews = t.Reviews.Select(r => new ReviewViewModel
                    {
                        UrlPhoto = r.UrlPhoto,
                        Login = r.Login,
                        Comment = r.Comment,
                        Rating = r.Rating,
                    }).ToList()
                }).ToList();
                return View(tamplateView);
            }
        }
        [HttpGet]
        public async Task <IActionResult> OpenAnswerUser([FromQuery]int tamplateId, [FromQuery]string login)
        {
            var tamplate = await _tamplateRepository.GetTamplateAsync(tamplateId);
            var user = await _userManager.FindByNameAsync(login);
            if (tamplate != null && user != null)
            {
                var answerList = tamplate.Answers.Where(a => a.AppUserId == user.Id)
                            .Select(a => a.AnswersUser).FirstOrDefault();
                if (answerList != null)
                {
                    var userAnswer = new UserAnswerViewModel
                    {
                        Login = user.UserName,
                        Email = user.Email,
                        Title = tamplate.Title,
                        Description = tamplate.Description,
                        UrlPhoto = tamplate.UrlPhoto,
                        Questions = tamplate.Questions.Select(q => new QuestionViewModel
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
                Login = userApp.UserName,
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
            if (profileView.FilePhoto != null && profileView.FilePhoto.Length > 0)
            {
                profileView.UrlPhoto = await _cloudinaryService.UploadPhotoAsync(profileView.FilePhoto);
            }
            var userApp = await _userManager.GetUserAsync(User);
            if (userApp == null)
            {
                return RedirectToAction("Singup", "Account");
            }   

            userApp.FirstName = profileView.FirstName;
            userApp.LastName = profileView.LastName;
            if (await _userManager.FindByNameAsync(profileView.Login) != null)
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
            userApp.UrlPhoto = profileView.UrlPhoto;
            var result = await _userManager.UpdateAsync(userApp);
            if (result.Succeeded)
            {
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
                Email = userApp.Email,
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
