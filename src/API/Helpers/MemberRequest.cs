namespace API.Helpers;

public class MemberRequest : PaginationRequest
{
    public string? Gender { get; set; }
    public string? CurrentMemberId { get; set; }
}