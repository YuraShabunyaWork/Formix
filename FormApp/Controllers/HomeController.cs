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
        private readonly ITemplateRepository _templateRepository;

        public HomeController(ILogger<HomeController> logger, 
            ITemplateRepository templateRepository)
        {
            _logger = logger;
            _templateRepository = templateRepository;
        }

        public async Task<IActionResult> Index()
        {         
            var templates = await _templateRepository.GetTemplatesAsync();
            if (templates == null)
                return View();
            var topTemplates = new List<HomeTemplates>();
            var lastTemplates = new List<HomeTemplates>();

            templates = templates.OrderByDescending(t => t.RatingTemplate).ToList();
            for (int i = 0; i < 3 && i < templates.Count(); i++)
            {
                topTemplates.Add(new HomeTemplates
                {
                    TemplateId = templates[i].Id,
                    Title = templates[i].Title,
                    Description = templates[i].Description,
                    Rating = templates[i].RatingTemplate,
                    UrlPhoto = templates[i].UrlPhoto
                });
            }

            templates = templates.OrderByDescending(t => t.CreatedAt).ToList();
            for (int i = 0; i < 6 && i < templates.Count(); i++)
            {
                lastTemplates.Add(new HomeTemplates
                {
                    TemplateId = templates[i].Id,
                    Title = templates[i].Title,
                    Description = templates[i].Description,
                    UrlPhoto = templates[i].UrlPhoto,
                    CreatedAt = templates[i].CreatedAt
                });
            }

            var startTemplates = new StartTempales
            {
                TopTemplates = topTemplates,
                LastTemplates = lastTemplates
            };

            return View(startTemplates);

        }
        [HttpGet]
        public async Task<IActionResult> OpenTemplatesByType([FromQuery]TemplateType category = 0, 
            [FromQuery]string sort = "new", 
            [FromQuery]string search = "")
        {
            var templates = await _templateRepository.GetTemplatesAsync();
            if (templates == null)
            {
                TempData["ToastMessage"] = "Templates are not found!";
                return RedirectToAction("Index");
            }
            IEnumerable<HomeTemplates> homeTemplates;
            if (category == 0)
            {
                homeTemplates = templates.Select(t => new HomeTemplates
                    {
                        TemplateId = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        UrlPhoto = t.UrlPhoto,
                        CreatedAt = t.CreatedAt,
                        Rating = t.RatingTemplate
                    });
                ViewData["Title"] = "All Templates";
            }
            else
            {
                homeTemplates = templates.Where(c => c.TemplateType == category)
                    .Select(t => new HomeTemplates
                    {
                        TemplateId = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        UrlPhoto = t.UrlPhoto,
                        CreatedAt = t.CreatedAt,
                        Rating = t.RatingTemplate
                    });
                ViewData["Title"] = category;
            }
            if(sort == "new")
            {                
                homeTemplates = homeTemplates.OrderByDescending(c => c.CreatedAt);
            }
            if(sort == "popular")
            {
                homeTemplates = homeTemplates.OrderByDescending(p => p.Rating);
            }
            ViewData["Sort"] = sort;
            homeTemplates = homeTemplates
                    .Where(t => t.Title!.ToLower().Contains(search.ToLower())
                        || t.Description!.ToLower().Contains(search.ToLower()));
            ViewData["Search"] = search;
            return View(homeTemplates.ToList());
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
