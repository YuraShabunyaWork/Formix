using Formix.Models.DB;
using Formix.Models.ViewModels.Answer;
using Formix.Models.ViewModels.Question;
using Formix.Models.ViewModels.Tamplate;
using Formix.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Formix.Controllers
{
    public class AnswerController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITamplateRepository _tamplateRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IReviewRepository _reviewRepository;

        public AnswerController(UserManager<AppUser> userManager,
            ITamplateRepository tamplateRepository,
            IAnswerRepository answerRepository,
            IReviewRepository reviewRepository)
        {
            _userManager = userManager;
            _tamplateRepository = tamplateRepository;
            _answerRepository = answerRepository;
            _reviewRepository = reviewRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> OpenTamplate([FromQuery] int idTamplate)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Singup", "Account");
            
            if (await _answerRepository.AnswerForUserExistsAsync(userId, idTamplate))
            {
                TempData["tamplateId"] = idTamplate;
                return View("ResetAnswer");
            }

            if (await _tamplateRepository.TamplateExistsAsync(idTamplate))
            {
                var tamplate = await _tamplateRepository.GetTamplateAsync(idTamplate);
                var result = new TamplateViewModel
                {
                    Title = tamplate.Title,
                    Description = tamplate.Description,
                    Id = idTamplate,
                    UrlPhoto = tamplate.UrlPhoto,
                    Rating = tamplate.RatingTamplate,
                    TamplateType = tamplate.TamplateType,
                    Questions = tamplate.Questions.Select(q => new QuestionViewModel
                    {
                        Title = q.Title,
                        TypeQuestion = q.TypeQuestion,
                        OptionsAnswer = q.OptionsAnswerList
                    }).ToList(),
                    ListReviews = tamplate.Reviews.Select(r => new ReviewViewModel
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
            var tamplateId = (int)TempData["tamplateId"]!;
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Singup", "Account");

            if(await _answerRepository.AnswerForUserExistsAsync(user.Id, tamplateId))
            {
                await _answerRepository.DeleteAwswerAsync(user.Id, tamplateId);
                await _reviewRepository.DeleteReviewUserForTamplate(user.UserName, tamplateId);
            }

            return RedirectToAction("OpenTamplate", "Answer", new { idTamplate = tamplateId });
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendAnswerTamplate([FromForm] AnswerViewModel answerView)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("", "");

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Singup", "Account");

            for (int i = 0; i < answerView.Response.Length; i++)
                for (int j = 0; j < answerView.Response[i].Length; j++)
                    answerView.Response[i][j] = answerView.Response[i][j] ?? "";

            var answer = new Answer
            {
                TamplateId = answerView.TamplateId,
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
                TamplateId = answer.TamplateId
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
                return RedirectToAction("Singup","Account");
            }
            var review = new Review
            {
                Login = user.UserName,
                TamplateId = reviewView.TamplateId,
                Rating = reviewView.Rating,
                Comment = reviewView.Comment??"",
                UrlPhoto = user.UrlPhoto
            };
            await _reviewRepository.CreateReviewAsync(review);
            return RedirectToAction("Index", "Home");
        }
    }
}
