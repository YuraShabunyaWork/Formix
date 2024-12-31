using Formix.Models.DB;
using Formix.Models.ViewModels.Answer;
using Formix.Models.ViewModels.Question;
using Formix.Models.ViewModels.Template;
using Formix.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Formix.Controllers
{
    public class AnswerController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITemplateRepository _templateRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IReviewRepository _reviewRepository;

        public AnswerController(UserManager<AppUser> userManager,
            ITemplateRepository templateRepository,
            IAnswerRepository answerRepository,
            IReviewRepository reviewRepository)
        {
            _userManager = userManager;
            _templateRepository = templateRepository;
            _answerRepository = answerRepository;
            _reviewRepository = reviewRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> OpenTemplate([FromQuery] int idTemplate)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Signup", "Account");
            
            if (await _answerRepository.AnswerForUserExistsAsync(userId, idTemplate))
            {
                TempData["templateId"] = idTemplate;
                return View("ResetAnswer");
            }

            if (await _templateRepository.TemplateExistsAsync(idTemplate))
            {
                var template = await _templateRepository.GetTemplateAsync(idTemplate);
                var result = new TemplateViewModel
                {
                    Title = template.Title,
                    Description = template.Description,
                    Id = idTemplate,
                    UrlPhoto = template.UrlPhoto,
                    Rating = template.RatingTemplate,
                    TemplateType = template.TemplateType,
                    Questions = template.Questions.Select(q => new QuestionViewModel
                    {
                        Title = q.Title,
                        TypeQuestion = q.TypeQuestion,
                        OptionsAnswer = q.OptionsAnswerList
                    }).ToList(),
                    ListReviews = template.Reviews.Select(r => new ReviewViewModel
                    {
                        Login = r.Login,
                        Rating = r.Rating,
                        Comment = r.Comment,   
                        UrlPhoto = r.UrlPhoto,
                    }).ToList()
                };
                return View(result);
            }
            return View("~/");
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ResetAnswer()
        {
            var templateId = (int)TempData["templateId"]!;
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Signup", "Account");

            if(await _answerRepository.AnswerForUserExistsAsync(user.Id, templateId))
            {
                await _answerRepository.DeleteAwswerAsync(user.Id, templateId);
                await _reviewRepository.DeleteReviewUserForTemplateAsync(user.UserName, templateId);
            }

            return RedirectToAction("OpenTemplate", "Answer", new { idTemplate = templateId });
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendAnswerTemplate([FromForm] AnswerViewModel answerView)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("", "");

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Signup", "Account");

            for (int i = 0; i < answerView.Response.Length; i++)
                for (int j = 0; j < answerView.Response[i].Length; j++)
                    answerView.Response[i][j] = answerView.Response[i][j] ?? "";

            var answer = new Answer
            {
                TemplateId = answerView.TemplateId,
                AppUserId = userId,
                AnswersUser = answerView.Response
                    .Select((response, i) => new AnswersUser
                    {
                        NumberQuestion = i,
                        ResponseList = response.ToList()
                    })
                    .ToList()
            };

            await _answerRepository.CreateAwswerAsync(answer);
            return View("Review", new ReviewViewModel
            {
                TemplateId = answer.TemplateId
            });
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Review(ReviewViewModel reviewView)
        {
            if (reviewView.Rating < 1)
            {
                ModelState.AddModelError("Rating", "Select rating");
                return View("Review", reviewView);
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null) 
            {
                ModelState.AddModelError("", "User does not exist");
                return RedirectToAction("Signup","Account");
            }
            var review = new Review
            {
                Login = user.UserName,
                TemplateId = reviewView.TemplateId,
                Rating = reviewView.Rating,
                Comment = reviewView.Comment??"",
                UrlPhoto = user.UrlPhoto
            };
            await _reviewRepository.CreateReviewAsync(review);
            return RedirectToAction("Index", "Home");
        }
    }
}
