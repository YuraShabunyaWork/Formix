using Formix.Models;
using Formix.Models.DB;
using Formix.Models.ViewModels.Home;
using Formix.Models.ViewModels.Tamplate;
using Formix.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Formix.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITamplateRepository _tamplateRepository;
        private readonly SignInManager<AppUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, 
            ITamplateRepository tamplateRepository,
            SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _tamplateRepository = tamplateRepository;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {         
            var tamplates = await _tamplateRepository.GetTamplatesAsync();
            if (tamplates == null)
                return View();
            var topTamplates = new List<HomeTamplates>();
            var lastTamplates = new List<HomeTamplates>();

            tamplates = tamplates.OrderByDescending(t => t.RatingTamplate).ToList();
            for (int i = 0; i < 3 && i < tamplates.Count(); i++)
            {
                topTamplates.Add(new HomeTamplates
                {
                    TamplateId = tamplates[i].Id,
                    Title = tamplates[i].Title,
                    Description = tamplates[i].Description,
                    Rating = tamplates[i].RatingTamplate,
                    UrlPhoto = tamplates[i].UrlPhoto
                });
            }

            tamplates = tamplates.OrderByDescending(t => t.CreatedAt).ToList();
            for (int i = 0; i < 6 && i < tamplates.Count(); i++)
            {
                lastTamplates.Add(new HomeTamplates
                {
                    TamplateId = tamplates[i].Id,
                    Title = tamplates[i].Title,
                    Description = tamplates[i].Description,
                    UrlPhoto = tamplates[i].UrlPhoto,
                    CreatedAt = tamplates[i].CreatedAt
                });
            }

            var startTamplates = new StartTampales
            {
                TopTamplates = topTamplates,
                LastTamplates = lastTamplates
            };

            return View(startTamplates);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
