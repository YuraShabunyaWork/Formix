namespace Formix.Models.ViewModels.Home
{
    public class HomeTemplates
    {
        public int TemplateId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? UrlPhoto { get; set; }
        public float Rating { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
