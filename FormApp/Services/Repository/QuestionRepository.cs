using Formix.Models.DB;
using Formix.Persistence.Data;
using Formix.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Formix.Services.Repository
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ApplicationDbContext _db;

        public QuestionRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ICollection<Question>> GetQuestionsForTamplateAsync(int idForm)
        {
            var questions = await _db.Questions.Where(q => q.TamplateId == idForm).ToListAsync();
            return questions;
        }
    }
}
