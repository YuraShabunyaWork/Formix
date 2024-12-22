using Formix.Models.ViewModels.Question;

namespace Formix.Models.ViewModels.UserMenu
{
    public class UserAnswerViewModel
    {
        public string Login { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string UrlPhoto { get; set; }
        public List<QuestionViewModel> Questions { get; set; }
        public List<List<string>> Answers { get; set; }
    }
}
