using Formix.Models.DB;
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

        public UserMenuController(UserManager<AppUser> userManager,
            ITamplateRepository tamplateRepository,
            ICloudinaryService cloudinaryService)
        {
            _userManager = userManager;
            _tamplateRepository = tamplateRepository;
            _cloudinaryService = cloudinaryService;
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
                    ListUsersAnsrews = t.Answers.Select(a => new UsersAnsrewForTamplate
                    {
                        Login = _userManager.FindByIdAsync(a.AppUserId).Result.UserName,
                        DateTime = a.DataAnswer
                    }).ToList(),
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
            var userId = await _userManager.FindByNameAsync(login);
            if (tamplate != null && userId != null)
            {
                var answerList = tamplate.Answers.Where(a => a.AppUserId == userId.Id)
                            .Select(a => a.AnswersUser).FirstOrDefault();
                if (answerList != null)
                {
                    var userAnswer = new UserAnswerViewModel
                    {
                        Login = login,
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
            if(await _userManager.FindByEmailAsync(profileView.Email) != null)
            { 
                userApp.Email = profileView.Email;
            }
            else
            {
                ModelState.AddModelError("Email", "Email is exist");
                return View(profileView);
            }
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
            TempData["urlPhoto"] = userApp.UrlPhoto;
            if (result.Succeeded)
            {
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
            await _userManager.UpdateAsync(user);
            
            return View("Settings", settingsView);
        }
        [HttpPost]
        public async Task<IActionResult> RessetPassword(SettingsViewModel settingsView)
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
                return View("Settings", settingsView);
            }
            else
            {
                var error = result.Errors.FirstOrDefault();
                ModelState.AddModelError(error.Code, error.Description);
                return View("Settings", settingsView);
            }
        }
    }
}
