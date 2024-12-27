using Formix.Models.DB;

namespace Formix.Services.Interfaces
{
    public interface ITemplateRepository
    {
        Task<List<Template>> GetTemplatesAsync();
        Task<List<Template>> GetTemplatesForUserAsync(string userId);
        Task<Template> GetTemplateAsync(int id);
        Task<bool> TemplateExistsAsync(int id);
        Task<bool> CreareTemplateAsync(Template template);
        Task<bool> UpdateTemplateAsync(Template template);
        Task<bool> DeleteTemplateAsync(Template template);
        Task<bool> SaveAsync();

    }
}
