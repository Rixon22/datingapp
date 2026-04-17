using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Helpers;

public class MessageParams : PaginationRequest
{
    public string? MemberId { get; set; }
    
    [Required]
    [EnumDataType(typeof(ContainerTypes))]
    public required ContainerTypes Container { get; set; }
}

public enum ContainerTypes
{
    Inbox,
    Outbox
}