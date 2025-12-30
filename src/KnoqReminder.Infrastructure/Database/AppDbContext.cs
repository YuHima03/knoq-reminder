using Microsoft.EntityFrameworkCore;

namespace KnoqReminder.Infrastructure.Database;

public partial class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public virtual DbSet<AheadOfTimeReminder> AheadOfTimeReminders { get; set; }

    public virtual DbSet<DailyReminder> DailyReminders { get; set; }

    public virtual DbSet<DestinationsDiscord> DestinationsDiscords { get; set; }

    public virtual DbSet<DestinationsTraq> DestinationsTraqs { get; set; }

    public virtual DbSet<UserReminder> UserReminders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AheadOfTimeReminder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("'current_timestamp()'");
            entity.Property(e => e.Offset).HasComment("Time offset before the event; Seconds is ignored; 00:00:00(on time) ~ 24:00:00(before a day)");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("'current_timestamp()'");

            entity.HasOne(d => d.Reminder).WithMany(p => p.AheadOfTimeReminders).HasConstraintName("ahead_of_time_reminders_ibfk_1");
        });

        modelBuilder.Entity<DailyReminder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("'current_timestamp()'");
            entity.Property(e => e.Time)
                .HasDefaultValueSql("'''22:00:00'''")
                .HasComment("Time of day in UTC; Seconds is ignored");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("'current_timestamp()'");

            entity.HasOne(d => d.Reminder).WithMany(p => p.DailyReminders).HasConstraintName("daily_reminders_ibfk_1");
        });

        modelBuilder.Entity<DestinationsDiscord>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("'current_timestamp()'");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("'current_timestamp()'");

            entity.HasOne(d => d.Reminder).WithMany().HasConstraintName("destinations_discord_ibfk_1");
        });

        modelBuilder.Entity<DestinationsTraq>(entity =>
        {
            entity.Property(e => e.ChannelId).HasComment("traQ channel uuid");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("'current_timestamp()'");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("'current_timestamp()'");

            entity.HasOne(d => d.Reminder).WithMany().HasConstraintName("destinations_traq_ibfk_1");
        });

        modelBuilder.Entity<UserReminder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("'current_timestamp()'");
            entity.Property(e => e.RemindsFreeEvents)
                .HasDefaultValueSql("'''none'''")
                .HasComment("none to disable, daily to enable only daily reminders, always to enable all reminders");
            entity.Property(e => e.RemindsWhenAbsent)
                .HasDefaultValueSql("'''none'''")
                .HasComment("none to disable, daily to enable only daily reminders, always to enable all reminders");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("'current_timestamp()'");
            entity.Property(e => e.UserId).HasComment("traQ user uuid");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
