using Azure;
using System.ComponentModel.DataAnnotations;

namespace Formix.Models.ViewModels.Answer
{
    public class AnswerViewModel
    {
        public string[][] Response { get; set; } 
        public int TemplateId { get; set; }
    }
}
