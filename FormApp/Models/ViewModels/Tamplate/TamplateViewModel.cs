using Formix.Models.ViewModels.Answer;
using Formix.Models.ViewModels.Question;
using System.ComponentModel.DataAnnotations;

namespace Formix.Models.ViewModels.Tamplate
{
    public class TamplateViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Title { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
        public string? UrlPhoto { get; set; } 
        public IFormFile FilePhoto { get; set; }
        [Required]
        public List<QuestionViewModel> Questions { get; set; }
        public List<UsersAnsrewForTamplate> ListUsersAnsrews { get; set; } = new();
        public List<ReviewViewModel> ListReviews { get; set; } = new();
        public float Rating { get; set; }
    }
}
