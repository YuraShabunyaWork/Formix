using Formix.Models.DB;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Formix.Persistence.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Template> Templates { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<AnswersUser> AnswersUsers { get; set; }
        public DbSet<Review> Reviews { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AppUser>()
                .HasIndex(u => u.Email)
                .IsUnique(true);             
            builder.Entity<Template>()
                .HasMany(t => t.Questions)
                .WithOne(q => q.Template)
                .HasForeignKey(t => t.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Template>()
                .HasMany(t => t.Answers)
                .WithOne(a => a.Template)
                .HasForeignKey(a => a.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Template>()
                .HasMany(r => r.Reviews)
                .WithOne(t => t.Template)
                .HasForeignKey(t => t.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Answer>()
                .HasMany(a => a.AnswersUser) 
                .WithOne(u => u.Answer)
                .HasForeignKey(u => u.AnswerId)
                .OnDelete(DeleteBehavior.Cascade);
            base.OnModelCreating(builder);
        }
    }
}
