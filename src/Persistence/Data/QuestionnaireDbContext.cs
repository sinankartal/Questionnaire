using Microsoft.EntityFrameworkCore;
using Persistence.Models;

namespace Persistence.Data
{
    public class QuestionnaireDbContext : DbContext
    {
        public QuestionnaireDbContext()
        {
        }

        public QuestionnaireDbContext(DbContextOptions<QuestionnaireDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SurveySubject>(builder =>
            {
                builder.HasOne(ss => ss.Survey)
                    .WithMany(s => s.SurveySubjects)
                    .HasForeignKey(ss => ss.SurveyId);

                builder.HasOne(ss => ss.Subject)
                    .WithMany(s => s.SurveySubjects)
                    .HasForeignKey(ss => ss.SubjectId);
            });

            modelBuilder.Entity<Subject>(builder =>
            {
                builder.HasMany(s => s.Questions)
                    .WithOne(q => q.Subject)
                    .HasForeignKey(q => q.SubjectId);
            });

            modelBuilder.Entity<Question>(builder =>
            {
                builder.HasMany(q => q.AnswerOptions)
                    .WithOne(ao => ao.Question)
                    .HasForeignKey(ao => ao.QuestionId);
            });

            modelBuilder.Entity<Survey>(builder =>
            {
                builder.Property(s => s.Name).IsRequired();
            });
            
            modelBuilder.Entity<Subject>(builder =>
            {
                builder.Property(s => s.TextsJson)
                    .IsRequired();
            });

            modelBuilder.Entity<Question>(builder =>
            {
                builder.Property(q => q.TextsJson)
                    .IsRequired();
            });

            modelBuilder.Entity<AnswerOption>(builder =>
            {
                builder.Property(ao => ao.TextsJson)
                    .IsRequired();
            });
            
            modelBuilder.Entity<Answer>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Id).ValueGeneratedOnAdd();

                entity.HasOne(a => a.Survey)
                    .WithMany()
                    .HasForeignKey(a => a.SurveyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Question)
                    .WithMany()
                    .HasForeignKey(a => a.QuestionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.AnswerOption)
                    .WithMany()
                    .HasForeignKey(a => a.AnswerOptionId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                // Configure properties
                entity.Property(a => a.UserId).IsRequired();
                entity.Property(a => a.Department).IsRequired();
                entity.Property(a => a.SurveyId).IsRequired();
                entity.Property(a => a.QuestionId).IsRequired();
                entity.Property(a => a.CreatedDate).IsRequired();
            });
        }


        // A survey consists of multiple subjects, and each subject can have multiple questions.
        // Each question can have multiple answer options, and each answer option can have a score.
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<SurveySubject> SurveySubjects { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<AnswerOption> AnswerOptions { get; set; }
        public DbSet<Answer> Answers { get; set; }

    }
}