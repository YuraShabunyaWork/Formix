using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Formix.Enums;
using Formix.Services.Interfaces;
using Formix.Models.ViewModels.Tamplate;
using Formix.Models.ViewModels.Question;
using Formix.Models.DB;

namespace Formix.Controllers
{
    [Authorize]
    public class TamplateController : Controller
    {
        private readonly ITamplateRepository _tamplateRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICloudinaryService _cloudinaryService;

        public TamplateController(ITamplateRepository tamplateRepository,
            IQuestionRepository questionRepository,
            IAnswerRepository answerRepository,
            UserManager<AppUser> userManager,
            ICloudinaryService cloudinaryService)
        {
            _tamplateRepository = tamplateRepository;
            _userManager = userManager;
            _cloudinaryService = cloudinaryService;
        }
        [HttpGet]
        [Authorize]
        public IActionResult CreateTamplate()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTamplate(TamplateViewModel tamplateCreate)
        {
            if (!ModelState.IsValid) 
                return View(tamplateCreate);

            var userId = _userManager.GetUserId(User);
            if (userId == null) 
                return RedirectToAction("Login", "Account");

            var newUrl = await _cloudinaryService.UploadPhotoAsync(tamplateCreate.FilePhoto);
            var tamplate = new Tamplate
            {
                Title = tamplateCreate.Title,
                Description = tamplateCreate.Description ?? string.Empty,
                AppUserId = userId,
                UrlPhoto = newUrl,
                Questions = tamplateCreate.Questions.Select(q => new Question 
                    { 
                        Title = q.Title, 
                        TypeQuestion = q.TypeQuestion,
                        OptionsAnswerList = q.OptionsAnswer
                    }).ToList()
            };

            if(await _tamplateRepository.CreareTamplateAsync(tamplate))
            {
                TempData["ToastMessage"] = "The template has been created successfully!";
                return RedirectToAction("OpenUserTamplates", "UserMenu");
            }

            return View("CreateTamplate", tamplateCreate);
        }

        [HttpGet]
        public IActionResult GetQuestion(QuestionType questionType)
        {
            var question = new QuestionViewModel { TypeQuestion = questionType };
            return PartialView("_GetQuestionPartialView", question);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DeleteTamplate([FromQuery] int idTampalte)
        {
            if(await _tamplateRepository.TamplateExistsAsync(idTampalte))
            {
                var tamplate = await _tamplateRepository.GetTamplateAsync(idTampalte);
                if(await _tamplateRepository.DeleteTamplateAsync(tamplate))
                {
                    await _cloudinaryService.DeletePhotoAsync(tamplate.UrlPhoto);
                    TempData["ToastMessage"] = "The template has been deleted successfully!";
                    return RedirectToAction("OpenUserTamplates", "UserMenu");
                }
            }
            return View();
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditTamplate([FromQuery] int idTampalte)
        {
            if (await _tamplateRepository.TamplateExistsAsync(idTampalte))
            {
                var tamplate = await _tamplateRepository.GetTamplateAsync(idTampalte);
                var result = new TamplateViewModel
                {
                    Title = tamplate.Title,
                    Description = tamplate.Description,
                    Id = tamplate.Id,
                    UrlPhoto = tamplate.UrlPhoto,
                    Questions = tamplate.Questions.Select(q => new QuestionViewModel
                    {
                        Title = q.Title,
                        TypeQuestion = q.TypeQuestion,
                        OptionsAnswer = q.OptionsAnswerList
                    }).ToList()
                };
                return View(result);
            }
            else
            {
                return RedirectToAction("OpenUserTamplates", "UserMenu");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateTamplate([FromForm]TamplateViewModel tamplateView)
        {
            if (await _tamplateRepository.TamplateExistsAsync(tamplateView.Id))
            {
                var tamplate = await _tamplateRepository.GetTamplateAsync(tamplateView.Id);
                tamplate.Title = tamplateView.Title;
                tamplate.Description = tamplateView.Description;
                await _cloudinaryService.DeletePhotoAsync(tamplate.UrlPhoto);
                tamplate.UrlPhoto = await _cloudinaryService.UploadPhotoAsync(tamplateView.FilePhoto);
                tamplate.Questions = tamplateView.Questions.Select(q => new Question
                {
                    Title = q.Title,
                    TypeQuestion = q.TypeQuestion,
                    OptionsAnswerList = q.OptionsAnswer,
                }).ToList();
                tamplate.Answers = new List<Answer>();
                await _tamplateRepository.UpdateTamplateAsync(tamplate);
            }
            TempData["ToastMessage"] = "The template has been updated successfully!";
            return RedirectToAction("OpenUserTamplates", "UserMenu");
        }
    }
}
