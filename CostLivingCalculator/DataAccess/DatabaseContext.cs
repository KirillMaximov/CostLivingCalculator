using CostLivingCalculator.Models;
using Microsoft.EntityFrameworkCore;

namespace CostLivingCalculator.DataAccess
{
    internal class DatabaseContext : DbContext
    {
        public DbSet<RegionModel> Regions { get; set; }
        public DbSet<CostLivingModel> CostLiving { get; set; }
        public DbSet<MessageModel> Messages { get; set; }
        public DbSet<AnswerModel> Answers { get; set; }
        public DbSet<CategoryPeopleModel> CategoryPeople { get; set; }
        public DbSet<MonthlyIncomeModel> MonthlyIncome { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySQL("server=localhost;database=cost_living_calculator;user=root;password=44f91C29ec");
            }
        }

        public DatabaseContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RegionModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();
            });

            modelBuilder.Entity<CostLivingModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RegionId).IsRequired();
                entity.Property(e => e.Code).IsRequired();
                entity.Property(e => e.Person).IsRequired();
                entity.Property(e => e.Worker).IsRequired();
                entity.Property(e => e.Child).IsRequired();
                entity.Property(e => e.Pensioner).IsRequired();
            });

            modelBuilder.Entity<MessageModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ChatId).IsRequired();
                entity.Property(e => e.MessageId).IsRequired();
                entity.Property(e => e.Type).IsRequired(false);
                entity.Property(e => e.Created).IsRequired();
            });

            modelBuilder.Entity<AnswerModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ChatId).IsRequired();
                entity.Property(e => e.RegionId).IsRequired();
                entity.Property(e => e.NumberPeople).IsRequired();
                entity.Property(e => e.CreateDate).IsRequired();
            });

            modelBuilder.Entity<CategoryPeopleModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ChatId).IsRequired();
                entity.Property(e => e.Category).IsRequired();
            });

            modelBuilder.Entity<MonthlyIncomeModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ChatId).IsRequired();
                entity.Property(e => e.Amount).IsRequired();
                entity.Property(e => e.Month).IsRequired(false);
            });
        }
    }
}
