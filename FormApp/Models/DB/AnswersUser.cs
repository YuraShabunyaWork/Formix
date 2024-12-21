using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Formix.Models.DB
{
    public class AnswersUser
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int NumberQuestion { get; set; }
        [Required]
        [MaxLength(500)]
        public string Response { get; set; } = string.Empty;
        [Required]
        public int AnswerId { get; set; }
        public Answer? Answer { get; set; }

        [NotMapped]
        public List<string> ResponseList
        {
            get => string.IsNullOrEmpty(Response)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(Response);
            set => Response = JsonSerializer.Serialize(value);
        }
    }
}
