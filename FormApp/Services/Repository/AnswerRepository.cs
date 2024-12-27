using Formix.Models.DB;
using Formix.Persistence.Data;
using Formix.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Formix.Services.Repository
{
    public class AnswerRepository : IAnswerRepository
    {
        private readonly ApplicationDbContext _db;

        public AnswerRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> AnswerForUserExistsAsync(string idUser, int idTemplate)
        {
            return await _db.Answers.AnyAsync(a => a.AppUserId == idUser && a.TemplateId == idTemplate);
        }

        public async Task<bool> CreateAwswerAsync(Answer answer)
        {
            await _db.AddAsync(answer);
            return await SaveAsync();
        }

        public async Task<bool> DeleteAwswerAsync(string idUser, int idTemplate)
        {
            var answer = await _db.Answers
                .Where(a => a.AppUserId == idUser && a.TemplateId == idTemplate)
                .FirstOrDefaultAsync();
            _db.Answers.Remove(answer);
            return await SaveAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await _db.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
