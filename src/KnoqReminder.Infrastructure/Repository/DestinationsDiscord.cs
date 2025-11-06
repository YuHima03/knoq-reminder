using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KnoqReminder.Infrastructure.Repository;

[Keyless]
[Table("destinations_discord")]
[Index("ReminderId", "WebhookId", Name = "reminder_id", IsUnique = true)]
public partial class DestinationsDiscord
{
    [Column("reminder_id")]
    public Guid ReminderId { get; set; }

    [Column("webhook_id")]
    [StringLength(63)]
    public string WebhookId { get; set; } = null!;

    [Column("webhook_secret")]
    [StringLength(255)]
    public string WebhookSecret { get; set; } = null!;

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at", TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; }

    [ForeignKey("ReminderId")]
    public virtual UserReminder Reminder { get; set; } = null!;
}
