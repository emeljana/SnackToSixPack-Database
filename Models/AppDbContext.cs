using Microsoft.EntityFrameworkCore;
using SnackToSixPack.Classes;
using Microsoft.EntityFrameworkCore.Design;

namespace SnackToSixPack.Models;

public class AppDbContext : DbContext
{
    // Detta behövs för EF CLI
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<PlanExercise> PlanExercises { get; set; }
    public DbSet<WorkoutLog> WorkoutLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=snacktosixpack.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkoutPlan>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(wp => wp.UserId);

        modelBuilder.Entity<PlanExercise>()
            .HasOne<WorkoutPlan>()
            .WithMany()
            .HasForeignKey(pe => pe.PlanId);

        modelBuilder.Entity<PlanExercise>()
            .HasOne<Exercise>()
            .WithMany()
            .HasForeignKey(pe => pe.ExerciseId);

        modelBuilder.Entity<WorkoutLog>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(wl => wl.UserId);
    }
}

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite("Data Source=snacktosixpack.db");

        return new AppDbContext(optionsBuilder.Options);
    }
}

