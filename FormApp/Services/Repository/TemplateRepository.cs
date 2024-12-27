using Formix.Models.DB;
using Formix.Persistence.Data;
using Formix.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Formix.Services.Repository
{
    public class TemplateRepository : ITemplateRepository
    {
        private readonly ApplicationDbContext _db;

        public TemplateRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<bool> CreareTemplateAsync(Template template)
        {
            await _db.AddAsync(template);
            return await SaveAsync();
        }

        public async Task<bool> DeleteTemplateAsync(Template template)
        {
            _db.Remove(template);
            return await SaveAsync();
        }

        public async Task<bool> TemplateExistsAsync(int id)
        {
            return await _db.Templates.AnyAsync(f => f.Id == id);
        }

        public async Task<Template> GetTemplateAsync(int id)
        {
            var template = await _db.Templates
                .Where(f => f.Id == id)
                .Include(t => t.Questions)
                .Include(t => t.Answers)
                .ThenInclude(t => t.AnswersUser)
                .Include(t => t.Reviews)
                .FirstOrDefaultAsync();
            return template;
        }

        public async Task<List<Template>> GetTemplatesAsync()
        {
            return await _db.Templates
                .Include(t => t.Answers)
                .Include(t => t.Reviews)
                .ToListAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await _db.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> UpdateTemplateAsync(Template template)
        {
            _db.Update(template);
            return await SaveAsync();
        }

        public async Task<List<Template>> GetTemplatesForUserAsync(string userId)
        {
            var templates = await _db.Templates.Where(t => t.AppUserId == userId)
                .Include(t => t.Answers)
                .Include(r => r.Reviews)
                .ToListAsync();

            return templates;
        }
    }
}
