using API.Entities;

namespace API.Helpers;

public interface ILikesRepository
{
    void Add(MemberLike like);
    void Delete(MemberLike like);
    Task<IReadOnlyList<string>> GetCurrentMemberLikeIds(string memberId);
    Task<MemberLike?> GetMemberLike(string sourceMemberId, string targetMemberId);
    Task<IReadOnlyList<Member>> GetMemberLikes(string predicate, string memberId);
    Task<bool> SaveAllChanges();
    
}