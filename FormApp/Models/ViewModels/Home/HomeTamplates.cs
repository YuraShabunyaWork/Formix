using Formix.Models.ViewModels.Answer;

namespace Formix.Models.ViewModels.Home
{
    public class HomeTamplates
    {
        public int TamplateId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? UrlPhoto { get; set; }
        public float Rating { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
