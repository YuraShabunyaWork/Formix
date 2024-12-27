using Formix.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Formix.Models.DB
{
    public class Question
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(300)]
        public string Title { get; set; }
        [Required]
        public QuestionType TypeQuestion { get; set; }

        [MaxLength(2000)]
        public string OptionsAnswer { get; set; } = string.Empty;
        public int TemplateId { get; set; }
        public Template? Template { get; set; }

        [NotMapped]
        public List<string> OptionsAnswerList
        {
            get => string.IsNullOrEmpty(OptionsAnswer)
                        ? new List<string>()
                        : JsonSerializer.Deserialize<List<string>>(OptionsAnswer);
            set => OptionsAnswer = JsonSerializer.Serialize(value);
        }
    }
}