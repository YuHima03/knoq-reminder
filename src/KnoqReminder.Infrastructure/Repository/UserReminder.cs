using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KnoqReminder.Infrastructure.Repository;

[Table("user_reminders")]
[Index("UserId", Name = "user_id")]
public partial class UserReminder
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// traQ user uuid
    /// </summary>
    [Column("user_id")]
    public Guid UserId { get; set; }

    /// <summary>
    /// none to disable, daily to enable only daily reminders, always to enable all reminders
    /// </summary>
    [Column("reminds_when_absent")]
    [StringLength(7)]
    public string RemindsWhenAbsent { get; set; } = null!;

    /// <summary>
    /// none to disable, daily to enable only daily reminders, always to enable all reminders
    /// </summary>
    [Column("reminds_free_events")]
    [StringLength(7)]
    public string RemindsFreeEvents { get; set; } = null!;

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at", TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; }

    [InverseProperty("Reminder")]
    public virtual ICollection<AheadOfTimeReminder> AheadOfTimeReminders { get; set; } = [];

    [InverseProperty("Reminder")]
    public virtual ICollection<DailyReminder> DailyReminders { get; set; } = [];
}
