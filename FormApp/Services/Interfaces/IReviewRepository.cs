using Formix.Models.DB;

namespace Formix.Services.Interfaces
{
    public interface IReviewRepository
    {
        Task<bool> SaveAsync();
        Task<bool> CreateReviewAsync(Review review);
        Task<bool> DeleteReviewUserForTemplate(string login, int teplateId);
    }
}
