using Formix.Models.DB;

namespace Formix.Services.Interfaces
{
    public interface IAnswerRepository
    {
        Task<bool> AnswerForUserExistsAsync(string idUser, int idTamplate);
        Task<bool> CreateAwswerAsync(Answer answer);
        Task<bool> DeleteAwswerAsync(string idUser, int idTamplate);
        Task<bool> SaveAsync();
    }
}
