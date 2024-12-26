using Formix.Models.DB;
using Formix.Persistence.Data;
using Formix.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Formix.Services.Repository
{
    public class TamplateRepository : ITamplateRepository
    {
        private readonly ApplicationDbContext _db;

        public TamplateRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<bool> CreareTamplateAsync(Tamplate tamplate)
        {
            await _db.AddAsync(tamplate);
            return await SaveAsync();
        }

        public async Task<bool> DeleteTamplateAsync(Tamplate tamplate)
        {
            _db.Remove(tamplate);
            return await SaveAsync();
        }

        public async Task<bool> TamplateExistsAsync(int id)
        {
            return await _db.Tamplates.AnyAsync(f => f.Id == id);
        }

        public async Task<Tamplate> GetTamplateAsync(int id)
        {
            var tamplate = await _db.Tamplates
                .Where(f => f.Id == id)
                .Include(t => t.Questions)
                .Include(t => t.Answers)
                .ThenInclude(t => t.AnswersUser)
                .Include(t => t.Reviews)
                .FirstOrDefaultAsync();
            return tamplate;
        }

        public async Task<List<Tamplate>> GetTamplatesAsync()
        {
            return await _db.Tamplates
                .Include(t => t.Answers)
                .Include(t => t.Reviews)
                .ToListAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await _db.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> UpdateTamplateAsync(Tamplate tamplate)
        {
            _db.Update(tamplate);
            return await SaveAsync();
        }

        public async Task<List<Tamplate>> GetTamplatesForUserAsync(string userId)
        {
            var tamplates = await _db.Tamplates.Where(t => t.AppUserId == userId)
                .Include(t => t.Answers)
                .Include(r => r.Reviews)
                .ToListAsync();

            return tamplates;
        }
    }
}
