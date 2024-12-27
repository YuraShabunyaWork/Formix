using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Formix.Models.DB
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public string Login {  get; set; }
        public string UrlPhoto {get;set;}
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int TemplateId { get; set; }
        public Template Template { get; set; }
    }
}
