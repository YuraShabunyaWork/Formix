using Formix.Models.DB;
using Formix.Persistence.Data;
using Formix.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Formix.Services.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ReviewRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> CreateReviewAsync(Review review)
        {
            await _dbContext.AddAsync(review);
            return await SaveAsync();
        }

        public async Task<bool> DeleteReviewUserForTemplate(string login, int templateId)
        {
            var reviews = await _dbContext.Reviews
                .Where(r => r.Login == login && r.TemplateId == templateId)
                .ToListAsync();
            _dbContext.RemoveRange(reviews);
            return await SaveAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
