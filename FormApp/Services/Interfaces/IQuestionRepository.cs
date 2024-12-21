using Formix.Models.DB;

namespace Formix.Services.Interfaces
{
    public interface IQuestionRepository
    {
        Task<ICollection<Question>> GetQuestionsForTamplateAsync(int idForm);
    }
}
