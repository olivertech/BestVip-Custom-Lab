using BestVipCustomLab.Domain;
using Microsoft.EntityFrameworkCore;

namespace BestVipCustomLab.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<CampaignTheme> CampaignThemes => Set<CampaignTheme>();
    public DbSet<CampaignAsset> CampaignAssets => Set<CampaignAsset>();
    public DbSet<CampaignContent> CampaignContents => Set<CampaignContent>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<SurveyQuestion> SurveyQuestions => Set<SurveyQuestion>();
    public DbSet<SurveyOption> SurveyOptions => Set<SurveyOption>();
    public DbSet<SurveyResponse> SurveyResponses => Set<SurveyResponse>();
    public DbSet<SurveyAnswer> SurveyAnswers => Set<SurveyAnswer>();
    public DbSet<Visitor> Visitors => Set<Visitor>();
    public DbSet<TrafficSource> TrafficSources => Set<TrafficSource>();
    public DbSet<ConsentRecord> ConsentRecords => Set<ConsentRecord>();
    public DbSet<VipInterest> VipInterests => Set<VipInterest>();
    public DbSet<Interaction> Interactions => Set<Interaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.HasIndex(x => x.Slug).IsUnique();
            entity.HasOne(x => x.Theme)
                .WithOne()
                .HasForeignKey<CampaignTheme>(x => x.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(x => x.Assets).WithOne().HasForeignKey(x => x.CampaignId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(x => x.Contents).WithOne().HasForeignKey(x => x.CampaignId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(x => x.Products).WithOne().HasForeignKey(x => x.CampaignId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(x => x.SurveyQuestions).WithOne().HasForeignKey(x => x.CampaignId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(x => x.VipInterests).WithOne().HasForeignKey(x => x.CampaignId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Visitor>(entity =>
        {
            entity.HasIndex(x => x.Email).IsUnique();
            entity.HasMany(x => x.ConsentRecords).WithOne().HasForeignKey(x => x.VisitorId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SurveyQuestion>(entity =>
        {
            entity.HasMany(x => x.Options).WithOne().HasForeignKey(x => x.SurveyQuestionId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SurveyResponse>(entity =>
        {
            entity.HasMany(x => x.Answers).WithOne().HasForeignKey(x => x.SurveyResponseId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Category>().HasIndex(x => x.Slug).IsUnique();
        modelBuilder.Entity<TrafficSource>().HasIndex(x => x.Slug).IsUnique();
    }
}
