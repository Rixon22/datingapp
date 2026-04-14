namespace API.DTOs;

public class MessageRequest
{
    public required string RecipientId { get; set; }
    public required string Content { get; set; }
}