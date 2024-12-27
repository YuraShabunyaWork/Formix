using Formix.Enam;
using Formix.Models.ViewModels.Answer;
using Formix.Models.ViewModels.Question;
using System.ComponentModel.DataAnnotations;

namespace Formix.Models.ViewModels.Template
{
    public class TemplateViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Title { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Please select a topic.")]
        [Range(1, 9, ErrorMessage = "Invalid topic selected.")]
        public TemplateType TemplateType { get; set; }
        public string UrlPhoto { get; set; } = "/Logo.png";
        public IFormFile? FilePhoto { get; set; }
        [Required]
        public List<QuestionViewModel> Questions { get; set; }
        public List<UsersAnsrewForTemplate> ListUsersAnsrews { get; set; } = new();
        public List<ReviewViewModel> ListReviews { get; set; } = new();
        public float Rating { get; set; }
    }
}
