using Formix.Enam;
using Formix.Models;
using Formix.Models.ViewModels.Home;
using Formix.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;

namespace Formix.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITamplateRepository _tamplateRepository;

        public HomeController(ILogger<HomeController> logger, 
            ITamplateRepository tamplateRepository)
        {
            _logger = logger;
            _tamplateRepository = tamplateRepository;
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
        [HttpGet]
        public async Task<IActionResult> OpenTemplatesByCategory([FromQuery]TamplateType action)
        {
            var tamplates = await _tamplateRepository.GetTamplatesAsync();
            IEnumerable<HomeTamplates> homeTamplates;
            if (action == 0)
            {
                homeTamplates = tamplates.Select(t => new HomeTamplates
                    {
                        TamplateId = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        UrlPhoto = t.UrlPhoto,
                        CreatedAt = t.CreatedAt,
                        Rating = t.RatingTamplate
                    });
                ViewData["Title"] = "All Templates";
            }
            else
            {
                homeTamplates = tamplates.Where(c => c.TamplateType == action)
                    .Select(t => new HomeTamplates
                    {
                        TamplateId = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        UrlPhoto = t.UrlPhoto,
                        CreatedAt = t.CreatedAt,
                        Rating = t.RatingTamplate
                    });
                ViewData["Title"] = action;
            }
            return View(homeTamplates.OrderByDescending(d => d.CreatedAt).ToList());
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Search([FromQuery]string search)
        {
            var tamplates = await _tamplateRepository.GetTamplatesAsync();
            if (tamplates == null)
            {
                TempData["ToastMessage"] = "Tamplates are not found!";
                return RedirectToAction("Index");
            }
            if (string.IsNullOrEmpty(search))
            {
                TempData["ToastMessage"] = "Enter search!";
                return RedirectToAction("Index");
            }
            tamplates = tamplates
                    .Where(t => t.Title!.ToLower().Contains(search.ToLower())
                        || t.Description.ToLower().Contains(search.ToLower()))
                    .ToList();
            var homeTamplate = tamplates.Select(t => new HomeTamplates
            {
                TamplateId = t.Id,
                Title = t.Title,
                Description = t.Description,
                UrlPhoto = t.UrlPhoto,
                CreatedAt = t.CreatedAt,
                Rating = t.RatingTamplate
            }).ToList();
            ViewData["Title"] = "Search";
            return View(homeTamplate);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
