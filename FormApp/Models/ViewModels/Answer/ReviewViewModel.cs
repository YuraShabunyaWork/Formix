using System.ComponentModel.DataAnnotations;

namespace Formix.Models.ViewModels.Answer
{
    public class ReviewViewModel
    {
        public int TamplateId { get; set; }
        public string Login { get; set; }
        public string UrlPhoto { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
