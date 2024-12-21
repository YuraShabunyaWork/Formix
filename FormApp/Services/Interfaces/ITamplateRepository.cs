using Formix.Models.DB;

namespace Formix.Services.Interfaces
{
    public interface ITamplateRepository
    {
        Task<List<Tamplate>> GetTamplatesAsync();
        Task<List<Tamplate>> GetTamplatesForUserAsync(string userId);
        Task<Tamplate> GetTamplateAsync(int id);
        Task<bool> TamplateExistsAsync(int id);
        Task<bool> CreareTamplateAsync(Tamplate tamplate);
        Task<bool> UpdateTamplateAsync(Tamplate tamplate);
        Task<bool> DeleteTamplateAsync(Tamplate tamplate);
        Task<bool> SaveAsync();

    }
}
