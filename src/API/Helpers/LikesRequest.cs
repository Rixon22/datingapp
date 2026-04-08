using System.ComponentModel.DataAnnotations;

namespace API.Helpers;

public class LikesRequest : PaginationRequest
{
    public string MemberId { get; set; } = "";
    [Required]
    public required string Predicate { get; set; } = "liked";   
}