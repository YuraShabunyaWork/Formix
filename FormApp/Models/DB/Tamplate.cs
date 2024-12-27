using Formix.Enam;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Formix.Models.DB
{
    public class Tamplate
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string? Title { get; set; }
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        public string UrlPhoto { get; set; } = "/Logo.png";
        [Required]
        public TamplateType TamplateType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public List<Question> Questions { get; set; } = new();
        public List<Answer> Answers { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
        [NotMapped]
        public float RatingTamplate
        {
            get 
            {
                if (Reviews.Count > 0)
                    return RatingTamplate = (float)Reviews.Sum(r => r.Rating) / Reviews.Count;
                return RatingTamplate = 0;
            }
            private set { }
        }
    }
}
