using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class MessageResponse
{
    public required string Id { get; set; } = Guid.NewGuid().ToString();
    public required string SenderId { get; set; }
    public required string SenderDisplayName { get; set; }
    public string? SenderImageUrl { get; set; }
    public required string RecipientId { get; set; }
    public required string RecipientDisplayName { get; set; }
    public string? RecipientImageUrl { get; set; }
    public required string Content { get; set; }   
    public DateTime? DateRead { get; set; }
    public DateTime MessageSent { get; set; }
}