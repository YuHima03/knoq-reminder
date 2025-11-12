using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KnoqReminder.Infrastructure.Database;

[Table("daily_reminders")]
[Index("ReminderId", Name = "reminder_id")]
[Index("Time", Name = "time")]
public partial class DailyReminder
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("reminder_id")]
    public Guid ReminderId { get; set; }

    /// <summary>
    /// Time of day in UTC; Seconds is ignored
    /// </summary>
    [Column("time", TypeName = "time")]
    public TimeSpan Time { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at", TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; }

    [ForeignKey("ReminderId")]
    [InverseProperty("DailyReminders")]
    public virtual UserReminder Reminder { get; set; } = null!;
}
