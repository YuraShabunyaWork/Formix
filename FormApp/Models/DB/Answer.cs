using System.ComponentModel.DataAnnotations;

namespace Formix.Models.DB
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }
        public List<AnswersUser> AnswersUser { get; set; } = new();
        [Required]
        public string AppUserId { get; set; }
        public DateTime DataAnswer { get; set; } = DateTime.UtcNow;
        [Required]
        public int TamplateId { get; set; }
        public Tamplate Tamplate { get; set; }
    }
}
