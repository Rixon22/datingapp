namespace API.Entities;

public class Message
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Content { get; set; }
    public DateTime? DateRead { get; set; }
    public DateTime MessageSent { get; set; } = DateTime.UtcNow;
    public bool SenderDeleted { get; set; }
    public required string SenderId { get; set; }
    public bool RecipientDeleted { get; set; }
    public required string RecipientId { get; set; }

    // Navigation properties
    public Member Sender { get; set; } = null!;
    public Member Recipient { get; set; } = null!;

}