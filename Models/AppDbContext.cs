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
    // parameterless konstruktor för att slippa argument när vi skapar objektet
    public AppDbContext() : base()
    {}
    public DbSet<User> Users { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<PlanExercise> PlanExercises { get; set; }
    public DbSet<WorkoutLog> WorkoutLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var dbPath = Path.Combine(AppContext.BaseDirectory, "snacktosixpack.db");
            Console.WriteLine("DB PATH => " + dbPath);
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Profile>()
            .HasOne<User>()
            .WithOne(u => u.Profile)
            .HasForeignKey<Profile>(p => p.UserId);
        
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
        var dbPath = Path.Combine(AppContext.BaseDirectory, "snacktosixpack.db");

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite($"Data Source={dbPath}");

        Console.WriteLine("FACTORY DB PATH => " + dbPath);

        return new AppDbContext(optionsBuilder.Options);
    }
}

