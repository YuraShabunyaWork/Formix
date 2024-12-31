using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Formix.Enums;
using Formix.Services.Interfaces;
using Formix.Models.ViewModels.Template;
using Formix.Models.ViewModels.Question;
using Formix.Models.DB;

namespace Formix.Controllers
{
    [Authorize]
    public class TemplateController : Controller
    {
        private readonly ITemplateRepository _templateRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICloudinaryService _cloudinaryService;

        public TemplateController(ITemplateRepository templateRepository,
            IQuestionRepository questionRepository,
            IAnswerRepository answerRepository,
            UserManager<AppUser> userManager,
            ICloudinaryService cloudinaryService)
        {
            _templateRepository = templateRepository;
            _userManager = userManager;
            _cloudinaryService = cloudinaryService;
        }
        [HttpGet]
        [Authorize]
        public IActionResult CreateTemplate()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTemplate(TemplateViewModel templateCreate)
        {
            if (!ModelState.IsValid) 
                return View(templateCreate);

            var userId = _userManager.GetUserId(User);
            if (userId == null) 
                return RedirectToAction("Login", "Account");

            string newUrl;
            if (templateCreate.FilePhoto != null && templateCreate.FilePhoto.Length > 0)
            {
                newUrl = await _cloudinaryService.UploadPhotoAsync(templateCreate.FilePhoto);
            }
            else
            {
                newUrl = "/Logo.jpg";
            }
            var template = new Template
            {
                Title = templateCreate.Title,
                Description = templateCreate.Description ?? string.Empty,
                AppUserId = userId,
                UrlPhoto = newUrl,
                TemplateType = templateCreate.TemplateType,
                Questions = templateCreate.Questions.Select(q => new Question 
                    { 
                        Title = q.Title, 
                        TypeQuestion = q.TypeQuestion,
                        OptionsAnswerList = q.OptionsAnswer
                    }).ToList()
            };

            if(await _templateRepository.CreareTemplateAsync(template))
            {
                TempData["ToastMessage"] = "The template has been created successfully!";
                return RedirectToAction("OpenUserTemplates", "UserMenu");
            }

            return View("CreateTemplate", templateCreate);
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetQuestion(QuestionType questionType)
        {
            var question = new QuestionViewModel { TypeQuestion = questionType };
            return PartialView("_GetQuestionPartialView", question);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DeleteTemplate([FromQuery] int idTemplate)
        {
            if(await _templateRepository.TemplateExistsAsync(idTemplate))
            {
                var template = await _templateRepository.GetTemplateAsync(idTemplate);
                if(await _templateRepository.DeleteTemplateAsync(template))
                {
                    await _cloudinaryService.DeletePhotoAsync(template.UrlPhoto);
                    TempData["ToastMessage"] = "The template has been deleted successfully!";
                    return RedirectToAction("OpenUserTemplates", "UserMenu");
                }
            }
            return View();
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditTemplate([FromQuery] int idTemplate)
        {
            if (await _templateRepository.TemplateExistsAsync(idTemplate))
            {
                var template = await _templateRepository.GetTemplateAsync(idTemplate);
                var result = new TemplateViewModel
                {
                    Title = template.Title,
                    Description = template.Description,
                    Id = template.Id,
                    UrlPhoto = template.UrlPhoto,
                    TemplateType = template.TemplateType,
                    Questions = template.Questions.Select(q => new QuestionViewModel
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
                return RedirectToAction("OpenUserTemplates", "UserMenu");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditTemplate([FromForm]TemplateViewModel templateView)
        {
            if (!ModelState.IsValid)
                return View(templateView);

            if (await _templateRepository.TemplateExistsAsync(templateView.Id))
            {
                var template = await _templateRepository.GetTemplateAsync(templateView.Id);
                template.Title = templateView.Title;
                template.Description = templateView.Description?? "";
                template.TemplateType = templateView.TemplateType;
                if (templateView.FilePhoto != null && templateView.FilePhoto.Length > 0)
                {
                    await _cloudinaryService.DeletePhotoAsync(template.UrlPhoto);
                    template.UrlPhoto = await _cloudinaryService.UploadPhotoAsync(templateView.FilePhoto);
                }
                template.Questions = templateView.Questions.Select(q => new Question
                {
                    Title = q.Title,
                    TypeQuestion = q.TypeQuestion,
                    OptionsAnswerList = q.OptionsAnswer,
                }).ToList();
                template.Reviews = new List<Review>();
                template.Answers = new List<Answer>();
                await _templateRepository.UpdateTemplateAsync(template);
            }
            TempData["ToastMessage"] = "The template has been updated successfully!";
            return RedirectToAction("OpenUserTemplates", "UserMenu");
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SaveTemplate(int idTemplate)
        {
            var refererUrl = HttpContext.Request.Headers["Referer"].ToString();
            var template = await _templateRepository.GetTemplateAsync(idTemplate);
            if(template == null)
            {
                TempData["ToastMessage"] = "The template does not exist!";
                return Redirect(refererUrl);
            }
            string userId = _userManager.GetUserId(User)!;
            if (template!.AppUserId == userId)
            {
                TempData["ToastMessage"] = "You have such a template!";
                return Redirect(refererUrl);
            }
            else
            {
                var newQuestions = template.Questions.Select(q => new Question
                {
                    Title = q.Title,
                    TypeQuestion = q.TypeQuestion,
                    OptionsAnswer = q.OptionsAnswer,
                }).ToList();
                var newTeplate = new Template
                {
                    Title = template.Title,
                    Description = template.Description,
                    TemplateType = template.TemplateType,
                    UrlPhoto = template.UrlPhoto,
                    Questions = newQuestions,
                    AppUserId = userId,                   
                };
                if (await _templateRepository.CreareTemplateAsync(newTeplate))
                    TempData["ToastMessage"] = "The template was successfully saved!";
                else
                    TempData["ToastMessage"] = "Something's wrong";
                return Redirect(refererUrl);
            }
        }
    }
}
