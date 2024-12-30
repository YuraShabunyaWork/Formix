using Formix.Models.DB;

namespace Formix.Services.Interfaces
{
    public interface IReviewRepository
    {
        Task<bool> SaveAsync();
        Task<bool> CreateReviewAsync(Review review);
        Task<bool> DeleteReviewUserForTemplateAsync(string login, int teplateId);
        Task<bool> DeleteAllReviewsForUserAsync(string login);
    }
}
