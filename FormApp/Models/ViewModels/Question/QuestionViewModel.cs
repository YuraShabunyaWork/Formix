using Formix.Enums;
using System.ComponentModel.DataAnnotations;

namespace Formix.Models.ViewModels.Question
{
    public class QuestionViewModel
    {
        [Required]
        [MaxLength(300)]
        [Display(Name = "Title question")]
        public string Title { get; set; }
        public QuestionType TypeQuestion { get; set; }
        public List<string> OptionsAnswer { get; set; } = new List<string>();
    }
}
