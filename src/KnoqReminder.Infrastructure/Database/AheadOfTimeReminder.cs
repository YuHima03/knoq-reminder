using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KnoqReminder.Infrastructure.Database;

[Table("ahead_of_time_reminders")]
[Index("Offset", Name = "offset")]
[Index("ReminderId", "Offset", Name = "reminder_id", IsUnique = true)]
public partial class AheadOfTimeReminder
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("reminder_id")]
    public Guid ReminderId { get; set; }

    /// <summary>
    /// Time offset before the event; Seconds is ignored; 00:00:00(on time) ~ 24:00:00(before a day)
    /// </summary>
    [Column("offset", TypeName = "time")]
    public TimeSpan Offset { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at", TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; }

    [ForeignKey("ReminderId")]
    [InverseProperty("AheadOfTimeReminders")]
    public virtual UserReminder Reminder { get; set; } = null!;
}
