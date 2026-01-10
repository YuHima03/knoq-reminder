using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KnoqReminder.Infrastructure.Database;

[Table("destination_traq_channels")]
[Index("ReminderId", "ChannelId", Name = "reminder_id", IsUnique = true)]
public partial class DestinationTraqChannel
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("reminder_id")]
    public Guid ReminderId { get; set; }

    /// <summary>
    /// traQ channel uuid
    /// </summary>
    [Column("channel_id")]
    public Guid ChannelId { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at", TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; }

    [ForeignKey("ReminderId")]
    [InverseProperty("DestinationTraqChannels")]
    public virtual UserReminder Reminder { get; set; } = null!;
}
