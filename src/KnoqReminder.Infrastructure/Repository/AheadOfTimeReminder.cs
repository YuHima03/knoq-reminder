using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KnoqReminder.Infrastructure.Repository;

[Table("ahead_of_time_reminders")]
[Index("ReminderId", Name = "reminder_id")]
public partial class AheadOfTimeReminder
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("reminder_id")]
    public Guid ReminderId { get; set; }

    /// <summary>
    /// Duration before the event; Seconds is ignored; 00:00:00(on time) ~ 24:00:00(before a day)
    /// </summary>
    [Column("duration", TypeName = "time")]
    public TimeSpan Duration { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at", TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; }

    [ForeignKey("ReminderId")]
    [InverseProperty("AheadOfTimeReminders")]
    public virtual UserReminder Reminder { get; set; } = null!;
}
