using Formix.Enam;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Formix.Models.DB
{
    public class Template
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string? Title { get; set; }
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        public string UrlPhoto { get; set; } = "/Logo.jpg";
        [Required]
        public TemplateType TemplateType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public List<Question> Questions { get; set; } = new();
        public List<Answer> Answers { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
        [NotMapped]
        public float RatingTemplate
        {
            get 
            {
                if (Reviews.Count > 0)
                    return RatingTemplate = (float)Reviews.Sum(r => r.Rating) / Reviews.Count;
                return RatingTemplate = 0;
            }
            private set { }
        }
    }
}
